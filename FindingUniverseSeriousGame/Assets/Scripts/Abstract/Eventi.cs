using UnityEngine;


// Nello script Communications ci sono funzioni statiche per inviare messaggi alla UI
// Ho lasciato questa perchè magari ci possiamo fare qualcosa per gli eventi
// Ma per mandare dialoghi o notifiche è meglio usare direttamente Communications.Dialogue() o Communications.Notify() così non siamo vincolati a un evento specifico
public abstract class Eventi : MonoBehaviour
{
    [TextArea(3, 10)] [SerializeField] string descrizione;
    [SerializeField] string notificaMessaggio;
    [SerializeField] Color notificationColor;

    /// <summary>
    /// Descrizione dell'evento
    /// </summary>
    [ContextMenu("TestaDescrizione")]
    protected void Descrizione()
    {
        DelegateClass.DialogueBoxEventsHandler?.Invoke(descrizione);
    }
    [ContextMenu("TestaNotifica")]
    protected void Notifica()
    {
        DelegateClass.NotificationEventsHandler?.Invoke(notificaMessaggio,notificationColor);
    }

    /// <summary>
    /// Invio una notifica personalizzata, utile per eventi che hanno più stati o messaggi dinamici.
    /// </summary>
    /// <param name="messaggio"></param>
    protected void NotificaPersonalizzata(string messaggio)
    {
        DelegateClass.NotificationEventsHandler?.Invoke(messaggio, notificationColor);
    }
}
