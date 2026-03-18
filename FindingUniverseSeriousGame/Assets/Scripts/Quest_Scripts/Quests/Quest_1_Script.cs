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
        //controllo di sicurezza ai riferimenti.
        if (player == null || questItem == null) return;

        //controllo di sicurezza sui Collider.
        if (playerCollider == null || questItemCollider == null) return;

        //controllo sullo stato della Quest.
        if (!questStarted || questCompleted) return;

        //controllo sulla condizione di completamento della Quest.
        if (playerCollider.bounds.Intersects(questItemCollider.bounds))
        {
            FinishQuest();
        }
    }

/// <summary>
/// Il metodo StartQuest va collegato ad un Trigger per far partire la quest. 
/// Inoltre, per la Quest 1 abilita l'oggetto della quest se questo è disabilitato nella Hierarchy.
/// </summary>
    public void StartQuest()
    {
        if (player != null && questItem != null)
        {
            playerCollider = player.GetComponent<Collider>();

            //se l'oggetto e disabilitato lo abilito
            if (!questItem.activeInHierarchy) questItem.SetActive(true);
            questItemCollider = questItem.GetComponent<Collider>();
            
            if (playerCollider != null && questItemCollider != null)
            {
                if (!questStarted)
                {
                    questStarted = true;
                    //WIP: possibile aggiunta di messaggio alla UI
                    Debug.Log("Quest 1 Cominciata!");
                }
            }
            else Debug.LogWarning("Possibili riferimenti a Collider mancanti per la Quest 1 in: " + gameObject.name);
        }
        else Debug.LogWarning("Possibili riferimenti a Oggetti mancanti per la Quest 1 in: " + gameObject.name);
        
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
