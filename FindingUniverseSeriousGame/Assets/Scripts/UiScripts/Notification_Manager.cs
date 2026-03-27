using UnityEngine;
using System.Collections;


public class Notification_Manager : MonoBehaviour
{
    [SerializeField] GameObject prefabTxtNotification;
    [SerializeField] GameObject notifPanel;

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
        Vfx_Typewriter scriptTypeWrite = ShowOnPanel();
        StartCoroutine(DialogueSequence(message, scriptTypeWrite));
    }

    /// <summary>
    /// Mostro notifica aggiornamento codex
    /// </summary>
    /// <param name="argomento"></param>
    public void ShowNotificationCodexUpdate(string argomento)
    {
        Vfx_Typewriter scriptTypeWrite =  ShowOnPanel(); // instanzio il testo
        StartCoroutine(DialogueSequence("Codex aggiornato: " + argomento, scriptTypeWrite));
    }

    private IEnumerator DialogueSequence(string message, Vfx_Typewriter typewriter)  // Sequenza completa per mostrare un messaggio di dialogo
    {
        // 3. Ora che la box è aperta e il testo è attivo, scrivi!
        yield return StartCoroutine(typewriter.TypeText(message));
        
        // 5. Chiudi
        yield return new WaitForSeconds(2f);
        typewriter.ClearText(); // Puliamo il testo dopo la visualizzazione
    }

    /// <summary>
    /// Instazio il prefab del testo per la notifica in content e poi ritorno lo script del typeWriter
    /// </summary>
    /// <returns></returns>
    private Vfx_Typewriter ShowOnPanel()
    {
        // instanzio il testo all'interno del content object in NotifcationCascade
        GameObject newText = Instantiate(prefabTxtNotification, notifPanel.transform);
        Destroy(newText, 3f); //distruggo l'oggetto dopo n secondi
        return newText.GetComponent<Vfx_Typewriter>(); // ritorno lo script per avviare l'animazione della scritura
    }
}
