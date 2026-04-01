using UnityEngine;

public class CodexManager : MonoBehaviour
{
    #region Inizializzazione variabili

    [Header("Struttura Codex (Il Database)")]
    [Tooltip("Configura qui le macro-categorie (Pianeti, Fenomeni, Musica).")]
    // IMPORTANTE: Deve essere 'public' affinché CodexNavigation possa leggerlo per aggiornare la grafica!
    public CategoryCodex[] categoryLists;  // Array principale che funge da database per tutti i dati del Codex

    // Riferimenti agli altri script
    private CodexNavigation codexNavigation; // Riferimento al "Motore Grafico" che si occuperà di muovere il cursore e mostrare i testi

    #endregion



    #region Metodi Unity (Ciclo di Vita)

    void Awake()
    {
        // Recupero automatico dello script di navigazione attaccato allo stesso GameObject
        codexNavigation = GetComponent<CodexNavigation>();
        
        // Controllo di sicurezza
        if (codexNavigation == null) 
        {
            Debug.LogError("CodexNavigation non trovato su " + gameObject.name + "! Assicurati di aver attaccato lo script.");
        }
    }

    void Start()
    {
        // All'avvio, ci assicuriamo che tutte le tendine fisiche delle categorie siano chiuse per avere un menu pulito e compatto fin dalla prima apertura.
        foreach (var category in categoryLists)
        {
            if (category.categoryList != null) 
            {
                category.categoryList.SetActive(false);
            }
            category.isOpen = false;
        }
    }

    /// <summary>
    /// Chiamato ad ogni apertura del menu per aggiornare la grafica con eventuali nuovi dati sbloccati. 
    /// </summary>
    void OnEnable()
    {
        // Quando il menu si apre, ordiniamo al navigatore di aggiornare tutta la grafica
        // per assicurarci che eventuali dati sbloccati di recente vengano mostrati subito.
        if (codexNavigation != null)
        {
            codexNavigation.RefreshFullUI();
        }
    }

    #endregion



    #region Input Giocatore

    void Update()
    {
        // Questo script legge gli input, ma fa eseguire le azioni grafiche al CodexNavigation

        // Scorrimento SU e GIÙ
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            codexNavigation.MoveCursor(1);
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            codexNavigation.MoveCursor(-1);
        }

        // Conferma Selezione (Apertura/Chiusura Tendine)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            codexNavigation.ConfirmSelection();
        }
    }

    #endregion



    #region Metodi Pubblici (Da chiamare nel gioco)
    
    /// <summary>
    /// Sblocca una voce specifica nel database del Codex e aggiorna immediatamente la UI.
    /// Da chiamare da script esterni (es. collisione con un pianeta, scansione completata).  (es. Supponiamo che Pianeti sia la Categoria 0, e Mercurio sia l'Entry 0:   FindObjectOfType<CodexManager>().UnlockCodexEntry(0, 0);)
    /// </summary>
    /// <param name="categoryIndex">L'indice della macro-categoria (es. 0 per Pianeti)</param>
    /// <param name="entryIndex">L'indice della singola voce all'interno della categoria (es. 0 per Mercurio)</param>
    public void UnlockCodexEntry(int categoryIndex, int entryIndex)
    {
        // Controlli di sicurezza per evitare errori "Index Out Of Range" se si passano numeri sbagliati
        if (categoryIndex >= 0 && categoryIndex < categoryLists.Length)
        {
            if (entryIndex >= 0 && entryIndex < categoryLists[categoryIndex].entries.Length)
            {
                // 1. Aggiorniamo il database interno segnando il dato come scoperto
                categoryLists[categoryIndex].entries[entryIndex].isDiscovered = true;
                
                // 2. Ordiniamo al navigatore visivo di aggiornare la grafica all'istante
                if (codexNavigation != null) 
                {
                    codexNavigation.RefreshFullUI();
                }
                
                // Messaggio di log per verificare che lo sblocco sia andato a buon fine
                Debug.Log($"Codex Aggiornato: Sbloccato {categoryLists[categoryIndex].entries[entryIndex].realName}!");
            }
        }
    }
    
    #endregion
}