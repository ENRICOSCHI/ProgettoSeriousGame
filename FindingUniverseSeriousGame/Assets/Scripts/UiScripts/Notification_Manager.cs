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

    public void ShowMessage(string message)  // Funzione pubblica per mostrare un messaggio di dialogo
    {
        typewriter.ClearText(); // Puliamo il testo prima di iniziare

        StopAllCoroutines();
        StartCoroutine(DialogueSequence(message));
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
