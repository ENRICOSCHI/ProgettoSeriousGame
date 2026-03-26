using UnityEngine;

/// <summary>
/// Quest relativa alla scoperta di un Oggetto
/// </summary>
public class Quest_2_Script : Quest_Generic_Script
{
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
        questStarted = true;  

        base.FinishQuest();

        //Aggiunta di un messaggio alla UI

        ManagerHandler.ManagerInstance.NotificationManager.ShowMessage("Hai completato la quest: " + questName);  // Notifica di completamento quest
    }
}
