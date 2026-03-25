using UnityEngine;

public class Quest_Trigger : MonoBehaviour
{

    private bool isPlayerInside = false;
    [SerializeField] private KeyCode interactionKey; //tasto da premere per attivare la quest
    private Quest_Generic_Script questType;

    void Update()
    {
        CheckInput();
    }

    void OnTriggerEnter(Collider other)
    {
        isPlayerInside = true;
        questType = other.GetComponent<Quest_Generic_Script>();

        if (questType is Quest_2_Script)
        {
            other.GetComponent<Quest_2_Script>().FinishQuest();
            Debug.Log("Trigger attivato (Quest 2)");

            //other.GetComponent<Quest_2_Script>().ActivateQuest();  
        }

    }

    void OnTriggerExit(Collider other)
    {
        isPlayerInside = false;
    }

    void CheckInput()
    {
        if (isPlayerInside && Input.GetKeyDown(interactionKey) && questType is Quest_3_Script)
        {
            if(!questType.questStarted)
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
}
