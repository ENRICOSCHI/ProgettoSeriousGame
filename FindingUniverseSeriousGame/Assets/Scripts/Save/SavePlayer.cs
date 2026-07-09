using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;

public class SavePlayer : MonoBehaviour, IHandleJSON
{
    [Header("Variabili del giocatore da salvare")]
    [SerializeField] private Transform playerPosition;
    private string KEYPLAYER = "PlayerData";

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

    /*private void Start()
    {
        Load(PersistentSceneData.Instance.isChangingScene);
    }*/

    #region Implementazione IHandleJSON
    public Dictionary<string, PlayerData> playerDataDictionary = new();

    public void SaveGame<TKey, TValue>(Dictionary<TKey, TValue> data)
    {
        /*Siccome uso generici devo indicare quali sono i valori per TKey e TValue.
          Per farlo uso i cast.
        */
        
        TKey key = (TKey)(object)KEYPLAYER; // Cast della chiave da string a TKey

        //se non sono stati creati dati per il player, li creo...
        if (!data.ContainsKey(key))
        {
            data.Add(key, (TValue)(object)new PlayerData //converto PlayerData in TValue
            {
                //position
                positionPlayerX = playerPosition.position.x,
                positionPlayerY = playerPosition.position.y,
                positionPlayerZ = playerPosition.position.z,

                //rotation
                rotationPlayerX = playerPosition.eulerAngles.x,
                rotationPlayerY = playerPosition.eulerAngles.y,
                rotationPlayerZ = playerPosition.eulerAngles.z,

                //battery
                battery = ManagerHandler.ManagerInstance.BatteryManager.GetCurrentBattery(),
                //life
                life = ManagerHandler.ManagerInstance.LifeManager.GetCurrentLife()
            });
        }
        else //... altrimenti li sovrascrivo
        {
            PlayerData playerData = (PlayerData)(object)data[key]; // Converto TValue in PlayerData
            //position
            playerData.positionPlayerX = playerPosition.position.x;
            playerData.positionPlayerY = playerPosition.position.y;
            playerData.positionPlayerZ = playerPosition.position.z;
            //rotation
            playerData.rotationPlayerX = playerPosition.eulerAngles.x;
            playerData.rotationPlayerY = playerPosition.eulerAngles.y;
            playerData.rotationPlayerZ = playerPosition.eulerAngles.z;

            //battery
            playerData.battery = ManagerHandler.ManagerInstance.BatteryManager.GetCurrentBattery();
            //life
            playerData.life = ManagerHandler.ManagerInstance.LifeManager.GetCurrentLife();
        }


        string path = ManagerHandler.ManagerInstance.SaveManager.GetPathForPlayer();

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);

        try
        {
            File.WriteAllText(path, json);
            Debug.Log("salvato in: " + path);
        }
        catch(Exception e)
        {
            Debug.Log("Errore nel salvataggio del File Json: \n" + e.Message);
        }
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
            string json;

            try
            {
                json = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                Debug.Log("Errore nella lettura del File Json: \n" + e.Message);
                return new Dictionary<TKey, TValue>();
            }

            return JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(json);
        }
        else return new Dictionary<TKey, TValue>();

    }

    public void Load(bool isChangingLevel)
    {
        #region Fetch Dati dal JSON

        if (isChangingLevel) return;
        
        if (!CheckJsonFile()) return;

        playerDataDictionary = LoadJson<string, PlayerData>();
        #endregion

        #region Assegnazione Dati al Giocatore 
        if (playerDataDictionary.ContainsKey(KEYPLAYER))
        {
            playerPosition.position = new Vector3(playerDataDictionary[KEYPLAYER].positionPlayerX, playerDataDictionary[KEYPLAYER].positionPlayerY, playerDataDictionary[KEYPLAYER].positionPlayerZ);

            MovimentoNavicella MV = playerPosition.GetComponent<MovimentoNavicella>();
            MV.SetRotationFromSave(playerDataDictionary[KEYPLAYER].rotationPlayerX, playerDataDictionary[KEYPLAYER].rotationPlayerY, playerDataDictionary[KEYPLAYER].rotationPlayerZ);

            ManagerHandler.ManagerInstance.BatteryManager.SetCurrentBattery(playerDataDictionary[KEYPLAYER].battery);
            ManagerHandler.ManagerInstance.LifeManager.SetCurrentLife(playerDataDictionary[KEYPLAYER].life);
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
