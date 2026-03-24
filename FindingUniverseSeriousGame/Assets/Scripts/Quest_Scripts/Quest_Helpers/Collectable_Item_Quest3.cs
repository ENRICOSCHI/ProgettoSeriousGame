using UnityEngine;

public class Collectable_Item_Quest3 : MonoBehaviour, ICollectable
{
    /*Script pensato in maniera specifica per aiutare l'esecuzione
    delle Quest di tipo 3, Fetch Quest a n Oggetti.
    Il presente Script va allocato nei Component degli Oggetti da raccogliere
    presneti nell'Array questItem (in riferimento a Quest_3_Script)*/

    [SerializeField] Quest_3_Script questScript; //riferimento allo Script della Quest 3 a cui questo oggetto è associato.

    #region ICollectable Implementation
    public void OnCollect()
    {
        if (questScript != null)
        {
            questScript.ItemCollected();
            gameObject.SetActive(false);
        }
    }
    #endregion
    
    /// <summary>
    /// SetQuestScript prende in ingresso un riferimento allo Script
    /// della Quest 3 a cui il singolo oggetto sarà associato.
    /// In particolare va chiamato nel relativo Quest_3_Script (in StartQuest())
    /// per ogni oggetto dell'array questItem.
    /// </summary>
    /// <param name="script"></param>
    public void SetQuestScript(Quest_3_Script script)
    {
        questScript = script;
    }

    private void OnTriggerEnter(Collider other)
    {
        /*uso del tag "Player" per identificare il giocatore, cambiarlo
        di conseguenza se si usa un tag diverso!*/
        if (other.CompareTag("Player"))
        {
            OnCollect();
        }
    }

}
