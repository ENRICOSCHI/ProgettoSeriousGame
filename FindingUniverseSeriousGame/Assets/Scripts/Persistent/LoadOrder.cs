using UnityEngine;
using System.Collections.Generic;

public class LoadOrder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // chiamo load Menu e Navicella
        DelegateClass.LoadEventHandler?.Invoke(PersistentSceneData.Instance.isChangingScene);
        var missionsListContainer = FindObjectsByType<Quest_3_Script>(FindObjectsSortMode.None); // raccolgo tutte le missioni di tipo 3 presenti nella scene
        //inizializzo tutte le missioni
        foreach(var mission in missionsListContainer) mission.Init();
    }
}
