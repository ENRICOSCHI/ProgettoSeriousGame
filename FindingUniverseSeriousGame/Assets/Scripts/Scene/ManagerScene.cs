using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerScene : MonoBehaviour
{
    string LEVEL1 = "Level1";
    string LEVEL2 = "Level2";
    string MENU = "Menu";

    public void ChangeLevel()
    {
        PersistentSceneData.Instance.isChangingScene = true;

        if(SceneManager.GetActiveScene().ToString() == LEVEL1)
            SceneManager.LoadScene(LEVEL2);
        else 
            SceneManager.LoadScene(LEVEL1);

    }

    public void ChangeToMenu()
    {
        /*implementare script per cambiare scena nel menu*/
    }
}
