using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerScene : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        PersistentSceneData.Instance.isChangingScene = true;
        SceneManager.LoadScene(sceneName);
    }
}
