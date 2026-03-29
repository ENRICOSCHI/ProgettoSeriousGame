using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CodexManager : MonoBehaviour
{
    #region Inizializzazione variabili

    [Header("Riferimenti")]
    [Tooltip("Trascinare qui l'oggetto 'Content' della ScrollView di sinistra. \n(Serve per evitare la sovrapposizione delle UI quando si aprono le tendine in gioco)")]
    [SerializeField] private RectTransform contentRectTransform;  // Riferimento al RectTransform del contenuto della ScrollView
    private MenuAsthetics menuAsthetics; // Riferimento a MenuAsthetics per aggiornare l'estetica dei bottoni in base al pannello attivo



    // Creazione di una struttura per mantenere ordine nell'Inspector
    [System.Serializable]
    public class CategoryCodex
    {
        [Tooltip("Inserire il componente TextMeshPro del titolo espandibile. \nEs: L'oggetto di testo '[+] PIANETI'")]
        public TMP_Text categoryTitle;  // Riferimento al testo del titolo della categoria
        
        [Tooltip("Inserire l'oggetto genitore (Empty GameObject) che contiene la lista delle voci di questa categoria. \nEs: L'oggetto 'Lista_Voci_Pianeti' (quello che contiene tutti i '???')")]
        public GameObject categoryList;  // Riferimento al contenitore della categoria (List_Planets...)
        
        [Tooltip("Inserire i singoli componenti TextMeshPro delle voci interne. \nEs: Gli oggetti di testo '???' o i nomi sbloccati all'interno della cartella.")]
        public TMP_Text[] entryTexts;  // Array di testi delle voci della categoria (es. nomi dei pianeti), da assegnare in Inspector
        
        [HideInInspector] public bool isOpen = false;  // Stato della categoria (aperta o chiusa), nascosto in Inspector perché gestito automaticamente
    }


    [Header("Struttura Codex")]
    [Tooltip("Configura qui le macro-categorie (Pianeti, Fenomeni, Musica). \nAggiungi un elemento per ogni macro-categoria e compila i campi interni.")]
    [SerializeField] private CategoryCodex[] categoryLists;  // Array di categorie del codex


    // Liste nascoste per tracciare cosa stiamo scorrendo
    private List<TMP_Text> selectableTexts = new List<TMP_Text>();  // Lista di tutti i testi selezionabili (titoli + voci) per gestire il movimento del cursore
    private List<bool> isNodeCategory = new List<bool>();  // Lista parallela a selectableTexts per indicare se ogni testo è un titolo di categoria (true) o una voce (false)
    private List<int> indexLinkedCategory = new List<int>();  // Lista parallela a selectableTexts per indicare a quale categoria è collegato ogni testo (utile per sapere quale categoria aprire/chiudere quando selezioniamo un titolo)



    private int currentCategoryIndex = 0;  // Indice della categoria attualmente attiva

    #endregion




    void Awake()
    {
        // Ottenimeno in automatico il riferimento allo script estetico attaccato allo stesso oggetto
        menuAsthetics = GetComponent<MenuAsthetics>();
        
        if (menuAsthetics == null)
        {
            Debug.LogWarning("MenuAsthetics non trovato su " + gameObject.name + "!");
        }
    }


    void Start()
    {
        // All'avvio, assicuriamoci che tutte le categorie siano chiuse e nascoste
        foreach (var category in categoryLists)
        {
            if (category.categoryList != null) 
            {
                category.categoryList.SetActive(false);
            }
            category.isOpen = false;
        }
        
        // Calcola la lista per la prima volta
        RecalculateNavigationList();
    }


    void Update()
    {
        // Controlli per muoversi SU e GIÙ
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveCursor(1);
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveCursor(-1);
        }

        // Controllo per confermare la selezione
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            ConfirmSelection();
        }
    }



    #region Funzioni di gestione del menu Codex
    /// <summary>
    /// Ricostruisce la lista di navigazione dall'alto verso il basso, 
    /// includendo solo i titoli e le voci delle categorie attualmente aperte.
    /// </summary>
    private void RecalculateNavigationList()
    {
        // Pulizia delle liste di scorrimento prima di ricostruirle
        selectableTexts.Clear();
        isNodeCategory.Clear();
        indexLinkedCategory.Clear();

        for (int i = 0; i < categoryLists.Length; i++)
        {
            // Aggiunge sempre il titolo della categoria principale
            selectableTexts.Add(categoryLists[i].categoryTitle);
            isNodeCategory.Add(true);
            indexLinkedCategory.Add(i);

            // Se la categoria è aperta, aggiunge anche tutte le sue voci interne alla lista di navigazione
            if (categoryLists[i].isOpen)
            {
                for (int j = 0; j < categoryLists[i].entryTexts.Length; j++)
                {
                    selectableTexts.Add(categoryLists[i].entryTexts[j]);
                    isNodeCategory.Add(false); // False perché è una singola voce, non una categoria
                    indexLinkedCategory.Add(i);
                }
            }
        }

        // Sicurezza: se chiudendo una cartella la lista si accorcia, riportiamo il cursore nei limiti
        if (currentCategoryIndex >= selectableTexts.Count)
        {
            currentCategoryIndex = selectableTexts.Count - 1;
        }

        // Passiamo la nuova lista aggiornata allo script estetico
        if (menuAsthetics != null)
        {
            menuAsthetics.SetTexts(selectableTexts.ToArray());
        }

        UpdateVisualCursor();
    }



    /// <summary>
    /// Muove l'indice di selezione in loop all'interno della lista corrente.
    /// </summary>
    /// <param name="direction">1 per muoversi in basso, -1 per muoversi in alto</param>
    private void MoveCursor(int direction)
    {
        if (selectableTexts.Count == 0) return;

        currentCategoryIndex += direction;

        // Gestione del Loop: se supera il limite va all'inizio, se va sotto lo zero va alla fine
        if (currentCategoryIndex >= selectableTexts.Count) 
        {
            currentCategoryIndex = 0;
        }
        else if (currentCategoryIndex < 0) 
        {
            currentCategoryIndex = selectableTexts.Count - 1;
        }

        UpdateVisualCursor();
    }



    /// <summary>
    /// Aggiorna il simbolo ">" e chiama lo script estetico per i colori.
    /// </summary>
    private void UpdateVisualCursor()
    {
        // Rimuove il simbolo ">" da tutti i testi della lista
        foreach (var textElement in selectableTexts)
        {
            if (textElement != null) 
            {
                textElement.text = textElement.text.Replace("> ", "");
            }
        }

        // Aggiunge il simbolo ">" solo all'elemento attualmente selezionato
        if (currentCategoryIndex >= 0 && currentCategoryIndex < selectableTexts.Count)
        {
            TMP_Text selectedText = selectableTexts[currentCategoryIndex];
            
            if (!selectedText.text.StartsWith(">"))
            {
                selectedText.text = "> " + selectedText.text;
            }
        }

        // Aggiorna i colori tramite lo script separato
        if (menuAsthetics != null)
        {
            menuAsthetics.UpdateTabVisuals(currentCategoryIndex);
        }
    }



    /// <summary>
    /// Esegue un'azione diversa a seconda se abbiamo premuto Invio su una categoria o su una voce.
    /// </summary>
    private void ConfirmSelection()
    {
        if (selectableTexts.Count == 0) return;

        // Recuperiamo le informazioni sull'elemento attualmente selezionato
        bool isCategory = isNodeCategory[currentCategoryIndex];
        int linkedIndex = indexLinkedCategory[currentCategoryIndex];

        if (isCategory)
        {
            // --- LOGICA TENDINA (Categoria) ---
            
            // Invertiamo lo stato (se era aperta si chiude, se era chiusa si apre)
            categoryLists[linkedIndex].isOpen = !categoryLists[linkedIndex].isOpen;
            bool isOpenNow = categoryLists[linkedIndex].isOpen;
            
            // Accendiamo o spegniamo l'oggetto contenitore
            categoryLists[linkedIndex].categoryList.SetActive(isOpenNow);

            // Cambiamo visivamente il simbolo [+] in [-] e viceversa
            if (isOpenNow)
            {
                categoryLists[linkedIndex].categoryTitle.text = categoryLists[linkedIndex].categoryTitle.text.Replace("[+]", "[-]");
            }
            else
            {
                categoryLists[linkedIndex].categoryTitle.text = categoryLists[linkedIndex].categoryTitle.text.Replace("[-]", "[+]");
            }

            // Forza il ricalcolo immediato del Layout per evitare sovrapposizioni grafiche in Game
            if (contentRectTransform != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(contentRectTransform);
            }

            // Ricalcoliamo la lista per aggiungere o rimuovere le voci interne
            RecalculateNavigationList();
        }
        else
        {
            // --- LOGICA VOCE SINGOLA (Pianeta, Fenomeno, Musica) ---
            
            // Qui inseriremo il codice per aggiornare la parte DESTRA dello schermo
            Debug.Log("Selezionata una voce! Indice Categoria Madre: " + linkedIndex + ". Testo: " + selectableTexts[currentCategoryIndex].text);
        }
    }
    #endregion
}