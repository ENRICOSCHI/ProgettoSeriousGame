using UnityEngine;
using System.Collections.Generic;

public class LoadOrder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // chiamo load Menu e Navicella
        DelegateClass.LoadEventHandler?.Invoke(PersistentSceneData.Instance.isChangingScene);
    }
}
