using UnityEngine;
using Unity.Cinemachine;
using Unity.Profiling;
using System.Collections;

public class MovimentoNavicella : MonoBehaviour
{
    [Header("Monitoring")]
    [SerializeField] public float currentSpeed = 0f;
    [SerializeField] float boost = 10f;
    [SerializeField] float boostDurationSecond = 2f;
    public float CurrentSpeed => currentSpeed; // Permette ad altri script di leggere la velocita attuale senza modificarla


    [Header("Camera Settings")]
    [SerializeField] CinemachineCamera cinemachineCamera; // Riferimento alla Cinemachine Camera
    [SerializeField] float minFOV = 40f; // FOV minimo
    [SerializeField] float maxFOV = 70f; // FOV massimo
    [SerializeField] float fovSensitivity = 0.5f; // Sensibilit� del FOV


    [Header("Control Configuration / Mapping Input")]
    [SerializeField] bool useAlternativeControls = false; // Se true, usa la modalit� alternativa
    private float targetH = 0f; // Input orizzontale target
    private float targetV = 0f; // Input verticale target
    private bool isAccelerating = false;
    private bool isDecelerating = false;
    private bool isBoostActive = false;


    [Header("Flight Settings")]

    [SerializeField] float rotationSpeed = 90f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] public float maxSpeed = 50f;
    [SerializeField] float maxPitchAngle = 60f; // Angolo massimo di pitch (su/gi�)
    [SerializeField] float maxRollAngle = 40f;  // Angolo massimo di roll (inclinazione ali)


    [Header("Auto-Leveling Settings")]
    [SerializeField] float returnSpeed = 2f;
    private float realTimePitch = 0f;
    private float realTimeYaw = 0f;
    private float realTimeRoll = 0f;

    [Header("Input Smoothing")]
    [SerializeField] float fluidityInput = 5f;
    private float hInputSmooth = 0f;
    private float vInputSmooth = 0f;

    // Update is called once per frame
    void Update()
    {
        MapInputs();
        Rotation();
        Speed();
        CamEffect();

        //Muove l'oggetto in avanti in base alla velocit� attuale
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Mappatura degli input, il giocatore pu� cambiare tra 2 input differenti
    /// </summary>
    void MapInputs()  //Espandibile se si vuole
    {
        /*
         useAlternativeControls = true 
            -> W/S: accellera/decellera
            -> ctrl/shift: su/giu
         useAlternativeControls = false
            -> W/S: su/gi�
            -> ctrl/shift: accellera/decellera
        */
        if (!useAlternativeControls)
        {
            // Modalit� Standard: WASD ruota, Ctrl/Shift accelera
            targetH = Input.GetAxis("Horizontal"); // A - D
            targetV = Input.GetAxis("Vertical");   // W - S
            float speedeProfondita = Input.GetAxis("Profondita");
            isAccelerating = speedeProfondita < -0.1f; // shift, ha valori che vanno in negativo
            isDecelerating = speedeProfondita > 0.1f; //ctrl
        }
        else
        {
            // Modalit� Alternativa: AD e Ctrl/Shift ruotano, WS accelera
            targetH = Input.GetAxis("Horizontal"); // A - D (Yaw)

            // Pitch gestito da Ctrl (Su) e Shift (Gi�)
            targetV = Input.GetAxis("Profondita");

            // Velocit� gestita da W e S
            float speedAxis = Input.GetAxis("Vertical");
            isAccelerating = speedAxis > 0.1f;
            isDecelerating = speedAxis < -0.1f;
        }
    }

    /// <summary>
    /// rotazione navicella durante lo spostamento a destra e sinistra
    /// </summary>
    void Rotation()
    {
        // Interpolazione degli input per una risposta pi� fluida
        hInputSmooth = Mathf.Lerp(hInputSmooth, targetH, Time.deltaTime * fluidityInput);
        vInputSmooth = Mathf.Lerp(vInputSmooth, targetV, Time.deltaTime * fluidityInput);

        // Logica PITCH (Su/Gi�)
        // Definiamo un "angolo bersaglio" (es. max 60 gradi)
        float targetPitchAngle = vInputSmooth * maxPitchAngle;

        // Usiamo il Lerp per raggiungerlo. returnSpeed controlla la morbidezza del movimento.
        realTimePitch = Mathf.Lerp(realTimePitch, targetPitchAngle, Time.deltaTime * returnSpeed);

        // Logica YAW (Destra/Sinistra) - Invariata (perch� deve essere cumulativa)
        if (Mathf.Abs(hInputSmooth) > 0.05f)
        {
            realTimeYaw += hInputSmooth * rotationSpeed * Time.deltaTime;
        }

        // Logica ROLL (Inclinazione ali)
        float targetRollAngle = -hInputSmooth * maxRollAngle;
        realTimeRoll = Mathf.Lerp(realTimeRoll, targetRollAngle, Time.deltaTime * returnSpeed);

        // Applichiamo la rotazione finale
        transform.localRotation = Quaternion.Euler(-realTimePitch, realTimeYaw, realTimeRoll);
    }

    /// <summary>
    /// Gestione velocit�/accellerazione
    /// </summary>
    void Speed()
    {
        if(isBoostActive) return; // Se il boost è attivo, non modifichiamo la velocità con l'input normale
        //Accelerazione con shift
        if (isAccelerating)
        {
            currentSpeed += acceleration * Time.deltaTime;
        }

        //Decelerazione con ctrl
        if (isDecelerating)
        {
            currentSpeed -= acceleration * Time.deltaTime;
        }

        //Limita la velocit� massima e minima
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
    }

    /// <summary>
    /// Effetti telecamera durante il volo
    /// </summary>
    void CamEffect()
    {
        if (cinemachineCamera != null)
        {
            // Calcoliamo quanto stiamo andando veloci in percentuale (da 0 a 1)
            float speedPercent = currentSpeed / maxSpeed;

            // Troviamo il FOV target tra il minimo e il massimo
            float targetFOV = Mathf.Lerp(minFOV, maxFOV, speedPercent);

            // Applichiamo il FOV alla lente della camera con un Lerp per non farlo scattare
            cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, targetFOV, Time.deltaTime * fovSensitivity);
        }
    }

    /// <summary>
    /// Aggiungo per un tempo limitato una velocità alla nave
    /// </summary>
    /// <returns></returns>
    public IEnumerator AddBoost()
    {
        isBoostActive = true;
        float oldSpeed = currentSpeed;// salvo la velocità priama del boost
        currentSpeed += boost; // applico boost
        yield return new WaitForSeconds(boostDurationSecond);
        currentSpeed = oldSpeed; // resetto velcità precedente
        isBoostActive = false;
    }
}
