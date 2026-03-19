using UnityEngine;

public abstract class Quest_Generic_Script : MonoBehaviour
{
    public string questName;
    public bool questStarted = false;
    public bool questCompleted = false;

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
    public virtual void FinishQuest()
    {
        if (questStarted && !questCompleted)
        {
            questCompleted = true;
            Debug.Log("Quest Completata!");
        }
    }
}
