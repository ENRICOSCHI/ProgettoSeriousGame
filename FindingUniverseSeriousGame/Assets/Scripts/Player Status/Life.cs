using System.Threading.Tasks;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class Life : MonoBehaviour
{

    #region Inizializzazione variabili

    // Riferimento allo script di movimento della navicella
    [SerializeField] MovimentoNavicella movimentoNavicella;
    [SerializeField] AudioClip beginningSound;


    [Header("Configurazione Danno")]
    public float baseDamage = 10f;  // Verrà poi modificato da un moltiplicatore in base alla velocità

    #endregion



    private void Start()
    {
        ManagerHandler.ManagerInstance.SFXManager.PlaySoundEffect(beginningSound, MovimentoNavicella.GetNavicellaTransform(), 1.0f);
    }

    
    // Controllo di collisione tramite tag (espandibile)
    private async Awaitable OnCollisionEnter(Collision collision)
    {
        // Controllo dei tag, espandibile a piacere in base a quanti tag abbiamo
        if(collision.gameObject.CompareTag("Debris") ||
           collision.gameObject.CompareTag("Satellite"))
        {
            await DamageApplier();
        }

        //Morte istantanea se si collide con un pianeta
        if (collision.gameObject.CompareTag("Pianeta"))
        {
            await ManagerHandler.ManagerInstance.LifeManager.TakeDamage(ManagerHandler.ManagerInstance.LifeManager.GetCurrentLife());
        }
    }


    //Formula di danno: baseDamage * moltiplicatore
    //Logica moltiplicatore: da 0.5 a 2
    private async Task DamageApplier()  
    {

        // Recupero del Manager tramite il Singleton
        var handler = ManagerHandler.ManagerInstance;


        // Ottenimento di velocità attuale e cap massimo
        float currentSpeed = movimentoNavicella.currentSpeed;
        float maxSpeed = movimentoNavicella.maxSpeed;

        // Calcolo del moltiplicatore di danno
        // Rapporto velocità: 0 = 0.5x, maxSpeed = 2.0x
        float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);
        float multiplier = 0.5f + (speedRatio * 1.5f);

        float finalDamage = baseDamage * multiplier;

        // Applicazione del danno
         await handler.LifeManager.TakeDamage(finalDamage);

        Debug.Log($"Impatto! Velocità: {currentSpeed:F1}. Moltiplicatore: {multiplier:F2}. Danno: {finalDamage:F1}");
    }
}
