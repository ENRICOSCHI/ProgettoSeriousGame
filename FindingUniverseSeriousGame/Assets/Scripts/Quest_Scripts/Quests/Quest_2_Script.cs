using UnityEngine;

public class Quest_2_Script : MonoBehaviour
{
    public string questName;
    public bool questStarted = false;
    public bool questCompleted = false;

    [Header("Quest Assets")]
    public Transform player;
    public KeyCode scansionKey; //tasto da premere per scansionare il pianeta;
    public float scanDistance; //distanza massima per poter scansionare il pianeta: da settare
    public Transform planetToScan; //pianeta da scansionare


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //controllo di sicurezza ai riferimenti
        if (player == null || planetToScan == null) return;

        //controllo sullo stato della quest.
        if (!questStarted || questCompleted) return;

        //controllo sulla condizione di completamento della Quest.
        if (Input.GetKeyDown(scansionKey) && Vector3.Distance(player.position, planetToScan.position) <= scanDistance)
            {
                FinishQuest();
            }
    }

    /// <summary>
    /// Il metodo StartQuest() va collegato ad un Trigger per far partire la quest
    /// </summary>
    public void StartQuest()
    {
        if (player != null && planetToScan != null)
        {
            if (!questStarted)
            {
                questStarted = true;
                //WIP: anche qua possibile aggiunta di messaggio alla UI
                Debug.Log("Quest 2 Cominciata!");
            }
        }
        else Debug.LogWarning("Possibili riferimenti a Oggetti mancanti per la Quest 2 in: " + gameObject.name);
    }
    
    /// <summary>
    /// Termina la Quest 2.
    /// </summary>
    public void FinishQuest()
    {
        if (questStarted && !questCompleted)
        {
            questCompleted = true;
            Debug.Log("Quest 2 Completata!");
        }
    }
}
