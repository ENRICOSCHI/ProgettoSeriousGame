using UnityEngine;

public class Quest_2_Script : Quest_Generic_Script
{
    // Quest di scoperta di un oggetto
    public override void FinishQuest()
    {
        questStarted = true;  // Per superare i controlli di FinishQuest()

        base.FinishQuest();

        //Aggiunta di un messaggio alla UI

        ManagerHandler.ManagerInstance.NotificationManager.ShowMessage("Hai completato la quest: " + questName);  // Notifica di completamento quest
    }
}
