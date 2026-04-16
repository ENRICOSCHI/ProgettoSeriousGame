using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class SaveManager : MonoBehaviour
{
    [ContextMenu("salvaTest")]
    public void SaveEvent()
    {
        DelegateClass.SaveEventHandler?.Invoke();
    }

    public string GetPathForCodex() => Path.Combine(Application.persistentDataPath, "codexSave.json");
    public string GetPathForMission() => Path.Combine(Application.persistentDataPath, "missionSave.json");

    /*public bool JsonCheck(string path)
    {
        if (File.Exists(path))
        {
            return true;
        }
        else
        {
            Debug.LogWarning($"File JSON non trovato: {path}");

            return false;
        }
    }*/
}
