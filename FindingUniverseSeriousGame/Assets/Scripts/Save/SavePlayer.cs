using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;

public class SavePlayer : MonoBehaviour, IHandleJSON
{
    [Header("Variabili del giocatore da salvare")]
    [SerializeField] private Transform playerPosition;

    void OnEnable()
    {
        DelegateClass.SaveEventHandler += Save;
        DelegateClass.LoadEventHandler += Load;
    }

    void OnDisable()
    {
        DelegateClass.SaveEventHandler -= Save;
        DelegateClass.LoadEventHandler -= Load;
    }

    private void Start()
    {
        Load(PersistentSceneData.Instance.isChangingScene);
    }

    #region Implementazione IHandleJSON
    public Dictionary<string, PlayerData> playerDataDictionary = new();

    public void SaveGame<TKey, TValue>(Dictionary<TKey, TValue> data)
    {
        if (!playerDataDictionary.ContainsKey("PlayerData"))
        {
            playerDataDictionary.Add("PlayerData", new PlayerData
            {
                positionPlayerX = playerPosition.position.x,
                positionPlayerY = playerPosition.position.y,
                positionPlayerZ = playerPosition.position.z,

                battery = ManagerHandler.ManagerInstance.BatteryManager.GetCurrentBattery(),
                life = ManagerHandler.ManagerInstance.LifeManager.GetCurrentLife()
            });
        }
        else
        {
            playerDataDictionary["PlayerData"].positionPlayerX = playerPosition.position.x;
            playerDataDictionary["PlayerData"].positionPlayerY = playerPosition.position.y;
            playerDataDictionary["PlayerData"].positionPlayerZ = playerPosition.position.z;
            playerDataDictionary["PlayerData"].battery = ManagerHandler.ManagerInstance.BatteryManager.GetCurrentBattery();
            playerDataDictionary["PlayerData"].life = ManagerHandler.ManagerInstance.LifeManager.GetCurrentLife();
        }


        string path = ManagerHandler.ManagerInstance.SaveManager.GetPathForPlayer();

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);

        File.WriteAllText(path, json);

        Debug.Log("salvato in: " + path);
    }

    public void Save(bool isChangingLevel)
    {
        if (!isChangingLevel)
        {
            SaveGame<string, PlayerData>(playerDataDictionary);
        }
    }

    public bool CheckJsonFile()
    {
        return File.Exists(ManagerHandler.ManagerInstance.SaveManager.GetPathForPlayer());
    }

    public Dictionary<TKey, TValue> LoadJson<TKey, TValue>()
    {
        string path = ManagerHandler.ManagerInstance.SaveManager.GetPathForPlayer();

        if (path != null)
        {
            string json = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(json);
        }
        else return new Dictionary<TKey, TValue>();

    }

    public void Load(bool isChangingLevel)
    {
        #region Fetch Dati dal JSON

        if (isChangingLevel) return;

        Dictionary<string, PlayerData> data = new();
        if (!CheckJsonFile()) return;

        playerDataDictionary = LoadJson<string, PlayerData>();
        #endregion

        #region Assegnazione Dati al Giocatore 
        if (playerDataDictionary.TryGetValue("PlayerData", out PlayerData playerData))
        {
            playerPosition.position = new Vector3(playerData.positionPlayerX, playerData.positionPlayerY, playerData.positionPlayerZ);
            ManagerHandler.ManagerInstance.BatteryManager.SetCurrentBattery(playerData.battery);
            ManagerHandler.ManagerInstance.LifeManager.SetCurrentLife(playerData.life);
            Debug.Log("Player data caricato");
        }
        else
        {
            Debug.LogWarning("PlayerData non trovato nel JSON. Assicurati che il file non sia corrotto e che contenga i dati corretti.");
        }
        #endregion
    }
    #endregion
}
