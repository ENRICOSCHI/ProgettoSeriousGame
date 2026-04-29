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
    public SaveManager SaveManager { get; private set; }
    public ManagerScene SceneManager { get; private set; }


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

        CreateManagers();
    }

    private void CreateManagers()
    {
        if(GetComponentInChildren<ManagerSpeed>() != null) 
            SpeedManager = GetComponentInChildren<ManagerSpeed>();
        if(GetComponentInChildren<MusicManager>() != null)
            MusicManager = GetComponentInChildren<MusicManager>();
        if(GetComponentInChildren<ManagerBattery>() != null)
            BatteryManager = GetComponentInChildren<ManagerBattery>();
        if(GetComponentInChildren<ManagerRotation>() != null)
            RotationManager = GetComponentInChildren<ManagerRotation>();
        if(GetComponentInChildren<DialogueManager>() != null)
            DialogueManager = GetComponentInChildren<DialogueManager>();
        if(GetComponentInChildren<Notification_Manager>() != null)
            NotificationManager = GetComponentInChildren<Notification_Manager>();
        if(GetComponentInChildren<ManagerLife>() != null)
            LifeManager = GetComponentInChildren<ManagerLife>();
        if(GetComponentInChildren<CodexManager>() != null)
            CodexManager = GetComponentInChildren<CodexManager>();
        if(GetComponentInChildren<MissionManager>() != null)
            MissionManager = GetComponentInChildren<MissionManager>();
        if(GetComponentInChildren<SFXManager>() != null)
            SFXManager = GetComponentInChildren<SFXManager>();
        if(GetComponentInChildren<ManagerSubtiitle>() != null)
            SubtitleManager = GetComponentInChildren<ManagerSubtiitle>();
        if(GetComponentInChildren<SaveManager>() != null)
            SaveManager = GetComponentInChildren<SaveManager>();
        if(GetComponentInChildren<ManagerScene>() != null)
            SceneManager = GetComponentInChildren<ManagerScene>();


        // La barra della vita si allinea ai valori attuali al primo frame
        if (LifeManager != null)
        {
            LifeManager.UpdateLifeDisplay();
        }
    }
}
