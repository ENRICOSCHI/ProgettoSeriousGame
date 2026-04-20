using UnityEngine;

public class PersistentSceneData : MonoBehaviour
{
    public static PersistentSceneData Instance { get; private set; }
    [HideInInspector] public bool isChangingScene = false;

    void Awake()
    {
        if(Instance != null) return;

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

}
