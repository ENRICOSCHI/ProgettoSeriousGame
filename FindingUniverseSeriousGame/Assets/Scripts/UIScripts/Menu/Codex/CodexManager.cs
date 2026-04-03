using UnityEngine;

public class CodexManager : MonoBehaviour
{
    #region Inizializzazione variabili
    [Header("Struttura Codex (Il Database)")]
    [Tooltip("Configura qui le macro-categorie (Pianeti, Fenomeni, Musica).")]
    public CategoryCodex[] categoryLists;  
    #endregion

    #region Metodi Unity (Ciclo di Vita)
    void Awake()
    {

        // Chiudiamo le cartelle fisicamente all'avvio
        foreach (var category in categoryLists)
        {
            if (category.categoryList != null) category.categoryList.SetActive(false);
            category.isOpen = false;
        }
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

                Debug.Log($"Codex Aggiornato: Sbloccato {categoryLists[categoryIndex].entries[entryIndex].realName}!");
            }
        }
    }
    #endregion
}