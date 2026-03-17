using UnityEngine;
using System.Collections;


public class Notification_Manager : MonoBehaviour
{
    [SerializeField] private Vfx_Typewriter typewriter;  // Oggetto con il testo

    private void OnEnable()
    {
        DelegateClass.NotificationEventsHandler += ShowMessage;
    }
    private void OnDisable()
    {
        DelegateClass.NotificationEventsHandler -= ShowMessage;
    }

    /// <summary>
    /// Mostra un messaggio di notifica generale
    /// </summary>
    /// <param name="message"></param>
    public void ShowMessage(string message)
    {
        typewriter.ClearText(); // Puliamo il testo prima di iniziare

        StopAllCoroutines();
        StartCoroutine(DialogueSequence(message));
    }

    /// <summary>
    /// Mostro notifica aggiornamento codex
    /// </summary>
    /// <param name="argomento"></param>
    public void ShowNotificationCodexUpdate(string argomento)
    {
        typewriter.ClearText(); // Puliamo il testo prima di iniziare

        StopAllCoroutines();
        StartCoroutine(DialogueSequence("Codex aggiornato: " + argomento));
    }

    private IEnumerator DialogueSequence(string message)  // Sequenza completa per mostrare un messaggio di dialogo
    {
        // 3. Ora che la box è aperta e il testo è attivo, scrivi!
        yield return StartCoroutine(typewriter.TypeText(message));
        
        // 5. Chiudi
        yield return new WaitForSeconds(2f);
        typewriter.ClearText(); // Puliamo il testo dopo la visualizzazione
    }
}
