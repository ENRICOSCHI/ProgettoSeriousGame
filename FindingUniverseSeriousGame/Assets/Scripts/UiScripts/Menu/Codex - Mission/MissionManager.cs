using UnityEngine;

public class MissionManager : MonoBehaviour
{
    #region Inizializzazione variabili
    [Header("Struttura Codex (Il Database)")]
    [Tooltip("Configura qui le macro-categorie (Pianeti, Fenomeni, Musica).")]
    public CategoryMission[] categoryLists;

    public bool isMission; // Variabile per distinguere se è il menu del Codex o delle Missioni
    #endregion


    #region Metodi Unity (Ciclo di Vita)

    /*void OnEnable()
    {
        // Sottoscrivo il metodo SaveGame all'evento di salvataggio
        DelegateClass.SaveEventHandler += Save;
    }

    void OnDesable()
    {
        // Rimuovo la sottoscrizione quando l'oggetto viene disabilitato
        DelegateClass.SaveEventHandler -= Save;
    }*/


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
    /// Sblocco l'oggetto indicato nel codex
    /// </summary>
    /// <param name="categoryIndex"> indice della categoria 0: pianeti, 1: eventi, 2: musica</param>
    /// <param name="entryIndex"> indice della entrata 0: Mercurio, 1: Venere,ecc..</param>
    public void UnlockMenuEntry(int categoryIndex, int entryIndex, bool isStarted, bool isCompleted)
    {
        if (categoryIndex >= 0 && categoryIndex < categoryLists.Length)
        {
            if (entryIndex >= 0 && entryIndex < categoryLists[categoryIndex].entries.Length)
            {
                categoryLists[categoryIndex].entries[entryIndex].isDiscovered = true;
                categoryLists[categoryIndex].entries[entryIndex].isStarted = isStarted;
                categoryLists[categoryIndex].entries[entryIndex].isCompleted = isCompleted;
                

                Debug.Log($"Menu Aggiornato: Sbloccato {categoryLists[categoryIndex].entries[entryIndex].realName}!");


                /*
                 Per il salvataggio si potrebbe fare una roba tipo: creo lista con ID di quello che ho sbloccato
                 poi al caricamento dei dati controllo gli ID che ho salvato e rimetto quelle parti attive nel codex con isDiscovered = true
                 */
            }
        }
    }

    public void UpdateAmountOnMenu(int categoryIndex, int entryIndex, int amount)
    {
        if (categoryIndex >= 0 && categoryIndex < categoryLists.Length)
        {
            if (entryIndex >= 0 && entryIndex < categoryLists[categoryIndex].entries.Length)
            {
                categoryLists[categoryIndex].entries[entryIndex].amount += amount;
            }
        }
    }
    #endregion

    /*#region Implementazione Interfaccia IHandleJSON
    public void SaveGame(Dictionary)
    {
        // Implementazione del salvataggio per un singolo elemento
        Debug.Log($"Salvataggio: {id} è {(state ? "sbloccato" : "bloccato")}.");
    }


    //Non necessario
    public void SaveGame(string id, bool isActive, bool isCompleted)
    {
        // Implementazione del salvataggio per un elemento con stato più complesso
        Debug.Log($"Salvataggio: {id} è {(isActive ? "attivo" : "inattivo")} e {(isCompleted ? "completato" : "non completato")}.");
    }

    public void Save()
    {
        //Metodo per far riferimento al path automatico di Unity per il salvataggio dei dati
        Debug.Log("Salvataggio del Codex in corso...");
        Debug.Log($"Percorso di salvataggio: {Application.persistentDataPath}");
    }
    #endregion*/
}
