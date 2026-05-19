using UnityEngine;

/// <summary>
/// Gestisce l'effetto effettivo del potenziamento una volta sbloccato o raccolto.
/// </summary>
public class EnhancementEffect : MonoBehaviour
{

    // Definisco le voci che appariranno nel menu a tendina dell'Inspector
    public enum EnhancementType
    {
        Armor,
        Speed1,
        Speed2,
        MagneticShield,
        RadiationShield,
        Music,
    }

    [Header("Configurazione Effetto")]
    [Tooltip("Seleziona dal menu a tendina quale funzione attivare per questo potenziamento.")]
    [SerializeField] private EnhancementType chosenEffect;

    /// <summary>
    /// Questo è il metodo principale da chiamare quando vuoi che il potenziamento faccia effetto
    /// </summary>
    public void ActivateEffect()
    {
        // Lo switch controlla cosa hai scelto nel menu a tendina e chiama la funzione giusta
        switch (chosenEffect)
        {
            case EnhancementType.Armor:
                Armor();
                break;

            case EnhancementType.Speed1:
                Speed1();
                break;

            case EnhancementType.Speed2:
                Speed2();
                break;

            case EnhancementType.MagneticShield:
                MagneticShield();
                break;

            case EnhancementType.RadiationShield:
                RadiationShield();
                break;

            case EnhancementType.Music:
                Music();
                break;

            default:
                Debug.LogWarning($"Attenzione: Al potenziamento {gameObject.name} non è stato assegnato nessun effetto dal menu a tendina.");
                break;
        }
    }

    #region Funzioni dei Potenziamenti

    private void Armor()
    {
        Debug.Log("Effetto applicato: Riduzione danni subiti!");
        
        //Riduzione del danno base subito del 50%
        DatabaseEnhancement.Instance.lifeScript.baseDamage /= 2f;
    }

    private void Speed1()
    {
        Debug.Log("Effetto applicato: Cap velocità alzato!");

        //Aumento del cap di velocità
        DatabaseEnhancement.Instance.movimentoNavicella.maxSpeed *= 2f;
    }

    private void Speed2()
    {
        Debug.Log("Effetto applicato: Cap velocità alzato!");

        //Aumento del cap di velocità
        DatabaseEnhancement.Instance.movimentoNavicella.maxSpeed *= 2f;
    }

    private void MagneticShield()
    {
        Debug.Log("Effetto applicato: Scudo Magnetico Attivato!");

        //Disattivazione degli effetti dell'evento solare
        DatabaseEnhancement.Instance.ventoSolareEvento.enabled = false;
    }

    private void RadiationShield()
    {
        PersistentSceneData.Instance.isChangeSceneUnlocked = true;
        Debug.Log("Effetto applicato: Scudo Antiradiazioni Attivato!");
    }

    private void Music()
    {
        DatabaseEnhancement.Instance.radioController.enabled = true;
        DatabaseEnhancement.Instance.musicUI.SetActive(true);
        Debug.Log("Effetto applicato: Musica Sbloccata!");
    }

    #endregion
}