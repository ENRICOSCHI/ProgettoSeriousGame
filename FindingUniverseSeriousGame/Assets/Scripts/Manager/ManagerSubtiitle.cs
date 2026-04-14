using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManagerSubtiitle : MonoBehaviour
{
    //[SerializeField] TextMeshProUGUI textUISubtitle;
    [SerializeField] AudioSource audioSourceSubtitles;
    public IEnumerator PlaySubtitle(Subtitles[] subtitles,AudioClip audioClipOfSubtitle)
    {
        if(audioClipOfSubtitle != null)
        {
            audioSourceSubtitles.clip = audioClipOfSubtitle;
            audioSourceSubtitles.Play();
        }
            

        foreach(Subtitles s in subtitles)
        {
            yield return StartCoroutine(ShowSubtitle(s));
        }
    }

    IEnumerator ShowSubtitle(Subtitles subtitle)
    {
        float durationClip = subtitle.timeEnd - subtitle.timeStart;
        ManagerHandler.ManagerInstance.DialogueManager.ShowMessageForSubtitle(subtitle.phrase, durationClip);
        yield return new WaitForSeconds(durationClip);
    }
}
