using System.Collections;
using UnityEngine;

public class VentoSolareEvento : Eventi
{
    [Header("Gestione tempo vento solare")]
    [SerializeField] int tempoMINIMO = 5;
    [SerializeField] int tempoMASSIMO = 15;
    [SerializeField] int durataEvento = 5;

    [Header("Sound Effects")]
    [SerializeField] AudioClip tempestaSFX;


    [ContextMenu("Test Evento Vento Solare")]
    void Test()
    {
        StartCoroutine(AttivaEvento(5));
    }


    void Start()
    {
        StartCoroutine(AvviaEventoInLoop());
    }

    /// <summary>
    /// Evento in loop che si attiva ogni tot secondi, 
    /// con un tempo random tra un minimo e un massimo, 
    /// e che attiva e disattiva l'effetto del vento solare
    /// </summary>
    /// <returns></returns>
    IEnumerator AvviaEventoInLoop()
    {
        while (true)
        {
            yield return AttivaEvento(Random.Range(tempoMINIMO, tempoMASSIMO));
        }
    }

    IEnumerator AttivaEvento(int secondiDelayEvento)
    {
        // aspetto un tempo di delay prima di attivare il prossimo evento solare
        yield return new WaitForSeconds(secondiDelayEvento);

        //se l'evento non è ancora stato scoperto...
        if (!ManagerHandler.ManagerInstance.CodexManager.categoryLists[1].entries[1].isDiscovered)
        {
            ActiveSubtitlesWithAudio();//attivo descrizione audio con sottotitoli
            UnlockOnCodexMenu(); //sblocco largomento nel codex
            PersistentSceneData.Instance.isDescriptionVentoSolareHappened = true; 
        }

        NotificaPersonalizzata(notificaMessaggio[0]); //notifico l'evento al giocatore
        //Avviso l'UI tramite delegate che l'evento solare è attivo e far partire quindi "l'intereferenza" visiva
        DelegateClass.VentoSolareEventsHandler?.Invoke(true);

        if (tempestaSFX != null)
            ManagerHandler.ManagerInstance.SFXManager.PlaySoundEffect(tempestaSFX, MovimentoNavicella.GetNavicellaTransform(), 1f);
        else
            Debug.LogWarning("Manca tempestaSFX in VentoSolareEvento.cs");

        //aspetto la fine dell'evento
        yield return new WaitForSeconds(durataEvento);
        //disattivo effetti alla UI
        DelegateClass.VentoSolareEventsHandler?.Invoke(false);
    }

}
