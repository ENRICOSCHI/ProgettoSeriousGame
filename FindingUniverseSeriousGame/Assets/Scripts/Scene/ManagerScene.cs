using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.IO;

public class ManagerScene : MonoBehaviour
{
    /* LoadScene dovranno diventare LoadSceneAsync per evitare blocchi durante il caricamento */
    string MENU = "MainMenu";
    string LEVEL1 = "Level1";

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
    public string LoadLevel()
    {
        string path = ManagerHandler.ManagerInstance.SaveManager.GetPathForScene();

        string sceneLoaded = "";

        try
        {
            sceneLoaded = File.ReadAllText(path);
            SceneManager.LoadScene(sceneLoaded);
            Debug.Log("scena caricata");
        }
        catch
        {
            //di default gli passo la scena Level1 
            sceneLoaded = "Level1";
            SceneManager.LoadScene(sceneLoaded);
            Debug.Log("File scene.json non trovato");
        }

        return sceneLoaded;
    }


    /// <summary>
    /// Nuova partita, cancellando i file JSON presenti da salvataggi precedenti
    /// </summary>
    public void NewGame()
    {
        //cancello i file JSON presenti da salvataggi precedenti
        string pathPlayer = ManagerHandler.ManagerInstance.SaveManager.GetPathForPlayer();
        string pathCodex = ManagerHandler.ManagerInstance.SaveManager.GetPathForCodex();
        string pathMission = ManagerHandler.ManagerInstance.SaveManager.GetPathForMission();
        string pathPersistentSceneData = ManagerHandler.ManagerInstance.SaveManager.GetPathForPersistentSceneData();
        string pathScene = ManagerHandler.ManagerInstance.SaveManager.GetPathForScene();

        if (File.Exists(pathPlayer)) File.Delete(pathPlayer);
        if (File.Exists(pathCodex)) File.Delete(pathCodex);
        if (File.Exists(pathMission)) File.Delete(pathMission);
        if (File.Exists(pathPersistentSceneData)) File.Delete(pathPersistentSceneData);
        if (File.Exists(pathScene)) File.Delete(pathScene);

        //carico la scena del menu
        SceneManager.LoadScene(LEVEL1);
    }
    
    #endregion
}
