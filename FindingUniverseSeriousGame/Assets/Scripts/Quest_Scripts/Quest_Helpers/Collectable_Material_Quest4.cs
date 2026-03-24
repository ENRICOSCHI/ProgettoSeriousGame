using UnityEngine;

public class Collectable_Material_Quest4 : MonoBehaviour, ICollectable
{
    /*Questo Script va allocato nei component dei materiali da raccogliere per la Quest 4.
    Che precedentemente ricordo vanno inseriti nell'Array repairMaterials (in riferimento a Quest_4_Script)*/
    [SerializeField] Quest_4_Script questScript;

    #region ICollectable Implementation
    public void OnCollect()
    {
        if (questScript != null)
        {
            questScript.MaterialCollected();
            gameObject.SetActive(false);
        }
    }
    #endregion

    /// <summary>
    /// SetQuestScript prende in ingresso un riferimento allo Script della Quest 4
    /// a cui il singolo Materiale dovrà fare riferimento.
    /// In particolare va chiamato nel relativo Quest_4_Script (in StartQuest()).
    /// </summary>
    /// <param name="script"></param>
    public void SetQuestScript(Quest_4_Script script)
    {
        questScript = script;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnCollect();
        }
    }
}
