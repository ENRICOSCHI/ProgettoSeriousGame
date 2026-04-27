using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManagerSubtiitle : MonoBehaviour
{
    //[SerializeField] TextMeshProUGUI textUISubtitle;
    [SerializeField] AudioSource audioSourceSubtitles;
    [SerializeField] AudioSource audioSourceMusic; 
    [SerializeField] float volumeMusicDuringSubtitle = 0.5f;

    private bool isSubtitlePlaying = false;
    private float oldVolumeMusic = 1f;

    /// <summary>
    /// Fa partire la riproduzione di un audio e mostra i sottotitoli sincronizzati.
    /// È un IEnumerator quindi vuole start coroutine quando viene chiamato
    /// </summary>
    /// <param name="subtitles"></param>
    /// <param name="audioClipOfSubtitle"></param>
    /// <returns></returns>
    public IEnumerator PlaySubtitle(Subtitles[] subtitles,AudioClip audioClipOfSubtitle)
    {
        //se i sottotitoli non stanno già suonando, prendo il volume attuale della musica per poterlo ripristinare alla fine dei sottotitoli
        if (!isSubtitlePlaying)
        {
            //prendo il volume della musica attuale
            oldVolumeMusic = audioSourceMusic.volume;
        }
        
        //abbasso il volume della musica per poter sentire meglio i sottotitoli
        audioSourceMusic.volume = volumeMusicDuringSubtitle;

        //faccio partire l'audio dei sottotitoli
        if(audioClipOfSubtitle != null)
        {
            audioSourceSubtitles.clip = audioClipOfSubtitle;
            audioSourceSubtitles.Play();
        }
            
        int subtitleLenght = subtitles.Length;

        //mostro i sottotitoli uno alla volta, aspettando la durata di ognuno prima di mostrare il successivo
        foreach(Subtitles s in subtitles)
        {
            isSubtitlePlaying = true;
            yield return StartCoroutine(ShowSubtitle(s,subtitleLenght));
        }
        //terminati i sottotitoli, rialzo il volume della musica
        audioSourceMusic.volume = oldVolumeMusic;
        isSubtitlePlaying = false;
    }

    IEnumerator ShowSubtitle(Subtitles subtitle, int numberSubtitles)
    {
        float durationClip = subtitle.timeEnd - subtitle.timeStart;
        ManagerHandler.ManagerInstance.DialogueManager.ShowMessageForSubtitle(subtitle.phrase, durationClip,numberSubtitles,subtitle.subtitleID);
        yield return new WaitForSeconds(durationClip);
    }
}
