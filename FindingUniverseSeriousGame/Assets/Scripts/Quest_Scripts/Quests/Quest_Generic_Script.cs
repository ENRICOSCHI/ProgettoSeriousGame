using UnityEngine;

/// <summary>
/// Enum rappresentativa dell'interagibilità
/// </summary>
public enum interactableType { scan, keyWord }

public abstract class Quest_Generic_Script : MonoBehaviour
{
    public string questName;
    [Tooltip("Indica la categoria del codex (Es. Pianeti, Eventi...)")]
    public int indiceCategory = -1;
    [Tooltip("Indica la voce specifica (Es. Mercurio, Venere...)")]
    public int indiceEntry = -1;
    [Tooltip("Colore della notifica per la quest")]
    public Color notificationColor;
    
    [HideInInspector]
    public bool questStarted = false;  // Quest iniziata
    [HideInInspector]
    public bool questCompleted = false;  // Quest terminata

    [Tooltip("indica la tipologia di interagibilità della quest")]
    public interactableType questInteractionType;



    #region Inizializzazione QuestManager

    public virtual void Start()
    {
        // Inizializza lo stato della quest dal QuestManager se esiste.
        // altrimenti lo crea con i valori di default (non iniziata, non completata)
        if (string.IsNullOrEmpty(questName))
        {
            Debug.LogWarning("Non è stato assegnato per la quest in: " + gameObject.name);
            return;
        }
        if (QuestManager_Script.instance != null)  // Se l'istanza della quest esiste, assegna i parametri
        {
            QuestData data = QuestManager_Script.instance.GetQuestData(questName);
            questStarted = data.isStarted;
            questCompleted = data.isCompleted;
        }
        else Debug.LogWarning("QuestManager_Script non trovato in scena.");
    }

    #endregion



    
    #region Metodi generali di inizio e fine quest

    /// <summary>
    /// Inizia la quest se e solo se non risulta iniziata.
    /// </summary>
    public virtual void StartQuest()
    {
        if (!questStarted)
        {
            questStarted = true;
            //WIP: possibile aggiunta di messaggio alla UI
            Debug.Log("Quest Cominciata!");
            //QuestManager_Script.instance.UpdateQuestData(questName, questStarted, questCompleted);
        }
    }

    /// <summary>
    /// Termina la quest se e solo se risulta iniziata e non completata.
    /// </summary>
    public virtual void FinishQuest()
    {
        if (questStarted && !questCompleted)
        {
            questCompleted = true;
            Debug.Log("Quest Completata!");
            //QuestManager_Script.instance.UpdateQuestData(questName, questStarted, questCompleted);
        }
    }

    #endregion
}
