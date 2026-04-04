using UnityEngine;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    #region Variabili dell'Inspector

    [Header("Archivio Musicale")]
    [Tooltip("Aggiungi qui tutte le canzoni. Unity userà il modello SongData per ogni slot.")]
    public SongData[] playlist; 

    #endregion


    #region Variabili Nascoste (Logica Shuffle)

    // La lista che funziona da "riproduzione casuale" per non ripetere le canzoni
    private List<int> shufflePool = new List<int>();

    #endregion


    void Start()
    {
        // All'avvio del gioco, riempiamo la lista per la prima volta
        ResetShufflePool();
    }

    /// <summary>
    /// Inserisce nella lista tutti gli ID delle canzoni (es. da 0 a 5 se hai 6 canzoni)
    /// </summary>
    private void ResetShufflePool()
    {
        shufflePool.Clear(); 
        
        for (int i = 0; i < playlist.Length; i++)
        {
            shufflePool.Add(i); 
        }
        
        Debug.Log("Musica: Playlist mescolata. Pronti a trasmettere!");
    }

    /// <summary>
    /// Estrae una canzone casuale senza ripetizioni. (algoritmo Shuffle Bag o Pool)
    /// </summary>
    public SongData GetRandomSong()
    {
        // Controllo di sicurezza
        if (playlist == null || playlist.Length == 0)
        {
            Debug.LogWarning("Radio: Non ci sono canzoni nella playlist!");
            return null;
        }


        // Se la lista si è svuotato, lo riempiamo di nuovo
        if (shufflePool.Count == 0)
        {
            ResetShufflePool();
        }

        // Pesca un elemento a caso
        int randomPoolIndex = Random.Range(0, shufflePool.Count);
        
        // Legge l'id della canzone corrispondente all'elemento pescato
        int songIndex = shufflePool[randomPoolIndex];
        
        // Elimina l'elemento dalle possibilità future
        shufflePool.RemoveAt(randomPoolIndex);

        // Consegna i dati della canzone
        return playlist[songIndex];
    }
}