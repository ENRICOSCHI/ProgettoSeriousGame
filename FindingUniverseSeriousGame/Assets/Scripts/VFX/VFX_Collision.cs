using UnityEngine;

public class VFX_Collision : MonoBehaviour
{
    [Header("Riferimenti")]
    [Tooltip("Trascina qui lo script che gestisce i danni della nave")]
    [SerializeField] private ManagerLife managerLife;

    [Tooltip("Trascina qui il PREFAB del Particle System dell'impatto esplosivo")]
    [SerializeField] private ParticleSystem impactVfx;

    // 1. Quando questo script si accende, si abbona all'evento tramite il simbolo +=
    void OnEnable()
    {
        if (managerLife != null)
        {
            managerLife.Collision += PlayImpactVFX;
        }
    }

    // 2. Quando si spegne, cancella l'abbonamento 
    void OnDisable()
    {
        if (managerLife != null)
        {
            managerLife.Collision -= PlayImpactVFX;
        }
    }

    /// <summary>
    /// Metodo che viene chiamato quando la nave subisce una collisione, riceve la posizione dell'impatto e genera un effetto visivo di esplosione in quel punto
    /// </summary>
    /// <param name="hitPoint"></param>
    private void PlayImpactVFX()
    {
        Debug.Log("Collisione rilevata! Generazione VFX di impatto.");
        if (impactVfx != null)
        {
           // Se l'effetto stava già andando, lo stoppiamo e lo facciamo ripartire
            impactVfx.Stop(); 
            impactVfx.Play(); 
        }
    }
}