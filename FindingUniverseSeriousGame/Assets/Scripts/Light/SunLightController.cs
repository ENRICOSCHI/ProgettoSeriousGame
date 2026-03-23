using UnityEngine;

/// <summary>
/// Gestisce l'illuminazione direzionale dinamica per un sistema solare.
/// Ruota la luce in modo che punti sempre dal centro (Sole) verso la navicella,
/// simulando una sorgente a 360 gradi in modo estremamente ottimizzato.
/// </summary>
[ExecuteInEditMode]  // Permette di vedere l'effetto in tempo reale anche in modalità Editor
public class SunLightController : MonoBehaviour
{
    // ─── Riferimenti ─────────────────────────────────────────────────────────

    [Header("Configurazione Obiettivo")]
    [Tooltip("La navicella del giocatore o la camera principale.")]
    [SerializeField] private Transform _target;

    [Tooltip("Il centro del Sole da cui esce la luce")]
    [SerializeField] private Vector3 _sunCenter = Vector3.zero;

    // ─── Lifecycle ───────────────────────────────────────────────────────────

    private void Update()
    {
        if (_target == null) return;

        AggiornaDirezioneLuce();
    }

    // ─── Logica di Illuminazione ─────────────────────────────────────────────

    /// <summary>
    /// Calcola il vettore direzione dal Sole al Player e orienta la luce.
    /// Questo garantisce che gli oggetti vicino al player siano sempre illuminati
    /// coerentemente con la posizione della stella.
    /// </summary>
    private void AggiornaDirezioneLuce()
    {
        // Calcolo del vettore direzione: Destinazione - Origine
        Vector3 direzioneLuce = (_target.position - _sunCenter).normalized;

        // Se la direzione è valida, orientiamo il transform della luce
        if (direzioneLuce != Vector3.zero)
        {
            // La luce direzionale illumina lungo il suo asse Z (forward)
            // Usiamo LookAt per puntare la luce nella stessa direzione del vettore
            transform.forward = direzioneLuce;
        }
    }

    // ─── Debug ───────────────────────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_sunCenter, _target != null ? _target.position : Vector3.up * 10f);
    }
}