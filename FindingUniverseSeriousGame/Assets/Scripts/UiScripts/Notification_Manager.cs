using UnityEngine;
using System.Collections;
using TMPro;


public class Notification_Manager : MonoBehaviour
{
    [SerializeField] GameObject prefabTxtNotification;
    [SerializeField] GameObject notifPanel;
    [SerializeField] float tempoNotifica = 2f;
    [SerializeField] Color colorNotificationCodexUpdate;

    private void OnEnable()
    {
        DelegateClass.NotificationEventsHandler += ShowNotifcation;
    }
    private void OnDisable()
    {
        DelegateClass.NotificationEventsHandler -= ShowNotifcation;
    }


    /// <summary>
    /// Mostra un messaggio di notifica generale
    /// </summary>
    /// <param name="message"></param>
    public void ShowNotifcation(string message, Color colorNotification)
    {
        Vfx_Typewriter scriptTypeWrite = ShowOnPanel(colorNotification);
        StartCoroutine(DialogueSequence(message, scriptTypeWrite));
    }

    /// <summary>
    /// Mostro notifica aggiornamento codex
    /// </summary>
    /// <param name="argomento"></param>
    public void ShowNotificationCodexUpdate(string argomento)
    {
        Vfx_Typewriter scriptTypeWrite =  ShowOnPanel(colorNotificationCodexUpdate); // instanzio il testo
        StartCoroutine(DialogueSequence("Codex aggiornato: " + argomento, scriptTypeWrite));
    }

    private IEnumerator DialogueSequence(string message, Vfx_Typewriter typewriter)  // Sequenza completa per mostrare un messaggio di dialogo
    {
        // 3. Ora che la box è aperta e il testo è attivo, scrivi!
        yield return StartCoroutine(typewriter.TypeText(message));
        
        // 5. Chiudi
        yield return new WaitForSeconds(tempoNotifica);
        typewriter.ClearText(); // Puliamo il testo dopo la visualizzazione
    }

    /// <summary>
    /// Instazio il prefab del testo per la notifica in content e poi ritorno lo script del typeWriter
    /// </summary>
    /// <returns></returns>
    private Vfx_Typewriter ShowOnPanel(Color textColor)
    {
        // instanzio il testo all'interno del content object in NotifcationCascade
        GameObject newText = Instantiate(prefabTxtNotification, notifPanel.transform);
        newText.GetComponent<TextMeshProUGUI>().color = textColor; 
        Destroy(newText, tempoNotifica+1); //distruggo l'oggetto dopo n + 1 secondi
        return newText.GetComponent<Vfx_Typewriter>(); // ritorno lo script per avviare l'animazione della scritura
    }
}
