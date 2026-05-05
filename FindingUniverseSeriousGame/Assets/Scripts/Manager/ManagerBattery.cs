using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Data;

public class ManagerBattery : MonoBehaviour
{
    [Header("Riferimenti Batteria")]
    [SerializeField] Image batteryFillImage; // Immagine che rappresenta la barra della batteria
    [SerializeField] TMP_Text batteryText; // Testo che mostra la percentuale della batteria
    [SerializeField] float maxBattery = 100f;
    [SerializeField] float currentBattery = 100f;
    [SerializeField] Color colorOk = Color.cyan;      // Il colore che indica la batteria in buone condizioni
    [SerializeField] Color colorDanger = Color.blue;    // Il colore che indica la batteria scarica
    [SerializeField] float sogliaPericolo = 20f;       // Percentuale sotto la quale cambia colore

    [Header("Sound Effects")]
    [SerializeField] AudioClip batteriaMAXSfx;
    [SerializeField] AudioClip batteriaScaricaSFX;
    

    /// <summary>
    /// Aggiorna la visualizzazione della batteria
    /// </summary>
    public void UpdateBatteryDisplay()
    {
        if (currentBattery == maxBattery)
        {
            if (batteriaMAXSfx != null)
                ManagerHandler.ManagerInstance.SFXManager.PlaySoundEffect(batteriaMAXSfx, gameObject.transform, 1f);
            else
                Debug.LogWarning("Manca batteriaMAXSfx in ManagerBattery.cs");
        }
            

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

        if (currentBattery < sogliaPericolo)// Se la batteria � sotto la soglia di pericolo, cambia il colore del testo in rosso per avvisare il giocatore
        {  
            batteryText.color = colorDanger;// O il tuo colore del pericolo specifico
            if (batteriaScaricaSFX != null)
                ManagerHandler.ManagerInstance.SFXManager.PlaySoundEffect(batteriaScaricaSFX, gameObject.transform, 1f);
            else
                Debug.LogWarning("Manca batteriaScaricaSFX in ManagerBattery.cs");
        }
        else
            batteryText.color = colorOk; // O il tuo colore neon specifico
    }

    public float GetCurrentBattery()
    {
        return currentBattery;
    }

    public void SetCurrentBattery(float battery)
    {
        currentBattery = battery;
        UpdateBatteryDisplay();
    }
}
