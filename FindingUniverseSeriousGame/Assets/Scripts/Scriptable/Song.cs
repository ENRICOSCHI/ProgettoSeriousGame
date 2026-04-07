using UnityEngine;

[CreateAssetMenu(fileName = "Song", menuName = "Scriptable Objects/Song")]
public class Song : ScriptableObject
{
    [Tooltip("ID unico da usare per i salvataggi")]
    public string songID;

    [Tooltip("Trascina qui il file .ogg della canzone")]
    public AudioClip audioFile;

    public string title;
    public string artist;
    public string album;
    public string year;

    [Tooltip("Indice nel codex")]
    public int songIDCodex;
    [TextArea(3, 10)] public string songDescription;
}
