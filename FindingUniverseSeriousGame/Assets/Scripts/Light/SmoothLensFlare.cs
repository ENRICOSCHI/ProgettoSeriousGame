using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Controlla l'intensità del Lens Flare in base alla visibilità a schermo.
/// Utilizza un'interpolazione lineare (Lerp) per gestire la transizione fluida.
/// </summary>
public class SunFlareFader : MonoBehaviour
{
    [Header("Riferimenti")]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _sunTransform; // La sfera del sole
    private LensFlareComponentSRP _flareComponent;

    [Header("Parametri di Dissolvenza")]
    [SerializeField] private float _fadeSpeed = 5f;
    [SerializeField] private float _maxIntensity = 1f;
    
    private float _currentTargetIntensity = 0f;

    private void Start()
    {
        _flareComponent = GetComponent<LensFlareComponentSRP>();
        if (_mainCamera == null) _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (_flareComponent == null || _sunTransform == null) return;

        CheckSunVisibility();
        ApplySmoothFade();
    }

    /// <summary>
    /// Calcola se il sole è all'interno del frustum (cono senza punta [in caso non masticassi l'inglese]) della camera (quindi calcola se il sole è dentro la viewport).
    /// </summary>
    private void CheckSunVisibility()
    {
        // Convertiamo la posizione 3D del sole in coordinate Viewport (0-1) e indichiamo quanto è visibile nello schermo da 0 a 1
        Vector3 viewportPos = _mainCamera.WorldToViewportPoint(_sunTransform.position);

        // Verifichiamo se il sole è davanti alla camera (z > 0) e dentro i bordi (0-1)
        bool isVisible = viewportPos.z > 0 && 
                         viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1;

        _currentTargetIntensity = isVisible ? _maxIntensity : 0f;
    }

    /// <summary>
    /// Applica l'intensità finale usando il Lerp per la morbidezza.
    /// </summary>
    private void ApplySmoothFade()
    {
        // Formula del Lerp: Intensity_next = Lerp(Intensity_current, Intensity_target, Time.deltaTime * Speed)
        // ricordo che il lerp prende il valore tra a e b a una distana t (in questo caso il tempo)
        // per esempio se il valora a è 4 e il valore b è 8 e t è 0.5, il valore in uscita sarà 6
        _flareComponent.intensity = Mathf.Lerp(
            _flareComponent.intensity, 
            _currentTargetIntensity, 
            Time.deltaTime * _fadeSpeed
        );
    }
}