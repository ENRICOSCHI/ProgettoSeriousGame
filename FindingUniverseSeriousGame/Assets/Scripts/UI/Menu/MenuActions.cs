using UnityEngine;

public class MenuActions : MonoBehaviour
{
    // Funzione per il tasto SALVA
    public void ActionSave()
    {
        Debug.Log("Salvataggio completato tramite il tasto SALVA nelle opzioni.");
        DelegateClass.SaveEventHandler?.Invoke(false);
    }

    // Funzione per il tasto CARICA
    public void ActionLoad()
    {
        Debug.Log("Caricamento completato tramite il tasto CARICA nelle opzioni.");
        DelegateClass.LoadEventHandler?.Invoke(false);
    }

    // Funzione per il tasto ESCI
    public void ActionQuit()
    {
        Debug.Log("Chiusura del gioco in corso...");
        
        // Se stiamo provando il gioco dentro l'editor di Unity, ferma la Play Mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        
        // Se abbiamo buildato il gioco (.exe), chiudi l'applicazione
#else
        Application.Quit();
#endif
    }
}