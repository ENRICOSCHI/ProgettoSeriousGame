using UnityEngine;
using System.Collections;
using TMPro;
using Unity.VisualScripting;


public class Notification_Manager : MonoBehaviour
{
    [SerializeField] GameObject prefabTxtNotification;
    [SerializeField] GameObject notifPanel;
    [SerializeField] float tempoNotifica = 2f;
    [SerializeField] float tempoIconaScale = 1f;
    [SerializeField] float tempoIconaFissa = 1f;
    [SerializeField] Color colorNotificationCodexUpdate;

    [Header("Sound Effects")]
    [SerializeField] AudioClip notifica;

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
    /// Mostra un messaggio di notifica generale
    /// </summary>
    /// <param name="message"></param>
    public void ShowNotifcationFromMenu(string message)
    {
        Vfx_Typewriter scriptTypeWrite = ShowOnPanel(Color.white);
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
        if (notifica != null)
            ManagerHandler.ManagerInstance.SFXManager.PlaySoundEffect(notifica,MovimentoNavicella.GetNavicellaTransform(),1f);
        else
            Debug.LogWarning("Manca notificaSFX in Notification_Manager.cs");

        // instanzio il testo all'interno del content object in NotifcationCascade
        GameObject newText = Instantiate(prefabTxtNotification, notifPanel.transform);
        newText.GetComponent<TextMeshProUGUI>().color = textColor; 
        Destroy(newText, tempoNotifica+1); //distruggo l'oggetto dopo n + 1 secondi
        return newText.GetComponent<Vfx_Typewriter>(); // ritorno lo script per avviare l'animazione della scritura
    }


    /// <summary>
    /// Animazione di scale da 0 a 1 per la notifica (e poi da 1 a 0 per chiuderla)
    /// </summary>
    public IEnumerator ScaleAnimationIcon(RectTransform obj)
    {
        Debug.Log("Animazione icona");
        float elapsedTime = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = new Vector3(0.6f, 0.6f, 0.6f); // Scala finale desiderata

        while (elapsedTime < tempoIconaScale/2) // Durata dell'animazione (metà del tempo totale della notifica)
        {
            obj.transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / (tempoIconaScale/2)); // Prima metà del tempo per scalare fino a 0.6
            elapsedTime += Time.deltaTime;
            yield return null; // Attende il prossimo frame prima di continuare l'animazione
        }

        yield return new WaitForSeconds(tempoIconaFissa); // Tempo in cui l'icona rimane alla scala finale

        elapsedTime = 0f; // Reset del tempo per la seconda parte dell'animazione

        while (elapsedTime < tempoIconaScale/2) // Durata dell'animazione (metà del tempo totale della notifica)
        {
            obj.transform.localScale = Vector3.Lerp(endScale, startScale, elapsedTime / (tempoIconaScale/2)); // Seconda metà del tempo per scalare da 0.6 a 0
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        obj.transform.localScale = startScale; // Assicurati che la scala finale sia esatta (controllo di sicurezza, per essere certi)
    }

    public void PlayScaleAnimationIcon(RectTransform obj)
    {
        StartCoroutine(ScaleAnimationIcon(obj));
    }
}
