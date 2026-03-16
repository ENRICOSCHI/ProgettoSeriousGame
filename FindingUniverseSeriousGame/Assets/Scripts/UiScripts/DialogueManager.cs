using UnityEngine;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueUI ui;  
    [SerializeField] private Vfx_Typewriter typewriter;  // Oggetto con il testo


    // Esempio di come chiamarlo: DialogueManager.Instance.ShowMessage("Ciao!");



    [ContextMenu("Test Dialogue")] // Comando di test (eseguibile premendo tasto destro sul componente in Inspector)
    public void TestDialogue()
    {
        ShowMessage("Sistemi di bordo online. Benvenuto, Comandante.");
    }


    public void ShowMessage(string message)  // Funzione pubblica per mostrare un messaggio di dialogo
    {
        typewriter.ClearText(); // Puliamo il testo prima di iniziare

        StopAllCoroutines();
        StartCoroutine(DialogueSequence(message));
    }

    private IEnumerator DialogueSequence(string message)  // Sequenza completa per mostrare un messaggio di dialogo
    {
        // 1. Chiedi alla UI di aprire la box
        ui.ShowPrompt(false); // Assicuriamoci che il prompt sia nascosto all'inizio
        ui.ShowBox();

        // 2. LOGICA "HANDSHAKE": Aspetta finché ui.isBoxReady non diventa true
        // Scrivo quando l'animazione di apertura è terminata e il testo è attivo
        yield return new WaitUntil(() => ui.isBoxReady);

        // 3. Ora che la box è aperta e il testo è attivo, scrivi!
        yield return StartCoroutine(typewriter.TypeText(message));
        ui.ShowPrompt(true); // Mostriamo il prompt per continuare

        // 4. Tempo di lettura
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));  // Aspetta che il giocatore prema E per continuare

        // 5. Chiudi
        ui.ShowPrompt(false); // Nascondiamo il prompt
        ui.DeactivateContent();
        yield return new WaitForSeconds(0.2f);
        ui.HideBox();
    }
}