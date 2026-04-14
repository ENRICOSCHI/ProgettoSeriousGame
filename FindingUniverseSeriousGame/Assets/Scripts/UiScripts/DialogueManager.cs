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

    // Esempio di come chiamarlo: DialogueManager.Instance.ShowNotifcation("Ciao!");
    [ContextMenu("Test Dialogue")] // Comando di test (eseguibile premendo tasto destro sul componente in Inspector)
    public void TestDialogue()
    {
        ShowMessage("Sistemi di bordo online. Benvenuto, Comandante. adadakdjadkajdnakd.ad dad adadadjawdjaodiq.dq,dqdqdjaodk,awa.da,dadw wdawda.");
    }
    #region "Dialogue Box Function"

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

    #endregion

    #region "Subtitle Box Function"
    public void ShowMessageForSubtitle(string message, float durationEvent)
    {
        dialogueGameObjectUI.SetActive(false);//resetto l'animazione, se tolgo questa riga l'animazione non sarà pulita al secondo richiamo del dialogue box
        dialogueGameObjectUI.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(DialogueBoxForSubtitle(message,durationEvent));
    }

    /// <summary>
    /// Attiva la box per mostrare i sottotitoli
    /// </summary>
    /// <param name="message"></param>
    /// <param name="durationEvent"></param>
    /// <returns></returns>
    private IEnumerator DialogueBoxForSubtitle(string message, float durationEvent)
    {

        // 1. Chiedi alla UI di aprire la box
        ShowPrompt(false); // Assicuriamoci che il prompt sia nascosto all'inizio
        ShowBox();

        // 2. LOGICA "HANDSHAKE": Aspetto finchè l'animazione non è finita
        yield return new WaitUntil(() => animDialogueBox.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        //yield return new WaitForSeconds(.5f);

        yield return StartCoroutine(typewriter.TypeText(message));

        //aspetto input
        yield return new WaitForSeconds(durationEvent);

        // 3. Chiudi
        HideBox();
    }

    #endregion

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
    /// Suddivide il testo in più sezioni parola per parola
    /// </summary>
    /// <param name="text">Il testo completo del dialogo</param>
    /// <param name="maxChars">Numero massimo di caratteri per pagina</param>
    /// <returns>Array di stringhe, ognuna rappresenta una pagina</returns>
    string[] SplitText(string text, int maxChars)
    {
        List<string> result = new List<string>();

        // Dividiamo l'intero testo in un array di singole parole (usando lo spazio come separatore)
        string[] words = text.Split(' ');

        string currentPage = "";

        foreach (string word in words)
        {
            // Controlliamo se aggiungendo la parola attuale (più uno spazio) supereremmo il limite.
            // Se sì, salviamo la pagina corrente nella lista e ne iniziamo una nuova.
            if (currentPage.Length + word.Length + 1 > maxChars && currentPage.Length > 0)
            {
                result.Add(currentPage); // Salviamo la pagina completata
                currentPage = "";        // Svuotiamo la pagina per ricominciare
            }

            // Se la pagina non è vuota, aggiungiamo uno spazio prima della parola successiva
            if (currentPage.Length > 0)
            {
                currentPage += " ";
            }

            // Aggiungiamo la parola alla pagina corrente
            currentPage += word;
        }

        // Finito il ciclo, se c'è ancora del testo rimasto nell'ultima pagina, lo salviamo
        if (currentPage.Length > 0)
        {
            result.Add(currentPage);
        }

        return result.ToArray();
    }
}