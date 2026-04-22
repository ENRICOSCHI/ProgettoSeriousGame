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
            
        int subtitleLenght = subtitles.Length;
        foreach(Subtitles s in subtitles)
        {
            yield return StartCoroutine(ShowSubtitle(s,subtitleLenght));
        }
    }

    IEnumerator ShowSubtitle(Subtitles subtitle, int numberSubtitles)
    {
        float durationClip = subtitle.timeEnd - subtitle.timeStart;
        ManagerHandler.ManagerInstance.DialogueManager.ShowMessageForSubtitle(subtitle.phrase, durationClip,numberSubtitles);
        yield return new WaitForSeconds(durationClip);
    }
}
