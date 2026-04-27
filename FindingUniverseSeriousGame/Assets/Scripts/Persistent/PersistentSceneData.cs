using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System;

public class PersistentSceneData : MonoBehaviour, IHandleJSON
{
    public static PersistentSceneData Instance { get; private set; }

    //Flag per controllare il cambio di scena
    [HideInInspector] public bool isChangingScene = false;

#region "variabili salvabili nei file json"
    private string KEYPERSISTENTSCENEDATA = "PersistentSceneData";
    Dictionary<string, FlagSavableData> persistentSceneDataDictionary = new();
    // Flag per evitare di ripetere la descrizione ogni volta che si entra negli eventi
    [HideInInspector] public bool isDescriptionUmbraHappened = false;
    [HideInInspector] public bool isDescriptionFiondaHappened = false;
    [HideInInspector] public bool isDescriptionVentoSolareHappened = false;
    [HideInInspector] public bool isChangeSceneUnlocked = false;
#endregion

#region "Unity Methods"
    void Awake()
    {
        if(Instance != null) return;

        Instance = this;

        Load(false); //carico i dati (se presenti) della partita precedente

        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        DelegateClass.SaveEventHandler += Save;
    }

    void OnDisable()
    {
        DelegateClass.SaveEventHandler -= Save;
    }
#endregion

#region "Implementazione IHandleJSON"

    public void Load(bool isChangingLevel)
    {
        #region Fetch Dati dal JSON
        if (!CheckJsonFile()) return;

        persistentSceneDataDictionary = LoadJson<string, FlagSavableData>();
        #endregion

        #region Assegnazione Dati al Giocatore 
        if (persistentSceneDataDictionary.ContainsKey(KEYPERSISTENTSCENEDATA))
        {
            isDescriptionFiondaHappened = persistentSceneDataDictionary[KEYPERSISTENTSCENEDATA].DescriptionFiondaHappened;
            isDescriptionUmbraHappened = persistentSceneDataDictionary[KEYPERSISTENTSCENEDATA].DescriptionUmbraHappened;
            isDescriptionVentoSolareHappened = persistentSceneDataDictionary[KEYPERSISTENTSCENEDATA].DescriptionVentoSolareHappened; 
            Debug.Log("PersistentSceneData caricato");
        }
        else
        {
            Debug.LogWarning("PersistentSceneData non trovato nel JSON. Assicurati che il file non sia corrotto e che contenga i dati corretti.");
        }
        #endregion
    }

    public bool CheckJsonFile()
    {
        return File.Exists(ManagerHandler.ManagerInstance.SaveManager.GetPathForPersistentSceneData());
    }

    public Dictionary<TKey, TValue> LoadJson<TKey, TValue>()
    {
        string path = ManagerHandler.ManagerInstance.SaveManager.GetPathForPersistentSceneData();

        if (path != null)
        {
            string json = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(json);
        }
        else return new Dictionary<TKey, TValue>();

    }

    public void Save(bool isChangingLevel)
    {
        SaveGame<string, FlagSavableData>(persistentSceneDataDictionary);
    }


    public void SaveGame<TKey, TValue>(Dictionary<TKey, TValue> data)
    {
        if (!persistentSceneDataDictionary.ContainsKey(KEYPERSISTENTSCENEDATA))
        {
            persistentSceneDataDictionary.Add(KEYPERSISTENTSCENEDATA, new FlagSavableData
            {
                DescriptionFiondaHappened = isDescriptionFiondaHappened,
                DescriptionUmbraHappened = isDescriptionUmbraHappened,
                DescriptionVentoSolareHappened = isDescriptionVentoSolareHappened,
                ChangeSceneUnlcoked = isChangeSceneUnlocked

            });
        }
        else
        {
            persistentSceneDataDictionary[KEYPERSISTENTSCENEDATA].DescriptionFiondaHappened = isDescriptionFiondaHappened;
            persistentSceneDataDictionary[KEYPERSISTENTSCENEDATA].DescriptionUmbraHappened = isDescriptionUmbraHappened;
            persistentSceneDataDictionary[KEYPERSISTENTSCENEDATA].DescriptionVentoSolareHappened = isDescriptionVentoSolareHappened;
            persistentSceneDataDictionary[KEYPERSISTENTSCENEDATA].ChangeSceneUnlcoked = isChangeSceneUnlocked;
        }

        string path = ManagerHandler.ManagerInstance.SaveManager.GetPathForPersistentSceneData();

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);

        File.WriteAllText(path, json);

        Debug.Log("salvato in: " + path);
    }
#endregion
}

[System.Serializable]
public class FlagSavableData
{
    public bool DescriptionUmbraHappened;
    public bool DescriptionFiondaHappened;
    public bool DescriptionVentoSolareHappened;
    public bool ChangeSceneUnlcoked;
}
