using UnityEngine;

public class MovingCursor : MonoBehaviour
{
    [Header("Riferimenti")]
    public Transform spaceship; // La nave nel mondo di gioco
    public Transform worldCenter; // L'Empty GameObject al centro della mappa vera

    [Header("Proporzione")]
    [Tooltip("Quante volte è più piccolo il modellino rispetto al mondo? (Es. 100)")]
    public float fattoreDiScala = 100f; 

    void Update()
    {
        if (spaceship == null || worldCenter == null) return;

        // 1. CALCOLO POSIZIONE
        // Quanto è lontana la nave dal centro del mondo? (In metri)
        Vector3 distanzaDalCentro = spaceship.position - worldCenter.position;

        // Rimpiccioliamo questa distanza usando il nostro fattore di scala
        // Se la nave è a 1000 metri, e la scala è 100, il cursore si sposterà di 10 metri.
        transform.localPosition = distanzaDalCentro / fattoreDiScala;

        // 2. CALCOLO ROTAZIONE
        // Siccome i nostri oggetti "scatola" sono puliti a 0,0,0, possiamo 
        // semplicemente copiare la rotazione spiaccicata identica!
        transform.localRotation = spaceship.rotation;
    }
}