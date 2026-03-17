using UnityEngine;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject dialogueGameObjectUI;
    [SerializeField] private Vfx_Typewriter typewriter;  // Oggetto con il testo
    [SerializeField] private GameObject prompText; // Per il prompt "Premi X per continuare"

    private Animator animDialogueBox;

    private void OnEnable()
    {
        DelegateClass.DialogueBoxEventsHandler += ShowMessage;
    }
    private void OnDisable()
    {
        DelegateClass.DialogueBoxEventsHandler -= ShowMessage;
    }
    private void Start()
    {
        animDialogueBox = dialogueGameObjectUI.GetComponent<Animator>();
    }

    // Esempio di come chiamarlo: DialogueManager.Instance.ShowMessage("Ciao!");
    [ContextMenu("Test Dialogue")] // Comando di test (eseguibile premendo tasto destro sul componente in Inspector)
    public void TestDialogue()
    {
        ShowMessage("Sistemi di bordo online. Benvenuto, Comandante.");
    }

    
    public void ShowMessage(string message)  // Funzione pubblica per mostrare un messaggio di dialogo
    {
        dialogueGameObjectUI.SetActive(true);
        typewriter.ClearText(); // Puliamo il testo prima di iniziare

        StopAllCoroutines();
        StartCoroutine(DialogueSequence(message));
    }

    private IEnumerator DialogueSequence(string message)  // Sequenza completa per mostrare un messaggio di dialogo
    {
        
        // 1. Chiedi alla UI di aprire la box
        ShowPrompt(false); // Assicuriamoci che il prompt sia nascosto all'inizio
        ShowBox();

        // 2. LOGICA "HANDSHAKE": Aspetto finchè l'animazione non è finita
        yield return new WaitUntil(() => animDialogueBox.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        // 3. Ora che la box è aperta e il testo è attivo, scrivi!
        yield return StartCoroutine(typewriter.TypeText(message));
        ShowPrompt(true); // Mostriamo il prompt per continuare

        // 4. Tempo di lettura
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));  // Aspetta che il giocatore prema E per continuare

        // 5. Chiudi
        ShowPrompt(false); // Nascondiamo il prompt
        yield return new WaitForSeconds(0.2f);
        HideBox();
    }


    private void ShowBox()  // Attiva l'animazione di apertura della box, non il testo
    {
        if (animDialogueBox != null)
            animDialogueBox.SetBool("Show_Dialogue_Box", true);

        // Non attiviamo il testo qui! Aspettiamo l'evento.
    }

    private void HideBox()  // Chiude la box e resetta tutto.
    {
        if (animDialogueBox != null)
            animDialogueBox.SetBool("Show_Dialogue_Box", false);
    }

    private void ShowPrompt(bool state)  // Mostra o nasconde il prompt "Premi X per continuare"
    {
        if (prompText != null)
            prompText.SetActive(state);
    }
}