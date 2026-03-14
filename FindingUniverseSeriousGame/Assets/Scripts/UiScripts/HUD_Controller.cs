using UnityEngine;
using TMPro;
using UnityEngine.UI;  // Gestisce elementi UI come barre e immagini

public class HUD_Controller : MonoBehaviour
{
    //reference script navicella
    private MovimentoNavicella spaceship;

    private void Awake()
    {
        spaceship = GameObject.FindFirstObjectByType<MovimentoNavicella>();//trovo lo script nella scena
    }

    void Update()
    {
        // Richiama le varie funzione per gli aggiornamenti dell'HUD
        ManagerSpeed.Instance.UpdateSpeedDisplay(spaceship);
        // In futuro ci aggiungeremo tutti i controlli dell'HUD, come ad esempio la batteria, la minimappa, ecc...
    }

    
}