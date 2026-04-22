using UnityEngine;

public class PersistentSceneData : MonoBehaviour
{
    public static PersistentSceneData Instance { get; private set; }

    //Flag per controllare il cambio di scena
    [HideInInspector] public bool isChangingScene = false;

    // Flag per evitare di ripetere la descrizione ogni volta che si entra negli eventi
    [HideInInspector] public bool isDescriptionUmbraHappened = false;
    [HideInInspector] public bool isDescriptionFiondaHappened = false;
    [HideInInspector] public bool isDescriptionVentoSolareHappened = false;

    void Awake()
    {
        if(Instance != null) return;

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

}
