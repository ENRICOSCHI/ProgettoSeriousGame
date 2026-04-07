using UnityEngine;

public class SFXManager : MonoBehaviour
{
    #region Inizializzazione variabili
    // L'altoparlante dedicato solo agli effetti
    [SerializeField] private AudioSource sfxSource;
    #endregion


    #region Metodi per i suoni

    /// <summary>
    /// Suono un unico effetto statico
    /// </summary>
    /// <param name="audioClip"></param>
    /// <param name="spawnTransform"></param>
    /// <param name="volume"></param>
    public void PlaySoundEffect(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        // creo oggetto con audio source "sfxSource" nella posizione passata dal parametro
        AudioSource audioSource = Instantiate(sfxSource, spawnTransform.position, Quaternion.identity);

        // assegno la clip all'audio source
        audioSource.clip = audioClip;

        // assegno il volume
        audioSource.volume = volume;

        // avvio l'audio
        audioSource.Play();

        // distruggo l'oggetto alla fine della durata della clip
        Destroy(audioSource.gameObject, audioSource.clip.length);
    }

    /// <summary>
    /// Suono un audioclip causale tra quelle passate come parametro
    /// </summary>
    /// <param name="audioClips"></param>
    /// <param name="spawnTransform"></param>
    /// <param name="volume"></param>
    public void PlayRandomSoundEffect(AudioClip[] audioClips, Transform spawnTransform, float volume)
    {
        // creo oggetto con audio source "sfxSource" nella posizione passata dal parametro
        AudioSource audioSource = Instantiate(sfxSource, spawnTransform.position, Quaternion.identity);

        // prendo una clip causale da audioClips
        // assegno la clip all'audio source
        audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];

        // assegno il volume
        audioSource.volume = volume;

        // avvio l'audio
        audioSource.Play();

        // distruggo l'oggetto alla fine della durata della clip
        Destroy(audioSource.gameObject, audioSource.clip.length);
    }

    #endregion

}