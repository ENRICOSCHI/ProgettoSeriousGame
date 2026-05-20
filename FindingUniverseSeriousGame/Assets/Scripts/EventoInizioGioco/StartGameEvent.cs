using UnityEngine;

public class StartGameEvent : MonoBehaviour
{
    [SerializeField] AudioClip audioSottotitoli; 
    [SerializeField] Subtitles[] sottotitoli;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!PersistentSceneData.Instance.isStarSceneEventHappenend)
        {
            ManagerHandler.ManagerInstance.SubtitleManager.PlaySubtitle(sottotitoli, audioSottotitoli);
            PersistentSceneData.Instance.isStarSceneEventHappenend = true;
        }
    }
}
