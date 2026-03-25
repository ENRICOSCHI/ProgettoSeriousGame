using UnityEngine;

public class Quest_3_Script : Quest_Generic_Script
{
    // Template per Fetch quest di raccolta di n oggetti.
    public bool doesReturnInPlace = false; //indica se gli oggetti da raccogliere devono essere riportati in un punto specifico (true) o semplicemente raccolti (false)

    [Header("Quest Assets")]
    [SerializeField] GameObject[] questItem; //Lista di riferimenti agli oggetti da tocccare / raccogliere

    private int currentAmount = 0; //quantità attuale di oggetti raccolti
    private int requiredAmount; //quantità richiesta di oggetti raccolti per completare la quest | da settare


    private void Awake()  // Ottiene quanti elementi sono presenti nell'array questItem
    {
        requiredAmount = questItem.Length; //imposto la quantità richiesta in base alla lunghezza dell'array di oggetti da raccogliere
    }

    public void ItemCollected()
    {
        if (!questStarted || questCompleted) return;

        currentAmount++;
        if (!doesReturnInPlace)
        {
            FinishQuest();  //logica per verificare se gli oggetti sono stati riportati in un punto specifico
        }
    }

    /*public override void Start()
    {
        base.Start();
        currentAmount = QuestManager_Script.instance.GetQuestData(questName).amountProgress;
    }*/

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
                    if (item.GetComponent<Collectable_Item_Quest3>() != null)
                    {
                        item.GetComponent<Collectable_Item_Quest3>().SetQuestScript(this);
                    }
                    else Debug.LogWarning("Riferimento a Collectable_Item mancante per l'oggetto " + item.name + " nella lista questItem per la Quest 3 in: " + gameObject.name);
                }
                else Debug.LogWarning("Riferimento a oggetto mancante nella lista questItem per la Quest 3 in: " + gameObject.name);
            }
            base.StartQuest();
        }
        else Debug.LogWarning("Riferimenti a oggetti mancanti o lista vuota per la Quest 3 in: " + gameObject.name);
    }

    public override void FinishQuest()
    {
        if (currentAmount < requiredAmount)
            return;

        base.FinishQuest();

        ManagerHandler.ManagerInstance.NotificationManager.ShowMessage("Quest 3 terminata.");
        Debug.Log("Quest 3 terminata");
        //QuestManager_Script.instance.UpdateQuestData(questName, questStarted, questCompleted, currentAmount);

        this.enabled = false;
    }
}
