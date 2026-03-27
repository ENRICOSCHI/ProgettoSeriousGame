using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MangerBattery : MonoBehaviour
{
    [Header("Riferimenti Batteria")]
    [SerializeField] Image batteryFillImage; // Immagine che rappresenta la barra della batteria
    [SerializeField] TMP_Text batteryText; // Testo che mostra la percentuale della batteria
    [SerializeField] float maxBattery = 100f;
    [SerializeField] float currentBattery = 100f;
    [SerializeField] Color colorOk = Color.cyan;      // Il colore che indica la batteria in buone condizioni
    [SerializeField] Color colorDanger = Color.blue;    // Il colore che indica la batteria scarica
    [SerializeField] float sogliaPericolo = 20f;       // Percentuale sotto la quale cambia colore
    

    /// <summary>
    /// Aggiorna la visualizzazione della batteria
    /// </summary>
    public void UpdateBatteryDisplay()
    {
        /*chiamare tramite evento quando c'è' bisogno di cambiare la carica della batteria*/
        if (batteryFillImage != null)
        {
            // Il FillAmount richiede un valore tra 0 e 1. Permette alla barra di riempirsi in modo proporzionale alla carica attuale.
            // Dividiamo la carica attuale per il massimo (es. 80/100 = 0.8)
            batteryFillImage.fillAmount = currentBattery / maxBattery;
        }

        //Aggiorna il testo della batteria in percentuale
        if (batteryText != null)
        {
            batteryText.text = $"{currentBattery:F0}%"; // F0 formatta il numero senza decimali
        }

        if (currentBattery < sogliaPericolo)  // Se la batteria � sotto la soglia di pericolo, cambia il colore del testo in rosso per avvisare il giocatore
            batteryText.color = colorDanger; // O il tuo colore del pericolo specifico
        else
            batteryText.color = colorOk; // O il tuo colore neon specifico
    }
}
