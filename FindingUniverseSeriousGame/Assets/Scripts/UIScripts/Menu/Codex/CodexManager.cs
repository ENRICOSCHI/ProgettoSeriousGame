using UnityEngine;

public class CodexManager : MonoBehaviour
{
    #region Inizializzazione variabili
    [Header("Struttura Codex (Il Database)")]
    [Tooltip("Configura qui le macro-categorie (Pianeti, Fenomeni, Musica).")]
    public CategoryCodex[] categoryLists;  

    private CodexNavigation codexNavigation; 
    #endregion

    #region Metodi Unity (Ciclo di Vita)

    // NUOVA FUNZIONE: Controlla di avere il navigatore, a prescindere da chi si sveglia prima
    private void EnsureReferences()
    {
        if (codexNavigation == null) 
            codexNavigation = GetComponent<CodexNavigation>();
    }

    void Awake()
    {
        EnsureReferences();

        // Chiudiamo le cartelle fisicamente all'avvio
        foreach (var category in categoryLists)
        {
            if (category.categoryList != null) category.categoryList.SetActive(false);
            category.isOpen = false;
        }
    }

    void OnEnable()
    {
        EnsureReferences(); // Ci assicuriamo di averlo prima di dargli ordini!

        if (codexNavigation != null)
        {
            codexNavigation.RefreshFullUI();
        }
    }

    #endregion

    #region Input Giocatore
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            codexNavigation.MoveCursor(1);
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            codexNavigation.MoveCursor(-1);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            codexNavigation.ConfirmSelection();
    }
    #endregion

    #region Metodi Pubblici
    public void UnlockCodexEntry(int categoryIndex, int entryIndex)
    {
        if (categoryIndex >= 0 && categoryIndex < categoryLists.Length)
        {
            if (entryIndex >= 0 && entryIndex < categoryLists[categoryIndex].entries.Length)
            {
                categoryLists[categoryIndex].entries[entryIndex].isDiscovered = true;
                
                EnsureReferences();
                if (codexNavigation != null) codexNavigation.RefreshFullUI();
                
                Debug.Log($"Codex Aggiornato: Sbloccato {categoryLists[categoryIndex].entries[entryIndex].realName}!");
            }
        }
    }
    #endregion
}