using UnityEngine;
using System.Collections;


public class Notification_Manager : MonoBehaviour
{
    //[SerializeField] private Vfx_Typewriter typewriter;  // Oggetto con il testo
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowMessage("pippo"); 
        }
    }

    /// <summary>
    /// Mostra un messaggio di notifica generale
    /// </summary>
    /// <param name="message"></param>
    public void ShowMessage(string message)
    {
        //typewriter.ClearText(); // Puliamo il testo prima di iniziare

        //StopAllCoroutines();
        Vfx_Typewriter scriptTypeWrite = ShowOnPanel();
        StartCoroutine(DialogueSequence(message, scriptTypeWrite));
    }

    /// <summary>
    /// Mostro notifica aggiornamento codex
    /// </summary>
    /// <param name="argomento"></param>
    public void ShowNotificationCodexUpdate(string argomento)
    {
        //typewriter.ClearText(); // Puliamo il testo prima di iniziare

        StopAllCoroutines();
        Vfx_Typewriter scriptTypeWrite =  ShowOnPanel();
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

    private Vfx_Typewriter ShowOnPanel()
    {
        GameObject newText = Instantiate(prefabTxtNotification, notifPanel.transform);
        Destroy(newText, 3f);
        return newText.GetComponent<Vfx_Typewriter>();
    }
}
