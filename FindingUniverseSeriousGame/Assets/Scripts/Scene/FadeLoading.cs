using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeLoading : MonoBehaviour
{
    [SerializeField] private Image FadeToBlack;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float fadeStop = 3f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FadeAnimation());
    }

    private IEnumerator FadeAnimation()
    {
        //Debug.Log("Animazione icona");
        float elapsedTime = 0f;

        yield return new WaitForSeconds(fadeStop); // Tempo in cui l'immagine rimane completamente nera

        elapsedTime = 0f; // Reset del tempo per la seconda parte dell'animazione

        while (elapsedTime < fadeDuration) // Durata dell'animazione (metà del tempo totale della notifica)
        {
            // L'USCITA D'EMERGENZA: Se l'immagine è stata distrutta dal cambio scena, interrompi la Coroutine
            if (FadeToBlack == null) yield break;

            FadeToBlack.color = new Color(0f, 0f, 0f, Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null; // Attende il prossimo frame prima di continuare l'animazione
        }
        
        // Evitiamo di lanciare l'evento se stiamo cambiando livello e l'oggetto non esiste più
        if (FadeToBlack != null)
        {
            DelegateClass.StartFirstMissionEventHandler.Invoke();
        }
    }
}