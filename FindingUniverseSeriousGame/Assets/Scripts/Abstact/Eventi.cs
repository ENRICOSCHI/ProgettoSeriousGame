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
        DelegateClass.DialogueBoxEventsHandler.Invoke(descrizione);
    }
    [ContextMenu("TestaNotifica")]
    protected void Notifica()
    {
        DelegateClass.NotificationEventsHandler.Invoke(notificaMessaggio);
    }
}
