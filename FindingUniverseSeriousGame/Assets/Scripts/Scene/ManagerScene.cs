using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.IO;

public class ManagerScene : MonoBehaviour
{
    /* LoadScene dovranno diventare LoadSceneAsync per evitare blocchi durante il caricamento */
    string MENU = "Menu";

    #region "unity cycle"
    private void OnEnable()
    {
        DelegateClass.SaveEventHandler += Save;
    }

    private void OnDisable()
    {
        DelegateClass.SaveEventHandler -= Save;
    }
    #endregion

    /// <summary>
    /// cambio livello dal menu
    /// </summary>
    public void ChangeLevel()
    {
        // bool messo a false perchè tecnicamente non sta cambiando livello
        PersistentSceneData.Instance.isChangingScene = false;

        //controllo per sicurezza
        Scene scenaAttuale = SceneManager.GetActiveScene();

        if (scenaAttuale.name == MENU)
        {
            SceneManager.LoadScene(LoadLevel());
        }
        else
            Debug.Log("non sei nel menu, attento a dove hai chiamato il metodo");

    }

    /// <summary>
    /// Cambio livello in base al livello passato
    /// </summary>
    /// <param name="levelToGo"></param>
    public void ChangeLevel(string levelToGo)
    {
        PersistentSceneData.Instance.isChangingScene = true;

        //salvo al cambio di scena
        DelegateClass.SaveEventHandler.Invoke(true);

        SceneManager.LoadScene(levelToGo);
    }

    /// <summary>
    /// Torno al menu, da chiamare insieme al bottone salvataggio
    /// </summary>
    public void ChangeToMenu()
    {
        SceneManager.LoadScene(MENU);
    }

    #region "salvataggi e caricamenti"
    private void Save(bool isChangigLevel)
    {
        string path = ManagerHandler.ManagerInstance.SaveManager.GetPathForScene();

        Scene scenaAttuale = SceneManager.GetActiveScene();

        //prendo scena aperta attuale
        string json = scenaAttuale.name;

        try
        {
            File.WriteAllText(path, json);
            Debug.Log("Scene salvata");
        }
        catch
        {
            Debug.Log("salvataggio non avvenuto");
        }
    }

    /// <summary>
    /// Carico il file json con il nome della scena corrente salvata precedentemente
    /// </summary>
    private string LoadLevel()
    {
        string path = ManagerHandler.ManagerInstance.SaveManager.GetPathForScene();

        string sceneLoaded = "";

        try
        {
            sceneLoaded = File.ReadAllText(path);
            Debug.Log("scena caricata");
        }
        catch
        {
            //di default gli passo la scena Level1 
            sceneLoaded = "Level1";
            Debug.Log("File scene.json non trovato");
        }

        return sceneLoaded;
    }
    #endregion
}
