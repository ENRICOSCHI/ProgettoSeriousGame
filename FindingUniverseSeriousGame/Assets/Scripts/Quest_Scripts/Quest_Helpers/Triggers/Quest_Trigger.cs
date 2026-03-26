using UnityEngine;

public class Quest_Trigger : MonoBehaviour
{
    [SerializeField] private KeyCode interactionKey; //tasto da premere per attivare la quest

    private bool isPlayerInside = false;
    private Quest_Generic_Script questType;

    void Update()
    {
        CheckInput();
    }

    void OnTriggerEnter(Collider other)
    {
        isPlayerInside = true;
        questType = other.GetComponent<Quest_Generic_Script>();
        if(questType.questInteractionType == interactableType.scan)
        {
            Tipo2();
        }
    }

    void OnTriggerExit(Collider other)
    {
        isPlayerInside = false;
    }

    void CheckInput()
    {
        if (isPlayerInside && Input.GetKeyDown(interactionKey) && questType.questInteractionType == interactableType.keyWord)
        {
            Tipo3();
        }
    }

    #region "TIPO QUEST 2"
    /// <summary>
    /// Missione che applica uno scan a uno oggetto, se l'oggetto fa parte della missione, quest0ultima viene completata
    /// </summary>
    void Tipo2()
    {
        if (questType is Quest_2_Script)
        {
            // prendo lo script quest 2 e poi avvio il metodo
            Quest_2_Script q2 = questType as Quest_2_Script;
            q2.FinishQuest();
            Debug.Log("Trigger attivato (Quest 2)");

            //other.GetComponent<Quest_2_Script>().ActivateQuest();  
        }
    }
    #endregion
    #region "TIPO QUEST 3"
    /// <summary>
    /// Missione che richiede la raccolta di N oggetti
    /// </summary>
    void Tipo3()
    {
        if(questType is Quest_3_Script)
        {
            if (!questType.questStarted)
            {
                questType.StartQuest();
            }

            Debug.Log("Trigger attivato (Quest 3)");
            Quest_3_Script q3 = questType as Quest_3_Script;

            if (questType.questStarted && q3.doesReturnInPlace)
            {
                q3.FinishQuest();
            }
            //other.GetComponent<Quest_3_Script>().ActivateQuest();
        }
    }
    #endregion
}
