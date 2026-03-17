using UnityEngine;

public class Quest_1_Script : MonoBehaviour
{
    /*Template per una fetch quest di qualcosa*/
    public string questName; 
    public bool questStarted = false;
    public bool questCompleted = false;

    [Header("Quest Assets")]
    public GameObject questItem; //ogetto da tocccare / raccogliere
    public GameObject player;

    private Collider playerCollider;
    private Collider questItemCollider;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (questStarted && !questCompleted)
        {
            /*Controllo se il collider del player interseca quello dell'oggetto della quest. 
            (oltre a controllare che il puntatore non sia nullo)*/
            if (questItem != null && player != null && playerCollider.bounds.Intersects(questItemCollider.bounds))
            {
                FinishQuest();
            }
        }
    }

/// <summary>
/// Il metodo StartQuest va collegato al Proximity Trigger Quest per far partire la quest 
/// quando il player si avvicina all'oggetto trigger.
/// Inoltre abilita l'oggetto della quest se è disabilitato nella Hierarchy.
/// </summary>
    public void StartQuest()
    {
        if (!questStarted && player != null)
        {
            questStarted = true;
            playerCollider = player.GetComponent<Collider>();
            //se l'oggetto e disabilitato lo abilito
            if (!questItem.activeInHierarchy) questItem.SetActive(true);
            if (questItem != null) questItemCollider = questItem.GetComponent<Collider>();
            //WIP: possibile aggiunta di messaggio alla UI
            Debug.Log("Quest 1 Cominciata!");
        }
    }

/// <summary>
/// Termina la Quest 1
/// </summary>
    public void FinishQuest()
    {
        if (questStarted && !questCompleted)
        {
            questItem.SetActive(false);
            questCompleted = true;
            Debug.Log("Quest 1 Completata!");
        }
    }
}
