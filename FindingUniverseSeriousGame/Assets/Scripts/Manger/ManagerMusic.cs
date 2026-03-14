using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ManagerMusic : MonoBehaviour
{
    //singleton
    public static ManagerMusic Instance { get; private set; }

    [Header("Riferimenti Audio")]
    [SerializeField] AudioSource musicSource;  //In futuro ci sar� la sorgente audio

    [Header("Riferimenti UI")]
    [SerializeField] GameObject musicUIElement; // Oggetto che contiene la scritta "Musica in riproduzione"


    //controllo che effettivamente sia l'unico oggetto attivo nelle scene (singleton)
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
