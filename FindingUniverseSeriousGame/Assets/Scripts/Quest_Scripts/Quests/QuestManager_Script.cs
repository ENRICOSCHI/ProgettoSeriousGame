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
            DontDestroyOnLoad(gameObject); //permette di far persistere il gameObject tra scene -> i progressi persistono.
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    
    #region Quest Data Management
    public void UpdateQuestData(string questName, bool isStarted, bool isCompleted, int amountProgress)
    {
        if (questDatabase.ContainsKey(questName))
        {
            questDatabase[questName].isStarted = isStarted;
            questDatabase[questName].isCompleted = isCompleted;
            questDatabase[questName].amountProgress = amountProgress;
        }
        else
        {
            questDatabase.Add(questName, new QuestData(isStarted, isCompleted, amountProgress));
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
            questDatabase.Add(questName, new QuestData(isStarted, isCompleted));
        }
    }
    #endregion

    #region Quest Data Retrieval

    public QuestData GetQuestData(string questName)
    {
        if (questDatabase.ContainsKey(questName))
        {
            return questDatabase[questName];
        }
        else
        {
            Debug.LogWarning("non trovata la Quest " + questName + " nel database.");
            return new QuestData(); /*ritorna una questData con parametri 
            di default (non iniziata, non completata, progresso 0).
            questo può essere utile per evitare null reference exceptions.*/
        }
    }
    #endregion
}
