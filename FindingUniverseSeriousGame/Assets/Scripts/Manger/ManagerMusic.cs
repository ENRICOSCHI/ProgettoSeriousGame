using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ManagerMusic : MonoBehaviour
{

    [Header("Riferimenti Audio")]
    [SerializeField] AudioSource musicSource;  //In futuro ci sar� la sorgente audio

    [Header("Riferimenti UI")]
    [SerializeField] GameObject musicUIElement; // Oggetto che contiene la scritta "Musica in riproduzione"


    /// <summary>
    /// Aggiorna la visualizzazione dello stato della musica
    /// </summary>
    public void UpdateMusicDisplay()
    {
        if (musicSource != null && musicUIElement != null)
        {
            // SetActive attiva o disattiva l'oggetto in base a una condizione.
            // In questo caso, l'oggetto � attivo SOLO SE la musica sta suonando.
            musicUIElement.SetActive(musicSource.isPlaying);
        }
        else if (musicUIElement != null)
        {
            // Se non c'� ancora una sorgente audio, teniamo la scritta spenta per sicurezza
            musicUIElement.SetActive(false);
        }
    }
}
