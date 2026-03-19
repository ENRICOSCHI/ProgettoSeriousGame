using UnityEngine;

public abstract class Eventi : MonoBehaviour
{
    [TextArea(3, 10)] [SerializeField] string descrizione;
    [SerializeField] string notificaMessaggio;

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
        DelegateClass.NotificationEventsHandler?.Invoke(notificaMessaggio);
    }

    /// <summary>
    /// Invio una notifica personalizzata, utile per eventi che hanno più stati o messaggi dinamici.
    /// </summary>
    /// <param name="messaggio"></param>
    protected void NotificaPersonalizzata(string messaggio)
    {
        DelegateClass.NotificationEventsHandler?.Invoke(messaggio);
    }
}
