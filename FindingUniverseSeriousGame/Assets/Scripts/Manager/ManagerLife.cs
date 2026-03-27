using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ManagerLife : MonoBehaviour
{
    [Header("Riferimenti Vita")]
    [SerializeField] Image lifeFillImage; // Immagine che rappresenta la barra della vita
    [SerializeField] TMP_Text lifeText; // Testo che mostra la percentuale della vita
    [SerializeField] float maxLife = 100f;
    [SerializeField] float currentLife = 100f;
    [SerializeField] Color colorOk = Color.green;      // Il colore che indica la vita in buone condizioni
    [SerializeField] Color colorDanger = Color.red;    // Il colore che indica la vita in pericolo
    [SerializeField] float sogliaPericolo = 20f;       // Percentuale sotto la quale cambia colore
    

    /// <summary>
    /// Aggiorna la visualizzazione della vita
    /// </summary>
    public void UpdateLifeDisplay()
    {
        /*chiamare tramite evento quando c'è' bisogno di cambiare la carica della vita*/
        if (lifeFillImage != null)
        {
            // Il FillAmount richiede un valore tra 0 e 1. Permette alla vita di riempirsi in modo proporzionale alla carica attuale.
            // Dividiamo la carica attuale per il massimo (es. 80/100 = 0.8)
            lifeFillImage.fillAmount = currentLife / maxLife;
        }

        //Aggiorna il testo della vita in percentuale
        if (lifeText != null)
        {
            lifeText.text = $"{currentLife:F0}%"; // F0 formatta il numero senza decimali
        }

        if (currentLife < sogliaPericolo)  // Se la vita e' sotto la soglia di pericolo, cambia il colore del testo in rosso per avvisare il giocatore
            lifeText.color = colorDanger;
        else
            lifeText.color = colorOk;
    }
}
