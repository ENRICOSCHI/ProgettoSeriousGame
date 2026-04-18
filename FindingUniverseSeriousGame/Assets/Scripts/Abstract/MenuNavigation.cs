using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using JetBrains.Annotations;

public abstract class MenuNavigation : MonoBehaviour
{
    #region Inizializzazione variabili

    [Header("Tasti di Navigazione")]
    [Tooltip("Tasti per muovere il cursore su e gi� nella lista")]
    [SerializeField] KeyCode moveUpKey = KeyCode.W;
    [SerializeField] KeyCode moveDownKey = KeyCode.S;
    [SerializeField] KeyCode altMoveUpKey = KeyCode.UpArrow;
    [SerializeField] KeyCode altMoveDownKey = KeyCode.DownArrow;
    [Tooltip("Tasti per confermare la selezione (Aprire/Chiudere le tendine)")]
    [SerializeField] KeyCode confirmKey = KeyCode.Return;
    [SerializeField] KeyCode altConfirmKey = KeyCode.Space;

    [Header("Riferimenti - UI Pannello Destro")]
    [Tooltip("Il testo che mostrer� il titolo del dato selezionato nella parte destra dello schermo")]
    [SerializeField] protected TMP_Text rightPanelTitleText; // Testo del titolo (es. MERCURIO)
    [Tooltip("Il testo che mostrer� la descrizione dettagliata nella parte destra dello schermo")]
    [SerializeField] protected TMP_Text rightPanelDescriptionText; // Testo del corpo della descrizione
    [Tooltip("Stato della missione da mostrare nel pannello destro")]
    [SerializeField] protected TMP_Text rightPanelStatusText1; // Testo precedente allo stato 1
    [SerializeField] protected TMP_Text rightPanelStatus1; // Testo che contiene lo stato 1
    [Tooltip("Testo degli oggetti raccolti da mostrare nel pannello destro")]
    [SerializeField] protected TMP_Text rightPanelStatusText2; // Testo precedente allo stato 2
    [SerializeField] protected TMP_Text rightPanelStatus2; // GameObject che contiene lo stato 2

    [Header("Riferimenti - Logica e Navigazione")]
    [Tooltip("Trascinare qui l'oggetto 'Content' della ScrollView di sinistra. (Serve per il ricalcolo del Layout)")]
    [SerializeField] private RectTransform contentRectTransform;


    // Riferimenti agli altri script essenziali
    protected MenuAsthetics menuAsthetics;
    

    // Liste nascoste per tracciare e muovere il cursore dinamicamente
    protected List<TMP_Text> selectableTexts = new List<TMP_Text>();  // Tutti i testi visibili al momento (Titoli + Voci)
    protected List<bool> isNodeCategory = new List<bool>();  // True = � una macro-categoria. False = � una singola voce.
    protected List<int> indexLinkedCategory = new List<int>();  // ID della macro-categoria a cui appartiene l'elemento
    protected List<int> indexLinkedEntry = new List<int>(); // ID della singola voce (es. Mercurio) all'interno della categoria

    protected int currentCategoryIndex = 0;  // Posizione attuale del cursore nella lista

    


    public Dictionary<string, bool> menuDictionary = new Dictionary<string, bool>();

    #endregion
    
    #region "metodi astratti"
    
    /// <summary>
    /// Controllo se il manger � diverso da null.
    /// </summary>
    /// <returns></returns>
    protected abstract bool IsManagerValid();
    
    /// <summary>
    /// Ottengo numero totale di categorie utilizzate.
    /// </summary>
    /// <returns></returns>
    protected abstract int GetCategoryCount();

    /// <summary>
    /// Ottengo il numero totale di entry utilizzate per quella specifica categoria nell'indice.
    /// </summary>
    /// <returns></returns>
    protected abstract int GetEntryCount(int categoryIndex);

    /// <summary>
    /// Ottengo la la tipologia di menu che usa il navigation.
    /// </summary>
    /// <returns></returns>
    protected abstract CategoryMenu GetMenuCategory(int index);

    /// <summary>
    /// Ottengo il TextMeshProUGUI della categoria e entry nell'indice.
    /// </summary>
    /// <returns></returns>
    protected abstract TMP_Text GetEntryUIText(int categoryIndex, int entryIndex);

    /// <summary>
    /// Ottengo il nome mostrato dell'entry nella category nell'indice.
    /// </summary>
    /// <returns></returns>
    protected abstract string GetEntryDisplayName(int categoryIndex, int entryIndex);
    #endregion


    // FUNZIONE DI SICUREZZA: Controlla di avere i riferimenti a prescindere da chi si sveglia prima
    private void EnsureReferences()
    {
        if (menuAsthetics == null) menuAsthetics = GetComponent<MenuAsthetics>();
        //if (missionManager == null) missionManager = FindAnyObjectByType<CodexManager>(FindObjectsInactive.Include);
    }
    void OnEnable()
    {
        // Se il manager è valido, forziamo la chiusura di tutto prima di aggiornare l'UI
        if (!IsManagerValid())
        {
            ForceCloseAllCategories(); 
        }
        //ogni volta che attivo il pannello aggiorno la UI del codex
        RefreshFullUI();
    }

    protected virtual void Awake()
    {
        // Recupero automatico dei componenti attaccati allo stesso GameObject in totale sicurezza
        EnsureReferences();

        // Controlli di sicurezza per evitare NullReferenceException in gioco
        if (menuAsthetics == null) Debug.LogWarning("MenuAsthetics non trovato in CodexNavigation!");
        
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
    /// Viene chiamato dal Manager per aggiornare l'interfaccia visiva con i dati pi� recenti.
    /// </summary>
    public void RefreshFullUI()
    {
        // Sicurezza: ci assicuriamo di avere i riferimenti prima di agire
        EnsureReferences();

        // Se non c'� un database valido, ci fermiamo
        if (IsManagerValid()) return;

        ClearRightPanel();
        RefreshAllLeftListTexts();
        RecalculateNavigationList();
    }



    #region Funzioni Interne di UI e Database

    /// <summary>
    /// Aggiorna i testi fisici della lista a sinistra. 
    /// Se la voce � sbloccata mostra il nome reale, altrimenti maschera con "???".
    /// </summary>
    private void RefreshAllLeftListTexts()
    {
        int catCount = GetCategoryCount(); //prendo tutte le categorie (esempio pianeti, eventi,ecc..)
        for(int i = 0; i< catCount; i++)
        {
            int entryCount = GetEntryCount(i);// per ogni entrie (quindi se siamo in pianeti prendo terra,marte,ecc..)
            for(int j = 0; j < entryCount; j++)
            {
                var UiText = GetEntryUIText(i, j);// prendo ogni riferimento al text
                if (UiText != null)
                {
                    UiText.text = GetEntryDisplayName(i, j); //assegno il nome
                }
            }
        }
    }

    /// <summary>
    /// Svuota il monitor di destra impostando i testi di default.
    /// </summary>
    protected void ClearRightPanel()
    {
        if (rightPanelTitleText != null) rightPanelTitleText.text = "SELEZIONA UN DATO";
        if (rightPanelDescriptionText != null) rightPanelDescriptionText.text = "";
        rightPanelStatus1.gameObject.SetActive(false);
        rightPanelStatusText1.gameObject.SetActive(false);
        rightPanelStatus2.gameObject.SetActive(false);
        rightPanelStatusText2.gameObject.SetActive(false);
    }

    /// <summary>
    /// Aggiorna il monitor di destra leggendo i dati dal Manager in base a dove si trova il cursore.
    /// </summary>
    protected virtual void UpdateRightPanelDisplay()
    {
        //Debug.Log("Update Right Panel");
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

        for (int i = 0; i < GetCategoryCount(); i++)
        {
            var catMenu = GetMenuCategory(i);
            // 1. Aggiunge il titolo della categoria principale
            selectableTexts.Add(catMenu.categoryTitle);
            isNodeCategory.Add(true);
            indexLinkedCategory.Add(i);
            indexLinkedEntry.Add(-1); // -1 indica che non � una voce specifica

            // 2. Se la categoria � aperta, aggiunge i suoi figli
            if (catMenu.isOpen)
            {
                
                for (int j = 0; j < GetEntryCount(i); j++)
                {
                    selectableTexts.Add(GetEntryUIText(i,j));
                    isNodeCategory.Add(false);
                    indexLinkedCategory.Add(i);
                    indexLinkedEntry.Add(j);
                }
            }
        }

        // Sicurezza: se la lista si � accorciata, riportiamo il cursore nel limite consentito
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
                // Se � una categoria, rimuoviamo solo il cursore
                selectableTexts[i].text = selectableTexts[i].text.Replace("> ", "");
            }
            else
            {
                // Se � una voce, guardiamo se � scoperta e togliamo l'eventuale cursore
                selectableTexts[i].text = GetEntryDisplayName(linkedCat,linkedEnt);
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
            var catMenu = GetMenuCategory(linkedIndex);
            catMenu.isOpen = !catMenu.isOpen;
            bool isOpenNow = catMenu.isOpen;

            // Accendiamo/Spegniamo il GameObject della lista
            catMenu.categoryList.SetActive(isOpenNow);

            // Cambiamo i simboli visivi [+] e [-]
            if (isOpenNow) catMenu.categoryTitle.text = catMenu.categoryTitle.text.Replace("[+]", "[-]");
            else catMenu.categoryTitle.text = catMenu.categoryTitle.text.Replace("[-]", "[+]");

            // Forziamo il ricalcolo istantaneo del Layout Group per evitare glitch grafici in Game
            if (contentRectTransform != null) LayoutRebuilder.ForceRebuildLayoutImmediate(contentRectTransform);

            // Ricalcoliamo la lista invisibile di navigazione
            RecalculateNavigationList();
        }
    }


    /// <summary>
    /// Forza la chiusura logica, visiva e testuale di tutte le categorie.
    /// </summary>
    protected void ForceCloseAllCategories()
    {
        for (int i = 0; i < GetCategoryCount(); i++)
        {
            var catMenu = GetMenuCategory(i);
            
            catMenu.isOpen = false; // Reset logico
            
            if (catMenu.categoryList != null)
                catMenu.categoryList.SetActive(false); // Spegne fisicamente i bottoni

            // Reset testuale: se per caso c'era il meno, lo fa tornare un più
            if (catMenu.categoryTitle != null && catMenu.categoryTitle.text.Contains("[-]"))
                catMenu.categoryTitle.text = catMenu.categoryTitle.text.Replace("[-]", "[+]");
        }
    }
    #endregion
}
