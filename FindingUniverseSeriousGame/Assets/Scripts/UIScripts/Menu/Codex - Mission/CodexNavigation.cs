using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class CodexNavigation : MonoBehaviour
{
    #region Inizializzazione variabili

    [Header("Tasti di Navigazione")]
    [Tooltip("Tasti per muovere il cursore su e giù nella lista")]
    [SerializeField] KeyCode moveUpKey = KeyCode.W;
    [SerializeField] KeyCode moveDownKey = KeyCode.S;
    [SerializeField] KeyCode altMoveUpKey = KeyCode.UpArrow;
    [SerializeField] KeyCode altMoveDownKey = KeyCode.DownArrow;
    [Tooltip("Tasti per confermare la selezione (Aprire/Chiudere le tendine)")]
    [SerializeField] KeyCode confirmKey = KeyCode.Return;
    [SerializeField] KeyCode altConfirmKey = KeyCode.Space;

    [Header("Riferimenti - UI Pannello Destro")]
    [Tooltip("Il testo che mostrerà il titolo del dato selezionato nella parte destra dello schermo")]
    [SerializeField] private TMP_Text rightPanelTitleText; // Testo del titolo (es. MERCURIO)
    [Tooltip("Il testo che mostrerà la descrizione dettagliata nella parte destra dello schermo")]
    [SerializeField] private TMP_Text rightPanelDescriptionText; // Testo del corpo della descrizione

    [Header("Riferimenti - Logica e Navigazione")]
    [Tooltip("Trascinare qui l'oggetto 'Content' della ScrollView di sinistra. (Serve per il ricalcolo del Layout)")]
    [SerializeField] private RectTransform contentRectTransform;  


    
    // Riferimenti agli altri script essenziali
    private MenuAsthetics menuAsthetics; 
    [SerializeField] MenuManager menuManager; // Il "Cervello Centrale" da cui leggere il database dei pianeti/fenomeni



    // Liste nascoste per tracciare e muovere il cursore dinamicamente
    private List<TMP_Text> selectableTexts = new List<TMP_Text>();  // Tutti i testi visibili al momento (Titoli + Voci)
    private List<bool> isNodeCategory = new List<bool>();  // True = è una macro-categoria. False = è una singola voce.
    private List<int> indexLinkedCategory = new List<int>();  // ID della macro-categoria a cui appartiene l'elemento
    private List<int> indexLinkedEntry = new List<int>(); // ID della singola voce (es. Mercurio) all'interno della categoria

    private int currentCategoryIndex = 0;  // Posizione attuale del cursore nella lista

    #endregion


    // FUNZIONE DI SICUREZZA: Controlla di avere i riferimenti a prescindere da chi si sveglia prima
    private void EnsureReferences()
    {
        if (menuAsthetics == null) menuAsthetics = GetComponent<MenuAsthetics>();
        //if (codexManager == null) codexManager = FindAnyObjectByType<CodexManager>(FindObjectsInactive.Include);
    }
    void OnEnable()
    {
        //ogni volta che attivo il pannello aggiorno la UI del codex
        RefreshFullUI();
    }

    void Awake()
    {
        // Recupero automatico dei componenti attaccati allo stesso GameObject in totale sicurezza
        EnsureReferences();
        
        // Controlli di sicurezza per evitare NullReferenceException in gioco
        if (menuAsthetics == null) Debug.LogWarning("MenuAsthetics non trovato in CodexNavigation!");
        if (menuManager == null) Debug.LogError("CodexManager non trovato! CodexNavigation ha bisogno di CodexManager per leggere i dati.");
    }

    void Update()
    {
        if (Input.GetKeyDown(moveDownKey) || Input.GetKeyDown(altMoveDownKey))
            MoveCursor(1);
        else if (Input.GetKeyDown(moveUpKey) || Input.GetKeyDown(altMoveUpKey))
            MoveCursor(-1);
        if (Input.GetKeyDown(confirmKey) || Input.GetKeyDown(altConfirmKey))
            ConfirmSelection();
    }

    /// <summary>
    /// Metodo pubblico principale. 
    /// Viene chiamato dal Manager per aggiornare l'interfaccia visiva con i dati più recenti.
    /// </summary>
    public void RefreshFullUI()
    {
        // Sicurezza: ci assicuriamo di avere i riferimenti prima di agire
        EnsureReferences();

        // Se non c'è un database valido, ci fermiamo
        if (menuManager == null || menuManager.categoryLists == null || menuManager.categoryLists.Length == 0) return;

        ClearRightPanel();
        RefreshAllLeftListTexts();
        RecalculateNavigationList();
    }



    #region Funzioni Interne di UI e Database

    /// <summary>
    /// Aggiorna i testi fisici della lista a sinistra. 
    /// Se la voce è sbloccata mostra il nome reale, altrimenti maschera con "???".
    /// </summary>
    private void RefreshAllLeftListTexts()
    {
        foreach (var category in menuManager.categoryLists)
        {
            foreach (var entry in category.entries)
            {
                if (entry.uiTextElement != null)
                {
                    entry.uiTextElement.text = entry.isDiscovered ? entry.realName : "???";
                }
            }
        }
    }

    /// <summary>
    /// Svuota il monitor di destra impostando i testi di default.
    /// </summary>
    private void ClearRightPanel()
    {
        if (rightPanelTitleText != null) rightPanelTitleText.text = "SELEZIONA UN DATO";
        if (rightPanelDescriptionText != null) rightPanelDescriptionText.text = "";
    }

    /// <summary>
    /// Aggiorna il monitor di destra leggendo i dati dal Manager in base a dove si trova il cursore.
    /// </summary>
    private void UpdateRightPanelDisplay()
    {
        // Evita errori se la lista è vuota
        if (selectableTexts.Count == 0 || currentCategoryIndex >= selectableTexts.Count) return;

        bool isCategory = isNodeCategory[currentCategoryIndex];
        int linkedCatIndex = indexLinkedCategory[currentCategoryIndex];
        int linkedEntryIndex = indexLinkedEntry[currentCategoryIndex];

        if (isCategory)
        {
            // Se siamo sopra "[+] PIANETI", svuotiamo lo schermo di destra
            ClearRightPanel();
        }
        else
        {
            // Se siamo sopra un pianeta specifico, peschiamo i suoi dati dal Manager
            CodexEntry selectedData = menuManager.categoryLists[linkedCatIndex].entries[linkedEntryIndex];

            if (selectedData.isDiscovered)
            {
                // Dato scoperto: mostriamo Titolo e Descrizione
                if (rightPanelTitleText != null) rightPanelTitleText.text = selectedData.realName;
                if (rightPanelDescriptionText != null) rightPanelDescriptionText.text = selectedData.description;
            }
            else
            {
                // Dato nascosto: mostriamo l'avviso di dati mancanti
                if (rightPanelTitleText != null) rightPanelTitleText.text = "???";
                if (rightPanelDescriptionText != null) rightPanelDescriptionText.text = "DATI MANCANTI.\nESPLORARE IL SISTEMA PER ACQUISIRE INFORMAZIONI.";
            }
        }
    }

    #endregion



    #region Funzioni di Navigazione del Codex (Chiamate dal Manager)

    /// <summary>
    /// Funzione principale della navigazione nel codex.
    /// Ricostruisce la "strada" invisibile su cui si muove il cursore.
    /// Include i titoli e le voci solo delle cartelle attualmente aperte.
    /// </summary>
    private void RecalculateNavigationList()
    {
        // Pulizia totale delle liste prima di ricostruirle
        selectableTexts.Clear();
        isNodeCategory.Clear();
        indexLinkedCategory.Clear();
        indexLinkedEntry.Clear(); 

        for (int i = 0; i < menuManager.categoryLists.Length; i++)
        {
            // 1. Aggiunge il titolo della categoria principale
            selectableTexts.Add(menuManager.categoryLists[i].categoryTitle);
            isNodeCategory.Add(true);
            indexLinkedCategory.Add(i);
            indexLinkedEntry.Add(-1); // -1 indica che non è una voce specifica

            // 2. Se la categoria è aperta, aggiunge i suoi figli
            if (menuManager.categoryLists[i].isOpen)
            {
                for (int j = 0; j < menuManager.categoryLists[i].entries.Length; j++)
                {
                    selectableTexts.Add(menuManager.categoryLists[i].entries[j].uiTextElement);
                    isNodeCategory.Add(false); 
                    indexLinkedCategory.Add(i);
                    indexLinkedEntry.Add(j); 
                }
            }
        }

        // Sicurezza: se la lista si è accorciata, riportiamo il cursore nel limite consentito
        if (currentCategoryIndex >= selectableTexts.Count) currentCategoryIndex = selectableTexts.Count - 1;
        
        // Passiamo la nuova lista a MenuAsthetics per i colori
        if (menuAsthetics != null) menuAsthetics.SetTexts(selectableTexts.ToArray());

        UpdateVisualCursor();
    }

    /// <summary>
    /// Muove l'indice di selezione in loop. Chiamato quando si premono le freccette (o W / S).
    /// </summary>
    /// <param name="direction">1 per andare in basso, -1 per andare in alto</param>
    public void MoveCursor(int direction)
    {
        if (selectableTexts.Count == 0) return;

        currentCategoryIndex += direction;

        // Gestione del Loop (salto da inizio a fine e viceversa)
        if (currentCategoryIndex >= selectableTexts.Count) currentCategoryIndex = 0;
        else if (currentCategoryIndex < 0) currentCategoryIndex = selectableTexts.Count - 1;

        UpdateVisualCursor();
    }

    /// <summary>
    /// Pulisce il cursore ">", lo applica all'elemento attuale, gestisce i colori e aggiorna il monitor destro.
    /// </summary>
    private void UpdateVisualCursor()
    {
        // 1. Pulizia dei simboli ">" da tutti i testi
        for (int i = 0; i < selectableTexts.Count; i++)
        {
            if (selectableTexts[i] == null) continue;

            bool isCat = isNodeCategory[i];
            int linkedCat = indexLinkedCategory[i];
            int linkedEnt = indexLinkedEntry[i];

            if (isCat)
            {
                // Se è una categoria, rimuoviamo solo il cursore
                selectableTexts[i].text = selectableTexts[i].text.Replace("> ", "");
            }
            else
            {
                // Se è una voce, guardiamo se è scoperta e togliamo l'eventuale cursore
                CodexEntry entryData = menuManager.categoryLists[linkedCat].entries[linkedEnt];
                selectableTexts[i].text = entryData.isDiscovered ? entryData.realName : "???";
            }
        }

        // 2. Aggiunta del simbolo ">" all'elemento selezionato
        if (currentCategoryIndex >= 0 && currentCategoryIndex < selectableTexts.Count)
        {
            TMP_Text selectedText = selectableTexts[currentCategoryIndex];
            if (!selectedText.text.StartsWith(">")) selectedText.text = "> " + selectedText.text;
        }

        // 3. Aggiornamento colori tramite script separato
        if (menuAsthetics != null) menuAsthetics.UpdateTabVisuals(currentCategoryIndex);

        // 4. Aggiornamento in tempo reale dello schermo a destra
        UpdateRightPanelDisplay();
    }

    /// <summary>
    /// Esegue l'azione del tasto Invio (Apre o chiude le tendine delle categorie).
    /// </summary>
    public void ConfirmSelection()
    {
        if (selectableTexts.Count == 0) return;

        bool isCategory = isNodeCategory[currentCategoryIndex];
        int linkedIndex = indexLinkedCategory[currentCategoryIndex];

        // Se premiamo invio su una Categoria (Tendina)
        if (isCategory)
        {
            // Modifichiamo il database nel Manager
            menuManager.categoryLists[linkedIndex].isOpen = !menuManager.categoryLists[linkedIndex].isOpen;
            bool isOpenNow = menuManager.categoryLists[linkedIndex].isOpen;
            
            // Accendiamo/Spegniamo il GameObject della lista
            menuManager.categoryLists[linkedIndex].categoryList.SetActive(isOpenNow);

            // Cambiamo i simboli visivi [+] e [-]
            if (isOpenNow) menuManager.categoryLists[linkedIndex].categoryTitle.text = menuManager.categoryLists[linkedIndex].categoryTitle.text.Replace("[+]", "[-]");
            else menuManager.categoryLists[linkedIndex].categoryTitle.text = menuManager.categoryLists[linkedIndex].categoryTitle.text.Replace("[-]", "[+]");

            // Forziamo il ricalcolo istantaneo del Layout Group per evitare glitch grafici in Game
            if (contentRectTransform != null) LayoutRebuilder.ForceRebuildLayoutImmediate(contentRectTransform);

            // Ricalcoliamo la lista invisibile di navigazione
            RecalculateNavigationList();
        }
    }
    #endregion
}