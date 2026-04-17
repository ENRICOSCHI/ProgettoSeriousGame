using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MissionManager : MonoBehaviour,IHandleJSON
{
    #region Inizializzazione variabili
    [Header("Struttura Missions (Il Database)")]
    [Tooltip("Configura qui le macro-categorie (Pianeti, Fenomeni, Musica).")]
    public CategoryMission[] categoryLists;
    #endregion


    #region Metodi Unity (Ciclo di Vita)

    void OnEnable()
    {
        // Sottoscrivo il metodo SaveGame all'evento di salvataggio
        DelegateClass.SaveEventHandler += Save;
        DelegateClass.LoadEventHandler += Load;
    }

    void OnDisable()
    {
        // Rimuovo la sottoscrizione quando l'oggetto viene disabilitato
        DelegateClass.SaveEventHandler -= Save;
        DelegateClass.LoadEventHandler -= Load;
    }


    void Awake()
    {
        Load();
    }
    #endregion

    #region Metodi Pubblici
    /// <summary>
    /// Sblocco l'oggetto indicato nella sezione Missions
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
                
                QuestManager_Script.instance.UpdateQuestData(
                    categoryLists[categoryIndex].entries[entryIndex].ID,
                    categoryLists[categoryIndex].entries[entryIndex].isStarted = isStarted,
                    categoryLists[categoryIndex].entries[entryIndex].isCompleted = isCompleted
                );

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
                categoryLists[categoryIndex].entries[entryIndex].amount++;
                QuestManager_Script.instance.UpdateQuestData(
                    categoryLists[categoryIndex].entries[entryIndex].ID,
                    categoryLists[categoryIndex].entries[entryIndex].amount
                );
            }
        }
    }
    #endregion

    #region Implementazione Interfaccia IHandleJSON
    public void SaveGame<TKey, TValue>(Dictionary<TKey, TValue> data)
    {
        string path = ManagerHandler.ManagerInstance.SaveManager.GetPathForMission();

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);

        File.WriteAllText(path, json);

        Debug.Log("salvato in: " + path);
    }

    public void Save()
    {
        SaveGame<string, QuestData>(QuestManager_Script.instance.GetQuestDataDictionary());
    }


    public bool CheckJsonFile()
    {
        return File.Exists(ManagerHandler.ManagerInstance.SaveManager.GetPathForMission());
    }

    public Dictionary<TKey, TValue> LoadJson<TKey, TValue>()
    {
        string path = ManagerHandler.ManagerInstance.SaveManager.GetPathForMission();

        if (path != null)
        {
            string json = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(json);
        }
        else return new Dictionary<TKey, TValue>();

    }

    /// <summary>
    /// Il Metodo prova a recuperare i dati dal JSON:
    /// itera, su ogni MissionEntry, di ciascuna CategoryMission (in categoryLists):
    /// un controllo per verificare se l'ID di quella MissionEntry è presente 
    /// tra i dati salvati, se sì, aggiorna lo stato di quella voce.
    /// </summary>
    /// <remarks>
    /// Qualora il JSON non esistesse, il Dictionary è inizializzato
    /// vuoto, la conseguente mancanza di corrispondenza con gli ID farà sì che 
    /// tutte le voci del Codex rimangano nascoste. 
    /// </remarks>
    public void Load()
    {
        #region Fetch Dati dal JSON

        Dictionary<string, QuestData> data = new Dictionary<string, QuestData>();
        if (CheckJsonFile())
        {
            QuestManager_Script.instance.SetQuestDataDictionary(LoadJson<string, QuestData>());
            data = QuestManager_Script.instance.GetQuestDataDictionary();
        }
        #endregion

        #region Caricamento Dati


        foreach (var category in categoryLists)
        {
            if (category.categoryList != null)
            {
                foreach (var entry in category.entries)
                {
                    foreach (var key in data.Keys)
                    {
                        if (data.TryGetValue(entry.ID, out QuestData q))
                        {
                            entry.isStarted = q.isStarted;
                            entry.isCompleted = q.isCompleted;
                            entry.isDiscovered = q.isDiscovered;
                            entry.amount = q.amountProgress;
                        }
                        else
                        {
                            category.categoryList.SetActive(false);
                            category.isOpen = false;
                        }

                    }
                }
            }
        }
        #endregion
        Debug.Log("Mission Manager Caricato");
    }
    #endregion
}
