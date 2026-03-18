using UnityEngine;
using UnityEngine.Events;

public class Interaction_Trigger_Script : MonoBehaviour
{
    public Transform player;

    [Header("Trigger Parameters")]
    public KeyCode triggerKey; //key della tastiera da premere per attivare la quest: da settare.
    public float triggerDistance; //distanza massima tra player e l'oggetto trigger per attivare la quest: da settare.
    
    public UnityEvent onPlayerTrigger; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //controllo di sicurezza
        if (player == null)
        {
            Debug.LogWarning("Nessun riferimento al player assegnato in: " + gameObject.name);
            return;
        }
        
        if (Input.GetKeyDown(triggerKey) && Vector3.Distance(transform.position, player.position) <= triggerDistance)
        {
            onPlayerTrigger.Invoke();
            //disabilita questo script per evitare che venga attivato più volte
            this.enabled = false;
        }
    }
}
