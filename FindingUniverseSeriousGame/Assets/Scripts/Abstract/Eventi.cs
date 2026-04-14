using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Nello script Communications ci sono funzioni statiche per inviare messaggi alla UI
// Ho lasciato questa perchè magari ci possiamo fare qualcosa per gli eventi
// Ma per mandare dialoghi o notifiche è meglio usare direttamente Communications.Dialogue() o Communications.Notify() così non siamo vincolati a un evento specifico
public abstract class Eventi : MonoBehaviour
{
    [Header("Gestione sottotitoli")]
    [SerializeField] AudioClip audioSubtitle;
    [SerializeField] Subtitles[] subtitleDescriptionEvent;
    [Header("Gestione notifiche")]
    public string[] notificaMessaggio;
    [SerializeField] Color notificationColor;

    /// <summary>
    /// Attivo i sottotitoli per questo evento
    /// </summary>
    protected void ActiveSubtitlesWithAudio()
    {
        StartCoroutine(ManagerHandler.ManagerInstance.SubtitleManager.PlaySubtitle(subtitleDescriptionEvent,audioSubtitle));
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
