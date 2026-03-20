using UnityEngine;

public class Quest_3_Script : Quest_Generic_Script
{
    // Template per Fetch quest di raccolta di n oggetti.

    [Header("Quest Assets")]
    [SerializeField] int requiredAmount; //quantità richiesta di oggetti raccolti per completare la quest | da settare
    [SerializeField] GameObject[] questItem; //Lista di riferimenti agli oggetti da tocccare / raccogliere

    private int currentAmount = 0; //quantità attuale di oggetti raccolti

    public void ItemCollected()
    {
        if (!questStarted || questCompleted) return;

        currentAmount++;
        if (currentAmount >= requiredAmount)
        {
            FinishQuest();
        }
    }

    public override void StartQuest()
    {
        if (questItem != null && questItem.Length > 0)
        {
            foreach (GameObject item in questItem)
            {
                if (item != null)
                {
                    //abilito l'oggetto se è disabilitato
                    if (!item.activeInHierarchy) item.SetActive(true);
                    if (item.GetComponent<Collectable_Item>() != null)
                    {
                        item.GetComponent<Collectable_Item>().SetQuestScript(this);
                    }
                    else Debug.LogWarning("Riferimento a Collectable_Item mancante per l'oggetto " + item.name + " nella lista questItem per la Quest 3 in: " + gameObject.name);
                }
                else Debug.LogWarning("Riferimento a oggetto mancante nella lista questItem per la Quest 3 in: " + gameObject.name);
            }
            base.StartQuest();
        }
        else Debug.LogWarning("Riferimenti a oggetti mancanti o lista vuota per la Quest 3 in: " + gameObject.name);
    }
}
