using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private Animator dialogueAnimator;
    [SerializeField] private GameObject dialogueContent; 
    [SerializeField] private GameObject prompText; // Per il prompt "Premi X per continuare"

    [HideInInspector] public bool isBoxReady = false; // Per sapere quando è il momento di attivare il testo


    public void ShowBox()  // Attiva l'animazione di apertura della box, non il testo
    {
        isBoxReady = false; // Resettiamo lo stato
        if (dialogueAnimator != null)
            dialogueAnimator.SetTrigger("Show_Dialogue_Box");
        
        // Non attiviamo il testo qui! Aspettiamo l'evento.
    }

    public void ActivateContent()  // Questa funzione verrà chiamata dall'Animation Event alla fine della clip
    {
        if (dialogueContent != null)
            dialogueContent.SetActive(true);

        isBoxReady = true; // Ora la box è pronta per mostrare il testo
    }

    public void HideBox()  // Chiude la box e resetta tutto.
    {
        ShowPrompt(false); // Assicuriamoci di nascondere il prompt se è attivo
        isBoxReady = false; // Resettiamo lo stato
        // Spegniamo subito il testo per pulizia
        DeactivateContent();
        
        if (dialogueAnimator != null)
            dialogueAnimator.SetTrigger("Hide_Dialogue_Box");
    }

    public void DeactivateContent() => dialogueContent.SetActive(false);  // Chiusura della box (evita un glitch grafico a fine animazione)

    public void ShowPrompt(bool state)  // Mostra o nasconde il prompt "Premi X per continuare"
    {
        if(prompText != null)
            prompText.SetActive(state);
    }
}