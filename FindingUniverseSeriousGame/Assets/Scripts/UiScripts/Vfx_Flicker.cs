using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]  //Questo codice non funziona se non è presente un CanvasGroup, quindi lo aggiunge automaticamente se manca
public class Vfx_VectorHardware : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    
    private bool isFlickering = false;
    private bool isGlitching = false;

    [Header("Sfarfallio Intermittente")]
    [Range(0f, 100f)] public float flickerChance = 1f; 
    [SerializeField] float flickerBurstDuration = 0.2f;
    [SerializeField] float flickerSpeed = 30f;
    [Range(0f, 1f)] public float minAlpha = 0.7f;

    [Header("Glitch Casuale")]
    [Range(0f, 100f)] public float glitchChance = 0.3f;
    [SerializeField] float glitchIntensity = 8f;
    [SerializeField] float glitchDuration = 0.06f;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.alpha = 1f; // Parte sempre stabile
    }

    void Update()
    {
        // Logica dello Sfarfallio
        if (!isFlickering && Random.Range(0f, 100f) < flickerChance)
        {
            StartCoroutine(FlickerRoutine());
        }

        // Logica del Glitch
        if (!isGlitching && Random.Range(0f, 100f) < glitchChance)
        {
            StartCoroutine(GlitchRoutine());
        }
    }

    IEnumerator FlickerRoutine()
    {
        isFlickering = true;
        float elapsed = 0f;

        while (elapsed < flickerBurstDuration)
        {
            // Crea l'oscillazione rapida tipica del monitor vettoriale
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
            canvasGroup.alpha = Mathf.Lerp(minAlpha, 1f, noise);
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f; // Torna alla stabilità totale
        isFlickering = false;
    }

    IEnumerator GlitchRoutine()
    {
        isGlitching = true;

        Vector2 glitchOffset = new Vector2(
            Random.Range(-glitchIntensity, glitchIntensity),
            Random.Range(-glitchIntensity, glitchIntensity)
        );

        rectTransform.anchoredPosition = originalPosition + glitchOffset;
        
        yield return new WaitForSeconds(glitchDuration);

        rectTransform.anchoredPosition = originalPosition;
        isGlitching = false;
    }
}