using UnityEngine;
using UnityEngine.UI; // Se hai riferimenti UI
using TMPro;

/// <summary>
/// Contiene tutti i riferimenti grafici e di sistema necessari ai potenziamenti.
/// Da posizionare su un GameObject "Manager" sempre attivo nella scena.
/// </summary>
public class DatabaseEnhancement : MonoBehaviour
{
    public static DatabaseEnhancement Instance { get; private set; }

    [Header("Oggetti per missioni target")]
    [Tooltip("Trascina qui il lifeManager, se il potenziamento ha effetti sulla vita del player.")]
    public ManagerLife lifeManager;
    public Life lifeScript;

    [Tooltip("Trascina qui il movimentoNavicella, se il potenziamento ha effetti sulla velocità della navicella.")]
    public MovimentoNavicella movimentoNavicella;

    [Tooltip("Trascina qui l'EventoSolare, se il potenziamento ha effetti sull'evento solare.")]
    public VentoSolareEvento ventoSolareEvento;

    [Tooltip("Trascina qui il musicManager e la UI della musica, se il potenziamento ha effetti sul sistema musicale.")]
    public RadioController radioController;
    public GameObject musicUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}