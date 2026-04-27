using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerScene : MonoBehaviour
{
    string LEVEL1 = "Level1";
    string LEVEL2 = "Level2";
    string MENU = "Menu";

    /// <summary>
    /// Cambio livello in base al livello corrente
    /// </summary>
    public void ChangeLevel()
    {
        PersistentSceneData.Instance.isChangingScene = true;

        if(SceneManager.GetActiveScene().ToString() == LEVEL1)
            SceneManager.LoadScene(LEVEL2);
        else 
            SceneManager.LoadScene(LEVEL1);

    }

    /// <summary>
    /// Torno al menu
    /// </summary>
    public void ChangeToMenu()
    {
        /*implementare script per cambiare scena verso menu*/
    }
}
