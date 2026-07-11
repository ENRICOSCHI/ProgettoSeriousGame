using UnityEngine;

public class BorderDetection : MonoBehaviour
{
    public enum BorderType { Esterno_Exit, Interno_Enter }

    [Header("Impostazioni Bordo")]
    [Tooltip("Scegli se il trigger scatta quando ESCI (Bordi Esterni) o quando ENTRI (Bordi Interni)")]
    [SerializeField] private BorderType tipoDiBordo = BorderType.Esterno_Exit;
    

    [Header("Riferimenti e Navigazione")]
    [SerializeField] string LEVELTOGO = "Level1";
    [SerializeField] Transform spawnRedirect;
    [SerializeField] Transform navicellaTransform;

    [Header("Variabili Avviso")] 
    [SerializeField] string messageError = "Non puoi ancora andare al prossimo livello";
    [SerializeField] Color messageColor = Color.red;

    // Scatta quando il giocatore entra nel trigger (per il trigger di bordo interno (ritorno al livello 1))
    private void OnTriggerEnter(Collider other)
    {
        if (tipoDiBordo == BorderType.Interno_Enter && other.CompareTag("Player"))
        {
            EseguiControlloCambioScena();
        }
    }

    // Scatta quando il giocatore esce dal trigger (per il trigger di bordo esterno (passaggio al livello successivo))
    private void OnTriggerExit(Collider other)
    {
        if (tipoDiBordo == BorderType.Esterno_Exit && other.CompareTag("Player"))
        {
            EseguiControlloCambioScena();
        }
    }

    // Centralizziamo la logica così non duplichiamo il codice
    private void EseguiControlloCambioScena()
    {
        
        if (PersistentSceneData.Instance.isChangeSceneUnlocked)
        {
            ManagerHandler.ManagerInstance.SceneManager.ChangeLevel(LEVELTOGO);
        }
        else
        {
            // Se non ho completato la missione, ritorno in uno spawn centrale...
            Debug.Log("Cambio scena bloccato.");
            ManagerHandler.ManagerInstance.NotificationManager.ShowNotifcation(messageError, messageColor);
            
            MovimentoNavicella MV = FindAnyObjectByType<MovimentoNavicella>();
            if (MV != null)
            {
                MV.SetCurrentSpeed(0f);
            }
            
            if (navicellaTransform != null && spawnRedirect != null)
            {
                navicellaTransform.position = spawnRedirect.position;
            }
        }
    }
}