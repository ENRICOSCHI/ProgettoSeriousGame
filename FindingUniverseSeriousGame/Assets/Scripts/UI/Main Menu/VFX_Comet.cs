using UnityEngine;

public class VFX_Cometa : MonoBehaviour
{
    [Header("Impostazioni Scia Cometa")]
    [Tooltip("Quante particelle spara al secondo?")]
    [SerializeField] private float quantitaEmissione = 60f;
    
    [Tooltip("A che velocità partono le particelle?")]
    [SerializeField] private float velocitaParticelle = 10f;

    // ECCO LA MAGIA PER LA LUNGHEZZA DELLA CODA
    [Tooltip("Quanto è lunga la coda? (Secondi di vita di ogni particella prima di svanire)")]
    [SerializeField] private float lunghezzaCoda = 2f; 

    private ParticleSystem sciaParticle;

    private void Awake()
    {
        sciaParticle = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        if (sciaParticle != null)
        {
            var flareMain = sciaParticle.main;
            var flareEmission = sciaParticle.emission;

            flareEmission.enabled = true;
            flareEmission.rateOverTime = quantitaEmissione;
            flareMain.startSpeed = velocitaParticelle;
            
            // Applichiamo la lunghezza della coda
            flareMain.startLifetime = lunghezzaCoda;

            sciaParticle.Play();
        }
    }
}