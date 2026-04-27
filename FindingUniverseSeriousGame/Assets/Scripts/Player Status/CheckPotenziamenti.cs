using UnityEngine;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Il presente script è pensato per essere attaccato al gameObject del potenziamento.
/// </summary>
public class CheckPotenziamenti : MonoBehaviour
{
    [Tooltip("Lista di ID di missioni che è necessario completare per sbloccare il potenziamento.")]
    [SerializeField] List<string> targetMissions;

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
        if (categoryMissions.Length == 0 || categoryMissions == null)
        {
            Debug.LogWarning($"Al potenziamento {gameObject.name} non è stato passato" +
                $"un Dictionary di riferimento per le missioni valido.");
            return false;
        }

        //Caso in cui targetMissions è vuota.
        if (targetMissions.Count == 0 || targetMissions == null)
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
    private void SbloccaPotenziamento(CategoryMission[] categoryMissions)
    {
        if (CheckPotenziamento(categoryMissions))
        {
            this.gameObject.SetActive(true); //Attivo visivamente il potenziamento
            this.enabled = false; // Disabilita lo script dopo aver sbloccato il potenziamento
        }
    }

    #endregion
}
