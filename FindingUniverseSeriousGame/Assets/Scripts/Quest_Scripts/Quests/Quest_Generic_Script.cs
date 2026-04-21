using UnityEngine;

/// <summary>
/// Enum rappresentativa dell'interagibilità
/// </summary>
public enum interactableType { scan, keyWord }

public abstract class Quest_Generic_Script : MonoBehaviour
{
    public string questName;
    public string idCodex;
    
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

    /// <summary>
    /// Init caricamento quest
    /// </summary>
    public virtual void Init()
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
            //controllo se esite la chiave nel codice...
            if (QuestManager_Script.instance.GetQuestDataDictionary().ContainsKey(idCodex))
            {
                //aggiorno con i valori caricati dal salvataggio precedente
                questStarted = QuestManager_Script.instance.GetQuestDataDictionary()[idCodex].isStarted;
                questCompleted = QuestManager_Script.instance.GetQuestDataDictionary()[idCodex].isCompleted;
            }
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
        }
    }

    /// <summary>
    /// Termina la quest se e solo se risulta iniziata e non completata.
    /// </summary>
    /// <remarks>
    /// L'aggiunta al Dictionary di QuestManager_Script è stata delegata a QuestManager_Script 
    /// stesso, in modo da centralizzare la gestione dei dati delle quest.
    /// </remarks>
    public virtual void FinishQuest()
    {
        if (questStarted && !questCompleted)
        {
            questCompleted = true;
            Debug.Log("Quest Completata!");
        }
    }

    #endregion
}
