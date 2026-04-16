using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
/// <summary>
/// Quest relativa alla scoperta di un Oggetto
/// </summary>
public class Quest_2_Script : Quest_Generic_Script
{
    void OnValidate()
    {
        BoxCollider bc = GetComponent<BoxCollider>();
        if(bc != null)
            bc.isTrigger = true;
    }
    
    /// <summary>
    /// Override del metodo FinishQuest() base adattato per Quest di tipo 2.
    /// Questo FinishQuest() non ha bisogno che la quest 
    /// sia stata precedentemente avviata
    /// </summary>
    /// <remarks>
    /// Prima di chiamare FinishQuest() base, questStarted è messo a "true",
    /// bypassando i relativi controlli di sicurezza.
    /// </remarks>
    public override void FinishQuest()
    {
        if (QuestManager_Script.instance.GetQuestDataDictionary().ContainsKey(idCodex))
        {
            return;
        }

        questStarted = true;  

        base.FinishQuest();

        //Aggiunta di un messaggio alla UI

        ManagerHandler.ManagerInstance.NotificationManager.ShowNotifcation("Hai completato la quest: " + questName,notificationColor);  // Notifica di completamento quest

        this.enabled = false;

        ManagerHandler.ManagerInstance.CodexManager.UnlockMenuEntry(indiceCategory, indiceEntry);  // Sblocca la voce del codex associata alla quest
        ManagerHandler.ManagerInstance.NotificationManager.ShowNotificationCodexUpdate(questName);
        ManagerHandler.ManagerInstance.MissionManager.UnlockMenuEntry(indiceCategory, indiceEntry, questStarted, questCompleted); // aggiorno UI nel menu
    }
}
