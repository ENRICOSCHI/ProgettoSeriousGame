using UnityEngine;

/// <summary>
/// Script per quest di raccolta di n oggetti
/// </summary>
public class Quest_3_Script : Quest_Generic_Script
{

    #region Configurazione variabili

    public bool doesReturnInPlace = false;
    //true = raccolta oggetti e ritorno al punto della quest
    //false = solo raccolta oggetti

    [Header("Quest Assets")]
    [Tooltip("Lista di riferimenti ai GameObject da far raccogliere")]
    [SerializeField] GameObject[] questItem;
    private int currentAmount = 0;
    private int requiredAmount; //settato in Awake()

    #endregion

    #region Inizializzazione via Awake()

    /// <summary>
    /// Fa combaciare <see cref="requiredAmount"/> con la dimensione di <see cref="questItem"/>
    /// </summary>
    private void Awake()
    {
        requiredAmount = questItem.Length;
    }

    #endregion

    #region Gestione Logica Raccolta Oggetti

    /// <summary>
    /// Metodo che aggiorna il contatore relativo agli oggetti raccolti.
    /// Pensato per essere chiamato dagli oggetti presenti nell'Array questItem 
    /// al momento della loro raccolta.
    /// </summary>
    public void ItemCollected()
    {
        if (!questStarted || questCompleted) return;  //Controllo di sicurezza, quest non ancora terminata

        currentAmount++;
        if (!doesReturnInPlace)
        {
            FinishQuest();
        }
    }

    #endregion

    #region Override StartQuest() & FinishQuest()

    /*public override void Start()
    {
        base.Start();
        currentAmount = QuestManager_Script.instance.GetQuestData(questName).amountProgress;
    }*/

    /// <summary>
    /// Override del metodo base StartQuest(), adattato alla logica delle Quest di tipo 3. 
    /// Passa un riferimento alla propria istanza a tutti gli elementi presenti nella lista
    /// <see cref="questItem"/> e li abilita nella Hierarchy di Unity.
    /// Chiama infine lo StartQuest() base.
    /// </summary>
    public override void StartQuest()
    {
        if (!QuestSafetyChecks.CheckICollectable(questItem))
        {
            Debug.Log("Riferimenti interni a uno script ICollectable" +
            " nella lista di raccoglibili per la Quest 3 assenti o impropri in " +
            gameObject.name);
            return;
        }

        if (!QuestSafetyChecks.CheckGameObjectArray(questItem))
        {
            Debug.Log("Lista di GameObject da raccogliere" +
            " non impostata corretamente per la Quest 3 in " + gameObject.name);
            return;
        }

        foreach (GameObject item in questItem)
        {

            //abilito l'oggetto se è disabilitato
            if (!item.activeInHierarchy) item.SetActive(true);
            item.GetComponent<Collectable_Item_Quest3>().SetQuestScript(this);

        }
        base.StartQuest();
    }

    /// <summary>
    /// Override del metodo base FinishQuest() adattato alla logica delle Quest di tipo 3.
    /// Viene effettuato un controllo sulla quantità attuale di Item raccolti: 
    /// se questa soddisfa la quantità richiesta indicata da <see cref="requiredAmount"/> è chiamato
    /// il FinishQuest() base. (i.e. la quest è terminata)
    /// </summary>
    /// <remarks>
    /// Al termine dell'esecuzione <see cref="Quest_3_Script"/> è disabilitato 
    /// </remarks>
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

    #endregion
}
