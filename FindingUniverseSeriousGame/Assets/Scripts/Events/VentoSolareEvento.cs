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
        yield return new WaitForSeconds(secondiDelayEvento);
        if (!ManagerHandler.ManagerInstance.CodexManager.categoryLists[1].entries[1].isDiscovered)
        {
            ActiveSubtitlesWithAudio();
            UnlockOnCodexMenu();
            PersistentSceneData.Instance.isDescriptionVentoSolareHappened = true;
        }
        NotificaPersonalizzata(notificaMessaggio[0]);
        DelegateClass.VentoSolareEventsHandler?.Invoke(true);

        if (tempestaSFX != null)
            ManagerHandler.ManagerInstance.SFXManager.PlaySoundEffect(tempestaSFX, MovimentoNavicella.GetNavicellaTransform(), 1f);
        else
            Debug.LogWarning("Manca tempestaSFX in VentoSolareEvento.cs");

        yield return new WaitForSeconds(durataEvento);
        DelegateClass.VentoSolareEventsHandler?.Invoke(false);
    }

}
