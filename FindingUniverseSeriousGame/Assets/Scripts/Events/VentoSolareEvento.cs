using System.Collections;
using UnityEngine;

public class VentoSolareEvento : Eventi
{
    [SerializeField] int tempoMINIMO = 5;
    [SerializeField] int tempoMASSIMO = 15;

    private bool firstTime = true;

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
        if (firstTime)
        {
            Descrizione();
            firstTime = false;
        }
        Notifica();
        DelegateClass.VentoSolareEventsHandler?.Invoke(true);
        yield return new WaitForSeconds(secondiDelayEvento);
        DelegateClass.VentoSolareEventsHandler?.Invoke(false);
    }

}
