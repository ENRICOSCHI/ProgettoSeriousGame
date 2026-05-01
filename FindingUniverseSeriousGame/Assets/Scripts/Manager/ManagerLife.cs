using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.Mathematics;

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

    [Header("Riferimento al Respawn Point")]
    [SerializeField] Transform respawnRedirect; //Idealmente è il medesimo del BorderDetection
    [SerializeField] Transform navicellatransform; //Riferimento per la posizione del player

    [Header("Messaggio Game Over")]
    [SerializeField] string messageGameOver = "Navicella distrutta! Respawn in corso...";
    [SerializeField] Color messageColor; //Impostare da Inspector...

    //Dichiarazione evento
    // L'evento è di tipo Action e trasporta un Vector3 (la posizione dell'impatto)
    // Usa Action al posto di delegate per semplicità, è una funzionalità di C# che permette di definire eventi senza dover creare un'intera classe delegate
    // Questo perchè serve solo qua
    public event Action Collision;



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

    
    /// <summary>
    /// Metodo per la gestione dei danni
    /// </summary>
    public async Awaitable TakeDamage(float amount)
    {
        currentLife -= amount;
        currentLife = Mathf.Clamp(currentLife, 0, maxLife);  // Così la vita non scende sotto lo zero

        // Aggiorna la grafica
        UpdateLifeDisplay();

        
        Collision?.Invoke();  // Lancia l'evento

        #region Logica Game Over

        if (currentLife <= 0)  // Controllo se la vita finisce
        {
            Debug.Log("Nave distrutta!");

            MovimentoNavicella MV = FindAnyObjectByType<MovimentoNavicella>();

            MV.SetCurrentSpeed(0f); // Ferma la navicella
            MV.enabled = false; //Disabilita la facoltà di movimento

            // todo: esplosione 

            // Messaggio di game over
            ManagerHandler.ManagerInstance.NotificationManager.ShowNotifcation(messageGameOver, messageColor);

            await Awaitable.WaitForSecondsAsync(4f);  // Si da il tempo al giocatore di vedere il game over

            Respawn(MV);

        }
        #endregion
    }

    public float GetCurrentLife()
    {
        return currentLife;
    }

    public void SetCurrentLife(float life)
    {
        currentLife = life;
        UpdateLifeDisplay();
    }

    /// <summary>
    /// Riporta la nivacella al punto di respawn indicato e ne ripristina la vita.
    /// </summary>
    private void Respawn(MovimentoNavicella MV)
    {
        navicellatransform.position = respawnRedirect.position;
        MV.enabled = true; // Riabilita il movimento
        currentLife = maxLife; // Ripristina la vita al massimo
        UpdateLifeDisplay(); // Aggiorna la grafica della vita
    }
}
