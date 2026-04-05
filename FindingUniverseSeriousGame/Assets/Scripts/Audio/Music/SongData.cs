using UnityEngine;

/// <summary>
/// Contenitore dati puro per le canzoni della Radio.
/// [System.Serializable] permette a Unity di mostrarlo nell'Inspector.
/// </summary>
[System.Serializable]
public class SongData
{
    [Tooltip("Trascina qui il file .ogg della canzone")]
    public AudioClip audioFile;
    
    public string title;
    public string artist;
    public string album;
    public string year;

    public int songIDCodex;
    [TextArea (3, 10)] public string songDescription;
}