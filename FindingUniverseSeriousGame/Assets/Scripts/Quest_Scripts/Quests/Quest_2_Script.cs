using UnityEngine;

public class Quest_2_Script : Quest_Generic_Script
{
    // Template per una Quest di scansione di un pianeta

    [Header("Quest Assets")]
    public Transform player;
    [SerializeField] KeyCode scansionKey; //tasto da premere per scansionare il pianeta;
    [SerializeField] float scanDistance; //distanza massima per poter scansionare il pianeta: da settare
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
    public override void StartQuest()
    {
        if (player != null && planetToScan != null)
        {
            base.StartQuest();
        }
        else Debug.LogWarning("Possibili riferimenti a Oggetti mancanti per la Quest 2 in: " + gameObject.name);
    }
}
