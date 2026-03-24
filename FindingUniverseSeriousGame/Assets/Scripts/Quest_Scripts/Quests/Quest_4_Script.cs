using UnityEngine;
using UnityEngine.InputSystem;

public class Quest_4_Script : Quest_Generic_Script
{
    /*Il presente Script ha lo scopo di gestire una quest di tipo 4, nella quale bisogna 
    interagire con un satellite per ripararlo se e solo se si posseggono i materiali per farlo.*/

    /*NOTA IMPLEMENTATIVA: repairMaterials è mantenuto come GameObject[] per accesso diretto a SetActive().
    Attualmente usiamo casting sicuro (as) con ICollectable. Potrebbe essere refactorizzato a ICollectable[]
    in futuro aggiungendo un metodo Activate(bool) all'interfaccia, per maggiore polimorfismo.
    Oltre che a una stringa _name per poter identificare ogni materiale nella hierarchy.
    Stessa cosa si può fare per la quest 3.*/


    #region References Handling

    [Header("Quest References")]
    [SerializeField] Transform sateliteTransform; //Da agganciare al Transform del satellite da riparare
    [SerializeField] Transform player; //Da agganciare al Transform del Player
    [SerializeField] GameObject[] repairMaterials; /*Da agganciare a ogni Oggetto che deve
    essere considerato come materiale di riparazione*/

    #endregion

    #region Variables Handling

    [Header("Quest Parameters")]
    [SerializeField] int requiredMaterials;
    [SerializeField] int currentMaterials;
    [SerializeField] KeyCode interactionKey; /*Questo tasto è quello da premere per interagire con 
    il satellite, i materiali vengono raccolti "passandoci sopra"*/
    [SerializeField] float interactionDistance; //Tra player e satellite.

    #endregion

    #region Start & Update
    public override void Start()
    {
        base.Start();
        currentMaterials = QuestManager_Script.instance.GetQuestData(questName).amountProgress;
    }

    private void Update()
    {
        //Effetuo vari controlli di sicurezza.
        if (SafetyChecks()) return;

        float distancePlayerSatelite = Vector3.Distance
            (player.transform.position, sateliteTransform.position);

        CompletionCondition(distancePlayerSatelite);
    }
    #endregion

    #region Quest Strat & Finish Management
    public override void StartQuest()
    {
        if (SafetyChecks()) return;
        
        foreach (GameObject material in repairMaterials)
        {
            //Attivo l'oggetto se è disabilitato
            if (!material.activeInHierarchy) material.SetActive(true); 
            Collectable_Material_Quest4 collectableScript = material.GetComponent<Collectable_Material_Quest4>();
            if (collectableScript != null)
            {
                collectableScript.SetQuestScript(this);
            }
            else
            {
                Debug.LogWarning("Riferimento a Collectable_Material_Quest4 mancante su " + material.name + " per la Quest 4 in: " + gameObject.name);
            }
        }
        base.StartQuest();  
    }

    public override void FinishQuest()
    {
        base.FinishQuest();
        QuestManager_Script.instance.UpdateQuestData(questName, questStarted, questCompleted, currentMaterials);
    }
    #endregion

    #region Quest Flow Management

    /// <summary>
    /// SafetyChecks effettua una serie di controlli di sicurezza per evitare crush durante l'esecuzione.
    /// In particolare discrimina due tipi di errori: 
    /// quelli che impediscono completamente l'esecuzione della Quest (Errors) 
    /// e quelli che invece non ne impediscono l'esecuzione ma potrebbero causare 
    /// comportamenti anomali (Warnings).
    /// </summary>
    /// <returns></returns>
    private bool SafetyChecks()
    {
        #region Errors
        if (!questStarted || questCompleted) return true;
        if (sateliteTransform == null)
        {
            Debug.LogWarning("Riferimento a satelliteTransform mancante per la Quest 4 in: " + gameObject.name);
            return true;
        }
        if (player == null)
        {
            Debug.LogWarning("Riferimento a player mancante per la Quest 4 in: " + gameObject.name);
            return true;
        }
        if (repairMaterials == null)
        {
            Debug.LogWarning("Riferimenti a repairMaterials mancanti per la Quest 4 in: " + gameObject.name);
            return true;
        }
        #endregion

        #region Warnings
        if (requiredMaterials <= 0)
        {
            Debug.LogWarning("Valore di requiredMaterials minore o uguale a 0 per la Quest 4 in: " + gameObject.name);
            return false;
        }
        if (interactionDistance <= 0)
        {
            Debug.LogWarning("Valore di interactionDistance minore o uguale a 0 per la Quest 4 in: " + gameObject.name);
            return false;
        }
        if (interactionKey == KeyCode.None)
        {
            Debug.LogWarning("Nessuna Key assegnata per l'interazione per la Quest 4 in: " + gameObject.name);
            return false;
        }
        if (requiredMaterials > repairMaterials.Length)
        {
            Debug.LogWarning("Valore di requiredMaterials maggiore del numero di materiali disponibili per la Quest 4 in: " + gameObject.name);
            return false;
        }
        #endregion

        return false;
    }

    /// <summary>
    /// CompletitionCondition verifica se la distanza player-satellite è minore o uguale
    /// a quella passata in ingresso e contestualmente se è stato premuto il tasto di interazione. 
    /// Se entrambe le condizioni sono verificate e sono stati raccolti abbastanza materiali,
    /// la quest è completata.
    /// </summary>
    /// <param name="distancePlayerSatelite"></param>
    private void CompletionCondition(float distancePlayerSatelite)
    {
        if (distancePlayerSatelite <= interactionDistance)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                if (currentMaterials >= requiredMaterials)
                {
                    FinishQuest();
                }
                else
                {
                    //WIP: possibile implementazione di un elemento UI.
                    Debug.Log("Non abbastanza materiali per riparare il satellite! Materiali attuali: "
                        + currentMaterials + " (in: " + gameObject.name + ")");
                }
            }
        }
    }

    /// <summary>
    /// MaterialCollected è il metodo che va chiamato a ogni raccolta di un materiale da riparazione.
    /// Idealmente è utilizzato dal relativo Script Collectable_Material_Quest4.
    /// </summary>
    public void MaterialCollected()
    {
        if (SafetyChecks()) return;

        currentMaterials++;
        QuestManager_Script.instance.UpdateQuestData(questName, questStarted, questCompleted, currentMaterials);
    }
    #endregion
}
