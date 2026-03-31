using UnityEngine;

public class Quest_Trigger : MonoBehaviour
{
    [SerializeField] private KeyCode interactionKey; //tasto da premere per attivare la quest

    private bool isPlayerInside = false;  //Indica se il player è dentro la zona di quest
    private Quest_Generic_Script questType;  //Tipo di quest

    void Update()
    {
        CheckInput();
    }

    void CheckInput()  //Controllo pressione tasto di interazione quest
    {
        if (isPlayerInside && Input.GetKeyDown(interactionKey) && questType.questInteractionType == interactableType.keyWord)
        {
            Tipo3();
        }
    }


    #region Metodi di controllo ingresso / uscita nella zona quest

    void OnTriggerEnter(Collider other)
    {
        questType = other.GetComponent<Quest_Generic_Script>();
        if (questType != null)
        {
            isPlayerInside = true;
            
            if (questType.questInteractionType == interactableType.scan)
            {
                Tipo2();
            }
            else
            {
                Debug.Log("Premi E per interagire con la quest");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        isPlayerInside = false;
    }

    #endregion


    #region "TIPO QUEST 2"
    /// <summary>
    /// Missione che applica uno scan a uno oggetto, se l'oggetto fa parte della missione, quest0ultima viene completata
    /// </summary>
    void Tipo2()
    {
        if (questType is Quest_2_Script && questType.isActiveAndEnabled)
        {
            // prendo lo script quest 2 e poi avvio il metodo
            Quest_2_Script q2 = questType as Quest_2_Script;
            q2.FinishQuest();
            Debug.Log("Trigger attivato (Quest 2)");

        }
    }
    #endregion


    #region "TIPO QUEST 3"
    /// <summary>
    /// Missione che richiede la raccolta di N oggetti
    /// </summary>
    void Tipo3()
    {
        if (questType is Quest_3_Script && questType.isActiveAndEnabled)
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
        }
    }
    #endregion
}
