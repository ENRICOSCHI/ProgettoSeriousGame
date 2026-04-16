using UnityEngine;

public class ManagerHandler : MonoBehaviour
{
    //singleton
    public static ManagerHandler ManagerInstance { get; private set; }

    public ManagerSpeed SpeedManager { get; private set; }
    public MusicManager MusicManager { get; private set; }
    public ManagerBattery BatteryManager { get; private set; }
    public ManagerRotation RotationManager { get; private set; }
    public DialogueManager DialogueManager { get; private set; }
    public Notification_Manager NotificationManager { get; private set; }
    public ManagerLife LifeManager {get; private set; }
    public CodexManager CodexManager { get; private set; }
    public MissionManager MissionManager { get; private set; }
    public SFXManager SFXManager { get; private set; }
    public ManagerSubtiitle SubtitleManager { get; private set; }


    //controllo che effettivamente sia l'unico oggetto attivo nelle scene (singleton)
    void Awake()
    {
        if (ManagerInstance == null)
        {
            ManagerInstance = this;
            //DontDestroyOnLoad(gameObject); 
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
        MusicManager = GetComponentInChildren<MusicManager>();
        BatteryManager = GetComponentInChildren<ManagerBattery>();
        RotationManager = GetComponentInChildren<ManagerRotation>();
        DialogueManager = GetComponentInChildren<DialogueManager>();
        NotificationManager = GetComponentInChildren<Notification_Manager>();
        LifeManager = GetComponentInChildren<ManagerLife>();
        CodexManager = GetComponentInChildren<CodexManager>();
        MissionManager = GetComponentInChildren<MissionManager>();
        SFXManager = GetComponentInChildren<SFXManager>();
        SubtitleManager = GetComponentInChildren<ManagerSubtiitle>();


        // La barra della vita si allinea ai valori attuali al primo frame
        if (LifeManager != null)
        {
            LifeManager.UpdateLifeDisplay();
        }
    }
}
