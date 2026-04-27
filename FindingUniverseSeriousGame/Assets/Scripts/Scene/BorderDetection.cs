using UnityEngine;

public class BorderDetection : MonoBehaviour
{
    [Header("Transform variables")]
    [SerializeField] Transform spawnRedirect;
    [SerializeField] Transform navicellaTransform;

    [Header("Warnign variables")] 
    [SerializeField] string messageError = "Non puoi ancora andare al prossimo livello";
    [SerializeField] Color messageColor;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PersistentSceneData.Instance.isChangeSceneUnlocked)
                ManagerHandler.ManagerInstance.SceneManager.ChangeLevel();
            else
            {
                //messaggio d'avviso
                Debug.Log("non hai ancora sbloccato il livello 2");
                ManagerHandler.ManagerInstance.NotificationManager.ShowNotifcation(messageError, messageColor);
                //mettere sfondo nero per caricamento pos.
                // disattivare velocità navicella
                MovimentoNavicella MV = FindAnyObjectByType<MovimentoNavicella>();
                MV.SetCurrentSpeed(0f);
                // dare nuova posizione navicella
                navicellaTransform.position = spawnRedirect.position;
            }
                
        }
    }
}
