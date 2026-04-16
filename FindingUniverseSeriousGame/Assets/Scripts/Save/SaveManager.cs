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

    [ContextMenu("caricaTest")]
    public void LoadEvent()
    {
        DelegateClass.LoadEventHandler?.Invoke();
    }

    public string GetPathForCodex() => Path.Combine(Application.persistentDataPath, "codexSave.json");
    public string GetPathForMission() => Path.Combine(Application.persistentDataPath, "missionSave.json");
}
