using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MissionManager : MonoBehaviour, IHandleJSON
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


    /*void Start()
    {
        Load(PersistentSceneData.Instance.isChangingScene);
    }*/
    #endregion

    #region Metodi Pubblici

    /// <summary>
    /// Sblocco l'oggetto indicato nella sezione Missions
    /// </summary>
    /// <param name="categoryIndex"> indice della categoria 0: pianeti, 1: eventi, 2: musica</param>
    /// <param name="entryIndex"> indice della entrata 0: Mercurio, 1: Venere,ecc..</param>
    /// <remarks>
    /// De facto è questo metodo a segnalare a QuestManager_Script di salvare lo stato
    /// di una data quest nel rispettivo Dictionary di salvataggio.
    /// Aggiunta che dunque avviene solo se la missione è stata scoperta.
    /// </remarks>
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

                /*Il seguente evento è pensato per essere catturato da CheckPotenziamenti, 
                al fine di sbloccare potenziamenti legati a quest specifiche.*/
                DelegateClass.UpdateQuestDataEventHandler?.Invoke(categoryLists);

                Debug.Log($"Menu Aggiornato: Sbloccato {categoryLists[categoryIndex].entries[entryIndex].realName}!");
            }
        }
    }

    /// <summary>
    /// Aggiorna quanti e quali oggetti sono stati raccolti in Quest basate
    /// su Quest_3_Script.
    /// </summary>
    /// <remarks>
    /// Il Metodo è pensato per essere chiamato da ItemCollected() di Quest_3_Script, 
    /// al fine di aggiornare la quantità di oggetti raccolti.
    /// Inoltre segnala a QuestManager_Script di salvare lo stato
    /// della Quest di riferimento.
    /// </remarks>
    public void UpdateAmountOnMenu(int categoryIndex, int entryIndex, int amount, string nomeOggettoSbloccato)
    {
        if (categoryIndex >= 0 && categoryIndex < categoryLists.Length)
        {
            if (entryIndex >= 0 && entryIndex < categoryLists[categoryIndex].entries.Length)
            {
                categoryLists[categoryIndex].entries[entryIndex].amount = amount;
                QuestManager_Script.instance.UpdateQuestData(
                    categoryLists[categoryIndex].entries[entryIndex].ID,
                    categoryLists[categoryIndex].entries[entryIndex].amount,
                    nomeOggettoSbloccato
                );

                /*Il seguente evento è pensato per essere catturato da CheckPotenziamenti, 
                al fine di sbloccare potenziamenti legati a quest specifiche.*/
                DelegateClass.UpdateQuestDataEventHandler?.Invoke(categoryLists);
            }
        }
    }
    #endregion

    #region Implementazione Interfaccia IHandleJSON

    /// <summary>
    /// Salva un Dictionary in formato JSON, prendendo chiave e valore di tipo generico.
    /// Il file è salvato nella cartella persistentePath del dispositivo, con nome "missionSave.json".
    /// </summary>
    public void SaveGame<TKey, TValue>(Dictionary<TKey, TValue> data)
    {
        string path = ManagerHandler.ManagerInstance.SaveManager.GetPathForMission();

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);

        try
        {
            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Debug.LogError("Errore durante il salvataggio del file JSON: \n" + e.Message);
            return;
        }
        
        Debug.Log("salvato in: " + path);
    }

    /// <summary>
    /// Chiama il metodo SaveGame() associato, passando il Dictionary contenuto nell'istanza
    /// di QuestManager_Script, contenente tutte le informazioni utili di ogni quest
    /// che sia stata avviata.
    /// </summary>
    /// <remarks>
    /// Passa a SaveGame() un dictionary dello stesso tipo di quello gestito da QuestManager_Script, 
    /// poi SaveGame() avendo firma generica, può accettarlo e serializzarlo in JSON.
    /// </remarks>
    public void Save(bool isChangingLevel)
    {
        SaveGame<string, QuestData>(QuestManager_Script.instance.GetQuestDataDictionary());
    }

    public bool CheckJsonFile()
    {
        return File.Exists(ManagerHandler.ManagerInstance.SaveManager.GetPathForMission());
    }

    /// <summary>
    /// Legge il JSNON salvato e deserializza successivamente i dati in un Dictionary
    /// generico.
    /// </summary>
    /// <returns>
    /// Un Dictionary che è poi declinato in quello del tipo richiesto.
    /// Dovesse non essere recuperato alcun JSON, è ritornato un Dictionary vuoto.
    /// </returns>
    public Dictionary<TKey, TValue> LoadJson<TKey, TValue>()
    {
        string path = ManagerHandler.ManagerInstance.SaveManager.GetPathForMission();

        string json;
        if (path != null)
        {
            try
            {
                json = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                Debug.LogError($"Errore nella lettura del file JSON: {e.Message}");
                return new Dictionary<TKey, TValue>();
            }

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
    public void Load(bool isChangingLevel)
    {
        #region Fetch Dati dal JSON

        Dictionary<string, QuestData> data = new Dictionary<string, QuestData>();
        if (CheckJsonFile())
        {
            QuestManager_Script.instance.SetQuestDataDictionary(LoadJson<string, QuestData>());
            data = QuestManager_Script.instance.GetQuestDataDictionary();
            Debug.Log("Mission Manager Caricato");
        }
        #endregion

        #region Caricamento Dati


        foreach (var category in categoryLists)
        {
            if (category.categoryList != null)
            {
                foreach (var entry in category.entries)
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
        #endregion
        #region "init missioni"
        var missionsListContainer = FindObjectsByType<Quest_3_Script>(FindObjectsSortMode.None); // raccolgo tutte le missioni di tipo 3 presenti nella scene
        //inizializzo tutte le missioni
        foreach (var mission in missionsListContainer) mission.Init();
        #endregion
        PersistentSceneData.Instance.isChangingScene = false;
    }
    #endregion
}
