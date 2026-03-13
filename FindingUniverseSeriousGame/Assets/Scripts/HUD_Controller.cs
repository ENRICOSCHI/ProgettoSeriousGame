using UnityEngine;
using TMPro;
using UnityEngine.UI;  // Gestisce elementi UI come barre e immagini

public class HUD_Controller : MonoBehaviour
{
    [Header("Riferimenti Script")]
    public MovimentoNavicella spaceship;


    [Header("Riferimenti Batteria")]
    public Image batteryFillImage; // Immagine che rappresenta la barra della batteria
    public TMP_Text batteryText; // Testo che mostra la percentuale della batteria
    public float maxBattery = 100f;
    public float currentBattery = 100f;
    public Color colorOk = Color.green;      // Il colore che indica la batteria in buone condizioni
    public Color colorDanger = Color.red;    // Il colore che indica la batteria scarica
    public float sogliaPericolo = 20f;       // Percentuale sotto la quale cambia colore


    [Header("Riferimenti Audio")]
    public AudioSource musicSource;  //In futuro ci sarà la sorgente audio


    [Header("Riferimenti UI")]
    public TMP_Text speedUIText;
    public GameObject musicUIElement; // Oggetto che contiene la scritta "Musica in riproduzione"

    void Update()
    {
        // Richiama le varie funzione per gli aggiornamenti dell'HUD
        UpdateSpeedDisplay();
        UpdateMusicDisplay();
        UpdateBatteryDisplay();

        // In futuro ci aggiungeremo tutti i controlli dell'HUD, come ad esempio la batteria, la minimappa, ecc...
    }


    void UpdateSpeedDisplay()  // Aggiorna la visualizzazione della velocità
    {
        if (spaceship != null && speedUIText != null)
        {
            speedUIText.text = spaceship.CurrentSpeed.ToString("F1");
        }
    }

    void UpdateMusicDisplay()  // Aggiorna la visualizzazione dello stato della musica
    {
        if (musicSource != null && musicUIElement != null)
        {
            // SetActive attiva o disattiva l'oggetto in base a una condizione.
            // In questo caso, l'oggetto è attivo SOLO SE la musica sta suonando.
            musicUIElement.SetActive(musicSource.isPlaying);
        }
        else if (musicUIElement != null)
        {
            // Se non c'è ancora una sorgente audio, teniamo la scritta spenta per sicurezza
            musicUIElement.SetActive(false);
        }
    }

    void UpdateBatteryDisplay()  // Aggiorna la visualizzazione della batteria
    {
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

        if (currentBattery < 20f)  // Se la batteria è sotto il 20%, cambia il colore del testo in rosso per avvisare il giocatore
            batteryText.color = colorDanger; // O il tuo colore del pericolo specifico
        else
            batteryText.color = colorOk; // O il tuo colore neon specifico
    }
}