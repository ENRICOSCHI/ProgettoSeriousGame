using UnityEngine;

public class StartGameEvent : MonoBehaviour
{
    [SerializeField] AudioClip audioSottotitoli; 
    [SerializeField] Subtitles[] sottotitoli;

    private void OnEnable()
    {
        DelegateClass.StartFirstMissionEventHandler += ActiveStartMission;
    }

    private void OnDisable()
    {
        DelegateClass.StartFirstMissionEventHandler -= ActiveStartMission;
    }

    void ActiveStartMission()
    {
        if (!PersistentSceneData.Instance.isStarSceneEventHappenend)
        {
            PersistentSceneData.Instance.isStarSceneEventHappenend = true;
            StartCoroutine(ManagerHandler.ManagerInstance.SubtitleManager.PlaySubtitle(sottotitoli, audioSottotitoli));
        }
    }
}
