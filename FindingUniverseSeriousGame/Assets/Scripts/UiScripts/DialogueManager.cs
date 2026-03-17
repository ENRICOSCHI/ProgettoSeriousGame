using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        ShowMessage("Sistemi di bordo online. Benvenuto, Comandante. adadakdjadkajdnakd.ad dad adadadjawdjaodiq.dq,dqdqdjaodk,awa.da,dadw wdawda.");
    }


    /// <summary>
    /// Mostra un messaggio di dialogo
    /// </summary>
    /// <param name="message"></param>
    public void ShowMessage(string message)
    {
        dialogueGameObjectUI.SetActive(false);//resetto l'animazione, se tolgo questa riga l'animazione non sarà pulita al secondo richiamo del dialogue box
        dialogueGameObjectUI.SetActive(true);
        typewriter.ClearText(); // Puliamo il testo prima di iniziare);

        StopAllCoroutines();
        StartCoroutine(DialogueSequence(message));
    }

    /// <summary>
    /// Sequenza completa per mostrare un messaggio di dialogo
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private IEnumerator DialogueSequence(string message) 
    {
        
        // 1. Chiedi alla UI di aprire la box
        ShowPrompt(false); // Assicuriamoci che il prompt sia nascosto all'inizio
        ShowBox();

        // 2. LOGICA "HANDSHAKE": Aspetto finchè l'animazione non è finita
        yield return new WaitUntil(() => animDialogueBox.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        //yield return new WaitForSeconds(.5f);

        // 3. Loop dialogo finchè non finisce la stringa message
        string[] pages = SplitText(message, typewriter.MAX_CHAR_TEXT);

        foreach (string page in pages)
        {
            //scrivo la pagina attuale
            yield return StartCoroutine(typewriter.TypeText(page));

            //mostro prompt
            ShowPrompt(true);

            //aspetto input
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

            ShowPrompt(false);
        }

        // 5. Chiudi
        ShowPrompt(false); // Nascondiamo il prompt
        yield return new WaitForSeconds(0.2f);
        HideBox();
    }


    private void ShowBox()  // Attiva l'animazione di apertura della box, non il testo
    {
        if (animDialogueBox != null)
            animDialogueBox.SetBool("Show_Dialogue_Box", true);

        // Non attiviamo il testo qui, aspettiamo l'evento.
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

    /// <summary>
    /// Suddivito il testo in più "pagine"
    /// </summary>
    /// <param name="text"></param>
    /// <param name="maxChars"></param>
    /// <returns></returns>
    string[] SplitText(string text, int maxChars)
    {
        List<string> result = new(); //creo lista
        //Faccio un for che va avanti ogni tot. caratteri
        //Salvo quei caratteri nella lista
        for (int i = 0; i < text.Length; i += maxChars)
        {
            //controllo in caso la stringa sia più corta di maxChars
            //e prendo solo i caratteri rimasti 
            int length = Mathf.Min(maxChars, text.Length - i);
            //prendo i caratteri da i fino a lenght e li salvo nella lista
            result.Add(text.Substring(i, length));
        }

        return result.ToArray();
    }
}