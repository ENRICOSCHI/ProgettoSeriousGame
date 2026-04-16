using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider))]
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

    /// <summary>
    /// Controlli di sicurezza per assicurare che i riferimenti a questItem siano corretti e che i GameObject siano presenti.
    /// </summary>
    void OnValidate()
    {
        BoxCollider bc = GetComponent<BoxCollider>();
        if(bc != null)
            bc.isTrigger = true;
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
        InfoQuestUpdate();
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
        #region Controlli di sicurezza
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
        #endregion

        foreach (GameObject item in questItem)
        {
            //abilito l'oggetto se è disabilitato
            if (!item.activeInHierarchy) item.SetActive(true);
            item.GetComponent<Collectable_Item_Quest3>().SetQuestScript(this);

        }

        ManagerHandler.ManagerInstance.NotificationManager.ShowNotifcation("Raccogli " + questItem.Length + " oggetti per completare la quest: " + questName,notificationColor);
        
        base.StartQuest();
    }

    void InfoQuestUpdate()
    {
        if(currentAmount < requiredAmount)
            ManagerHandler.ManagerInstance.NotificationManager.ShowNotifcation("Ti mancano " + (requiredAmount - currentAmount) + " oggetti per completare la quest: " + questName,notificationColor);
        else
            ManagerHandler.ManagerInstance.NotificationManager.ShowNotifcation("Hai raccolto tutti gli oggetti! " + (doesReturnInPlace ? "Ritorna al punto della quest per completarla." : ""),notificationColor); // se non devo tornare indietro ci pensera finish questa a dire che è completata.
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

        ManagerHandler.ManagerInstance.NotificationManager.ShowNotifcation(questName + " terminata.", notificationColor);
        ManagerHandler.ManagerInstance.CodexManager.UnlockMenuEntry(indiceCategory, indiceEntry);
        Debug.Log(questName + " terminata");
        //QuestManager_Script.instance.UpdateQuestData(questName, questStarted, questCompleted, currentAmount);

        this.enabled = false;
    }

    #endregion
}
