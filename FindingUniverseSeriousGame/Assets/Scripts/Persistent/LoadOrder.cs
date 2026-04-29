using UnityEngine;
using System.Collections.Generic;

public class LoadOrder : MonoBehaviour
{
    void Start()
    {
        // chiamo load Menu e Navicella
        DelegateClass.LoadEventHandler?.Invoke(PersistentSceneData.Instance.isChangingScene);
    }
}
