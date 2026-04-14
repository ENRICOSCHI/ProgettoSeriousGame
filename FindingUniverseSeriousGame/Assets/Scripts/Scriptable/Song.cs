using UnityEngine;

[CreateAssetMenu(fileName = "Song", menuName = "Scriptable Objects/Song")]
public class Song : ScriptableObject
{
    [Header("ID Salvataggi")]
    [Tooltip("ID unico da usare per i salvataggi")]
    public string songID;

    [Header("Audio Clip Canzone")]
    [Tooltip("Trascina qui il file .ogg della canzone")]
    public AudioClip audioFile;

    [Header("Info canzone")]
    public string title;
    public string artist;
    public string album;
    public string year;

    [Header("ID di riferimento nel codex")]
    [Tooltip("Indice nel codex")]
    public int songIDCodex;

    [Header("Parametri sottitoli")]
    public AudioClip subtitleAudioClip;
    public Subtitles[] subtitles;
}
