using UnityEngine;

public class SFXManager : MonoBehaviour
{
    #region Inizializzazione variabili
    [Header("Archivio Effetti Sonori")]
    
    public static SFXManager instance; // Riferimento statico all'istanza del SFXManager per accesso globale
    
    [Tooltip("Inserisci qui il suono di apertura menu")]
    public AudioClip openMenuSound;

    [Tooltip("Inserisci qui il suono di chiusura menu")]
    public AudioClip closeMenuSound;

    [Tooltip("Inserisci qui il suono della finestra di dialogo")]
    public AudioClip[] dialogueWindowSounds; // Array di suoni per la finestra di dialogo, per variare un po' l'effetto

    // L'altoparlante dedicato solo agli effetti
    [SerializeField] private AudioSource sfxSource;
    #endregion



    void Awake()
    {
        if (instance == null)
        {
            // Se non esiste ancora nessun SFXManager, questo diventa l'istanza
            instance = this; 
            DontDestroyOnLoad(gameObject); // Mantieni questo oggetto tra le scene
        }
        else
        {
            // Se c'è già un altro SFXManager, questo "clone" si suicida
            Destroy(gameObject); 
            return;
        }
        
        // Impostiamo l'AudioSource in modo sicuro per gli effetti 2D
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.spatialBlend = 0f; // Completamente 2D (nelle tue orecchie)
    }


    #region Metodi per i suoni

    public void PlayOpenMenu()
    {
        if (openMenuSound != null) sfxSource.PlayOneShot(openMenuSound);
    }

    public void PlayCloseMenu()
    {
        if (closeMenuSound != null) sfxSource.PlayOneShot(closeMenuSound);
    }

    public void PlayDialogueWindow()
    {
        // Controlliamo che l'array non sia vuoto (per evitare errori se dimentichi di mettere i suoni su Unity)
        if (dialogueWindowSounds != null && dialogueWindowSounds.Length > 0)
        {
            // Scegliamo un numero a caso da 0 fino alla grandezza dell'array
            int indiceCasuale = Random.Range(0, dialogueWindowSounds.Length);
            
            // Suoniamo l'audio corrispondente a quel numero
            sfxSource.PlayOneShot(dialogueWindowSounds[indiceCasuale]);
        }
    }
    #endregion
    
}