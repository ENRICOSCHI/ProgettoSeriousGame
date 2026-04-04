using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;


public class RadioController : MonoBehaviour
{
    #region Inizializzazione e Riferimenti

    [Tooltip("Trascina qui l'oggetto UI che ha lo script MusicUI")]
    [SerializeField] private MusicUI musicUI;

    [Header("Riferimenti")]
    [Tooltip("Trascina qui lo script MusicManager che fa da archivio")]
    private MusicManager musicManager;


    [SerializeField] private AudioSource audioSource; // Il "lettore cd" effettivo



    [Header("Impostazioni Tasti (Modificabili)")]
    public KeyCode keyPlayPause = KeyCode.Space;
    public KeyCode keyNext = KeyCode.E;
    public KeyCode keyPrevious = KeyCode.Q;

    // Variabili per tracciare lo stato della radio
    private bool isManuallyPaused = false;

    // La cronologia per permetterci di andare "Indietro" anche se la playlist è casuale
    private List<SongData> playHistory = new List<SongData>();
    private int historyIndex = -1; // -1 significa che non abbiamo ancora iniziato

    #endregion


    void Awake()
    {
        musicManager = GetComponent<MusicManager>();

        // Impostazioni di sicurezza per l'AudioSource
        audioSource.playOnAwake = false;
        audioSource.loop = false; // Importante: non deve loopare, altrimenti non passa alla canzone successiva
    }

    void Start()
    {
        // La radio parte da spenta
        isManuallyPaused = false;
    }

    void Update()
    {
        HandleInputs();
        CheckTrackEnd();

        UpdateProgressUI();
    }


    /// <summary>
    /// Calcola a che punto è la canzone e manda il dato allo schermo UI
    /// </summary>
    private void UpdateProgressUI()
    {
        // Se c'è una UI collegata e c'è una canzone caricata nel lettore...
        if (musicUI != null && audioSource.clip != null)
        {
            // Calcoliamo la percentuale di completamento (es. 60 secondi attuali / 120 secondi totali = 0.5)
            float progress = audioSource.time / audioSource.clip.length;
            
            // Passiamo il valore allo schermo
            musicUI.UpdateProgressBar(progress);
        }
    }


    #region Logica di Controllo (Input e Auto-Skip)

    /// <summary>
    /// Gestisce i tasti premuti dal giocatore.
    /// </summary>
    private void HandleInputs()
    {
        if (Input.GetKeyDown(keyPlayPause))
        {
            TogglePlayPause();
        }

        if (Input.GetKeyDown(keyNext))
        {
            PlayNextSong();
        }

        if (Input.GetKeyDown(keyPrevious))
        {
            PlayPreviousSong();
        }
    }

    /// <summary>
    /// Controlla se la canzone attuale è finita naturalmente, per passare alla prossima in automatico.
    /// </summary>
    private void CheckTrackEnd()
    {
        // Se l'audio non sta suonando, non siamo in pausa manuale, e abbiamo effettivamente una canzone caricata...
        if (!audioSource.isPlaying && !isManuallyPaused && audioSource.clip != null)
        {
            // Unity rileva che la traccia è finita (time è arrivato in fondo o a 0)
            if (audioSource.time == 0f || audioSource.time >= audioSource.clip.length)
            {
                PlayNextSong();
            }
        }
    }

    #endregion


    #region Comandi Radio

    /// <summary>
    /// Mette in pausa o fa ripartire la canzone attuale.
    /// </summary>
    private void TogglePlayPause()
    {
        if (audioSource.isPlaying)
        {
            // Se sta suonando, metti in pausa
            audioSource.Pause();
            isManuallyPaused = true;
        }
        else
        {
            // Se eravamo fermi all'avvio e non abbiamo ancora pescato nessuna canzone
            if (audioSource.clip == null)
            {
                PlayNextSong(); // Pesca la prima canzone e la fa partire!
            }
            else
            {
                // Se invece avevamo solo messo in pausa una canzone già in corso, riprendiamo
                audioSource.UnPause();
                isManuallyPaused = false;

                // Sicurezza extra: se non è ripartita, forziamo il Play
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
        }
    }

    /// <summary>
    /// Salta alla traccia successiva (o la pesca dal database se è una traccia nuova).
    /// </summary>
    private void PlayNextSong()
    {
        // Se non siamo all'ultima canzone della nostra cronologia (es. eravamo andati indietro)
        if (historyIndex < playHistory.Count - 1)
        {
            // Scorriamo semplicemente avanti nella cronologia
            historyIndex++;
            LoadAndPlay(playHistory[historyIndex]);
        }
        else
        {
            // Altrimenti, siamo al limite della cronologia. Chiediamo una canzone nuova al Database!
            if (musicManager != null)
            {
                SongData newSong = musicManager.GetRandomSong();

                if (newSong != null)
                {
                    playHistory.Add(newSong); // La salviamo nella cronologia
                    historyIndex = playHistory.Count - 1; // Aggiorniamo l'indice all'ultima posizione
                    LoadAndPlay(newSong);
                }
            }
        }
    }

    /// <summary>
    /// Torna alla traccia precedente sfruttando la cronologia.
    /// </summary>
    private void PlayPreviousSong()
    {
        // Se l'indice è maggiore di 0, significa che c'è almeno una canzone prima di questa
        if (historyIndex > 0)
        {
            historyIndex--;
            LoadAndPlay(playHistory[historyIndex]);
        }
        else
        {
            // Se siamo alla prima canzone in assoluto, al massimo la facciamo ricominciare da capo
            audioSource.time = 0f;
            if (isManuallyPaused) TogglePlayPause();
        }
    }

    /// <summary>
    /// Funzione interna che carica fisicamente il file audio e lo fa partire.
    /// </summary>
    private void LoadAndPlay(SongData songData)
    {
        if (songData.audioFile != null)
        {
            audioSource.clip = songData.audioFile;
            audioSource.time = 0f; 
            audioSource.Play();
            isManuallyPaused = false;

            // Aggiorna lo schermo con i dati della canzone
            if (musicUI != null)
            {
                musicUI.UpdateTrackInfo(songData);
            }

            Debug.Log($"Radio: In riproduzione '{songData.title}' di {songData.artist}");
        }
    }

    #endregion
}