using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class SaveManager : MonoBehaviour
{
    [ContextMenu("salvaTest")]
    public void SaveEvent()
    {
        DelegateClass.SaveEventHandler?.Invoke(false);
    }

    [ContextMenu("caricaTest")]
    public void LoadEvent()
    {
        DelegateClass.LoadEventHandler?.Invoke(false);
    }

    public void QuitGame()
    {
        Debug.Log("Chiusura del gioco in corso...");
        
        // Se stiamo provando il gioco dentro l'editor di Unity, ferma la Play Mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        
        // Se abbiamo buildato il gioco (.exe), chiudi l'applicazione
#else
        Application.Quit();
#endif
    }

    public string GetPathForCodex() => Path.Combine(Application.persistentDataPath, "codexSave.json");
    public string GetPathForMission() => Path.Combine(Application.persistentDataPath, "missionSave.json");
    public string GetPathForPlayer() => Path.Combine(Application.persistentDataPath, "playerSave.json");
    public string GetPathForPersistentSceneData() => Path.Combine(Application.persistentDataPath, "persistentSceneDataSave.json");
    public string GetPathForScene() => Path.Combine(Application.persistentDataPath, "scene.json");
}
