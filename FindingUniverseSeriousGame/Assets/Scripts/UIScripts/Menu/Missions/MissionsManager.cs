using UnityEngine;

public class MissionManager : MonoBehaviour
{
    #region Inizializzazione variabili
    [Header("Struttura Missioni (Il Database)")]
    [Tooltip("Configura qui le macro-categorie (SCoperta, Raccolta, Salvataggio, ...).")]
    public CategoryMission[] categoryLists;  
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
    /// <summary>
    /// Segna la missione indicata come completata
    /// </summary>
    public void CompleteMission(int categoryIndex, int entryIndex)
    {
        if (categoryIndex >= 0 && categoryIndex < categoryLists.Length)
        {
            if (entryIndex >= 0 && entryIndex < categoryLists[categoryIndex].entries.Length)
            {
                // Mettiamo la spunta alla missione
                categoryLists[categoryIndex].entries[entryIndex].isCompleted = true;

                Debug.Log($"Missione Completata: {categoryLists[categoryIndex].entries[entryIndex].missionName}!");
            }
        }
    }
    #endregion
}