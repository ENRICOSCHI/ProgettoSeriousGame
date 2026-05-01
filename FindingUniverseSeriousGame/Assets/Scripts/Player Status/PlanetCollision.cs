using UnityEngine;

public class PlanetCollision : MonoBehaviour
{
    [Header("Riferimenti")]
    [Tooltip("Trascina qui il GameObject che contiene lo script ManagerLife")]
    [SerializeField] private ManagerLife managerLife;
    
    private async void OnCollisionEnter(Collision collision)
    {
        //Controllo se lo schianto avviene contro un pianeta, in quel caso la vita va a zero direttamente
        if (collision.gameObject.layer == LayerMask.NameToLayer("Planet"))
        {
            await managerLife.TakeDamage(managerLife.GetCurrentLife());
        }
    }
}
