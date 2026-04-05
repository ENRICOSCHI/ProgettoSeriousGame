using UnityEngine;

/// <summary>
/// Classe globale per gestire l'invio di messaggi UI da qualsiasi punto del gioco
/// </summary>
public static class Communications
{
    public static void Dialogue(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            DelegateClass.DialogueBoxEventsHandler?.Invoke(message);
        }
    }

    public static void Notify(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            DelegateClass.NotificationEventsHandler?.Invoke(message);
        }
    }
}