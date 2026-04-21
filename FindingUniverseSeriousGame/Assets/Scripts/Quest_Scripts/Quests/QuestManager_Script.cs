using System.Collections.Generic;
using UnityEngine;

public class QuestManager_Script : MonoBehaviour 
{
    /*Questo script serve a gestire il salvataggio degli stati delle missioni
    anche a fronte di un cambio di scena, va collegato al GameObject QuestManager_Global
    che è creato nella scena in cui il player comincia la partita*/

    #region Setups
    public static QuestManager_Script instance;

    // Dizionario che associa il nome della missione al suo stato (questData)
    private Dictionary<string, QuestData> questDatabase = new();

    
    #endregion

    #region Initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    
    #region Quest Data Management
    public void UpdateQuestData(string questName, int amountProgress, string nomeOggettoSbloccato)
    {
        if (questDatabase.ContainsKey(questName))
        {
            questDatabase[questName].amountProgress = amountProgress;
            questDatabase[questName].oggettiSbloccati.Add(nomeOggettoSbloccato);
        }
    }

    public void UpdateQuestData(string questName, bool isStarted, bool isCompleted)
    {
        if (questDatabase.ContainsKey(questName))
        {
            questDatabase[questName].isStarted = isStarted;
            questDatabase[questName].isCompleted = isCompleted;
        }
        else
        {
            questDatabase.Add(questName, new QuestData(isStarted,isCompleted));
        }
    }
    #endregion

    #region Quest Data Retrieval

    /// <summary>
    /// Ottengo la quantità dai salvataggi
    /// </summary>
    /// <param name="IDquest"></param>
    /// <returns></returns>
    public int GetQuestAmount(string IDquest)
    {
        if (questDatabase.ContainsKey(IDquest))
            return questDatabase[IDquest].amountProgress;

        return 0;
    }

    /// <summary>
    /// Controllo se ho già sbloccato l'oggetto in un salvataggio precedente
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    public bool CheckObjectAlreadyUnlocked(string idQuest, string objectName)
    {
        if (questDatabase.ContainsKey(idQuest))
        {
            foreach(string s in questDatabase[idQuest].oggettiSbloccati)
            {
                if (s == objectName) return true;
            }
        }
        return false;
    }

    public Dictionary<string,QuestData> GetQuestDataDictionary()
    {
        return questDatabase;
    }

    public void SetQuestDataDictionary(Dictionary<string,QuestData> data)
    {

        foreach (var item in data)
        {
            if (questDatabase.ContainsKey(item.Key))
            {
                questDatabase[item.Key].isStarted = item.Value.isStarted;
                questDatabase[item.Key].isCompleted = item.Value.isCompleted;
                questDatabase[item.Key].amountProgress = item.Value.amountProgress;
                questDatabase[item.Key].oggettiSbloccati = item.Value.oggettiSbloccati;
            }
            else
            {
                questDatabase.Add(item.Key, item.Value);
            }
        }
    }
    #endregion
}
