using UnityEngine;

public class ManagerHandler : MonoBehaviour
{
    //singleton
    public static ManagerHandler ManagerInstance { get; private set; }

    public ManagerSpeed SpeedManager { get; private set; }
    public ManagerMusic MusicManager { get; private set; }
    public MangerBattery BatteryManager { get; private set; }
    public ManagerRotation RotationManager { get; private set; }
    public DialogueManager DialogueManager { get; private set; }

    //controllo che effettivamente sia l'unico oggetto attivo nelle scene (singleton)
    void Awake()
    {
        if (ManagerInstance == null)
        {
            ManagerInstance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpeedManager = GetComponentInChildren<ManagerSpeed>();
        MusicManager = GetComponentInChildren<ManagerMusic>();
        BatteryManager = GetComponentInChildren<MangerBattery>();
        RotationManager = GetComponentInChildren<ManagerRotation>();
        DialogueManager = GetComponentInChildren<DialogueManager>();
    }
}
