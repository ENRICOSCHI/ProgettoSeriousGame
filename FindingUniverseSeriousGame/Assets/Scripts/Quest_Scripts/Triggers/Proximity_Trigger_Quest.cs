using UnityEngine;
using UnityEngine.Events;

public class Proximity_Trigger_Quest : MonoBehaviour
{
    /*Il presente Script va collegato al GameObject che bisogna avvicinare per attivare la quest, 
    e va settato con il player, la distanza di trigger e la quest da attivare*/
    
    public Transform player;
    [SerializeField] float triggerDistance; //da settare, non mi rendo conto di quato possa essere ragionevole impostarla
    public UnityEvent onPlayerTrigger; //collegare lo script della quest da attivare

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
        
        if (Vector3.Distance(transform.position, player.position) <= triggerDistance)
        {
            onPlayerTrigger.Invoke();
            //disabilita questo script per evitare che venga attivato più volte
            this.enabled = false;
        }
    }
}
