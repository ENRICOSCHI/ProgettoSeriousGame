using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.VFX;

/// <summary>
/// Il presente script è pensato per essere attaccato al gameObject del potenziamento.
/// </summary>
public class CheckPotenziamenti : MonoBehaviour
{
    [Tooltip("Lista di ID di missioni che è necessario completare per sbloccare il potenziamento.")]
    [SerializeField] List<string> targetMissions;
    private MeshRenderer visualModel; // Riferimento al MeshRenderer del potenziamento, da assegnare in Start
    private EnhancementEffect enhancementEffect; // Riferimento allo script EnhancementEffect, da assegnare in Start
    Color notificationColor = Color.green;

    #region Gestione eventi

    private void OnEnable()
    {
        DelegateClass.UpdateQuestDataEventHandler += SbloccaPotenziamento;
    }

    private void OnDisable()
    {
        DelegateClass.UpdateQuestDataEventHandler -= SbloccaPotenziamento;
    }

    #endregion

    #region Metodi Unity
    private void Awake()
    {
         visualModel = GetComponent<MeshRenderer>();
         enhancementEffect = GetComponent<EnhancementEffect>();

    }
    #endregion

    #region Metodi per controllo e sblocco potenziamento

    /// <summary>
    /// Controlla se tutte le missioni definite da inspector come Target, risultano completate
    /// in un Dictionary di riferimento.
    /// </summary>
    /// <param name="missionDictionary">
    /// Idealmente è il Dictionary di riferimento che contiene le missioni già
    /// cominciate.
    /// </param>
    private bool CheckPotenziamento(CategoryMission[] categoryMissions)
    {
        #region Controlli di validità dei dati

        //Caso in cui categoryMissions è vuota.
        if (categoryMissions == null ||categoryMissions.Length == 0)
        {
            Debug.LogWarning($"Al potenziamento {gameObject.name} non è stato passato" +
                $"un Dictionary di riferimento per le missioni valido.");
            return false;
        }

        //Caso in cui targetMissions è vuota.
        if (targetMissions == null || targetMissions.Count == 0)
        {
            Debug.LogWarning($"Nel potenziamento {gameObject.name} non sono state assegnate" +
                $"missioni target.");
            return false;
        }

        #endregion

        #region Controllo sullo stato di completamento delle missioni Target

        /*Note sul comando che segue:
        * Ricordo che con .All di LINQ, quel "id" rappresenta ogni singolo elemento stringa
        della lista targetMissions.
        * Il susseguirsi di .All e .Any scandaglia l'intera struttura dati, ed è accettabile
        fintanto che si lavora con dimensioni non troppo grandi.*/
        
        return targetMissions.All(id => categoryMissions.Any(
            category => category.entries.Any(entry => entry.ID == id && entry.isCompleted)));

        #endregion  
    }

    /// <summary>
    /// Attiva il GameObject del potenziamento nella hirerchy, se tutte le missioni Target 
    /// risultano completate.
    /// Disabilita quindi lo script dopo aver sbloccato il potenziamento al fine di evitare 
    /// controlli inutili in futuro.
    /// </summary>
    private void SbloccaPotenziamento(CategoryMission[] categoryMissions, bool hasNotification)
    {
        if (CheckPotenziamento(categoryMissions))
        {
            visualModel.enabled = true; // Abilita il MeshRenderer per rendere visibile il potenziamento
            enhancementEffect.ActivateEffect(); // Attiva l'effetto del potenziamento
            
            Debug.Log($"Potenziamento {gameObject.name} sbloccato! Tutte le missioni target sono completate.");
            if (hasNotification)
                ManagerHandler.ManagerInstance.NotificationManager.ShowNotifcation($"Potenziamento {gameObject.name} sbloccato! Tutte le missioni target sono completate.", notificationColor);
            this.enabled = false; // Disabilita lo script dopo aver sbloccato il potenziamento
        }
    }

    #endregion
}
