using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MusicUI : MonoBehaviour
{
    #region Riferimenti UI
    [Header("Riferimenti UI (TextMeshPro)")]
    [Tooltip("Trascina qui il testo del titolo")]
    [SerializeField] private TMP_Text titleText;
    
    [Tooltip("Trascina qui il testo dell'autore")]
    [SerializeField] private TMP_Text artistText;
    
    [Tooltip("Trascina qui il testo dell'album")]
    [SerializeField] private TMP_Text albumText;
    
    [Tooltip("Trascina qui il testo dell'anno")]
    [SerializeField] private TMP_Text yearText;

    [Header("Riferimenti UI (Barra)")]
    [Tooltip("Trascina qui l'immagine che fa da riempimento della barra")]
    [SerializeField] private Image progressBarFill; // L'immagine con Image Type = Filled
    #endregion

    // Questa funzione viene chiamata dal RadioController solo quando cambia la canzone
    public void UpdateTrackInfo(SongData currentSong)
    {
        // Controllo di sicurezza
        if (currentSong == null) return;

        // Aggiorniamo i testi. Se un campo non ti serve e non lo hai assegnato, lo salta senza dare errori.
        if (titleText != null) titleText.text = currentSong.title;
        if (artistText != null) artistText.text = currentSong.artist;
        if (albumText != null) albumText.text = currentSong.album;
        if (yearText != null) yearText.text = currentSong.year;
    }

    public void UpdateProgressBar(float progressPercentage)
    {
        if (progressBarFill != null)
        {
            // fillAmount accetta un valore da 0.0 (vuota) a 1.0 (piena)
            progressBarFill.fillAmount = progressPercentage; 
        }
    }
}