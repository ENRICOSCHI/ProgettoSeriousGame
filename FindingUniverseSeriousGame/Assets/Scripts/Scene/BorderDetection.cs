using UnityEngine;

public class BorderDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PersistentSceneData.Instance.isChangeSceneUnlocked)
                ManagerHandler.ManagerInstance.SceneManager.ChangeLevel();
            else
            {
                Debug.Log("non hai ancora sbloccato il livello 2");
                /*Vedere se aggiungere il respawn in un punto prestabilito*/
            }
                
        }
    }
}
