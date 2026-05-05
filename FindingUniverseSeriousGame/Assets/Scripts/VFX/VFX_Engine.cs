using UnityEngine;

public class VFX_Engine : MonoBehaviour
{
    #region Variabili
    [Header("Riferimenti Motori")]
    [Tooltip("Trascina qui il Particle System del motoreSFX centrale")]
    [SerializeField] private ParticleSystem engineFlareMain;
    [SerializeField] private ParticleSystem engineFlareLeft;
    [SerializeField] private ParticleSystem engineFlareRight;

    [Header("Impostazioni Flare (Velocità/Lunghezza)")]
    [SerializeField] private float minFlareSpeed = 5f; 
    [SerializeField] private float maxFlareSpeed = 25f; 

    [Header("Impostazioni Flare (Intensità/Emissione)")]
    [SerializeField] private float minEmission = 50f;
    [SerializeField] private float maxEmission = 200f;

    [Header("Colore Flare")]
    [SerializeField] private Color minFlareColor = Color.cyan; 
    [SerializeField] private Color maxFlareColor = Color.white; 

    [Header("Impostazioni Motori Laterali")]
    [Tooltip("Quanto velocemente si accendono/spengono i propulsori (valori più alti = più reattivi)")]
    [SerializeField] private float thrusterRampSpeed = 4f;

    [Header("Riferimento allo script di movimento")]
    [SerializeField] private MovimentoNavicella shipMovement;

    // Variabili interne per tracciare lo stato "fisico" dei motori laterali (da 0 a 1)
    private float leftThrustPercent = 0f;
    private float rightThrustPercent = 0f;
    #endregion

    void Update()
    {
        if (shipMovement == null) return;

        // 1. MOTORE CENTRALE (basato sulla velocità della nave)
        float currentSpeed = shipMovement.CurrentSpeed;
        float maxSpeed = shipMovement.maxSpeed;
        float mainSpeedPercentage = 0f;

        if (maxSpeed > 0)
        {
            mainSpeedPercentage = Mathf.Clamp01(currentSpeed / maxSpeed);
        }
        
        // Passiamo true perché è il motoreSFX principale (può rimanere al minimo)
        UpdateSingleEngineVFX(engineFlareMain, mainSpeedPercentage, true);


        // 2. MOTORE SINISTRO (D per effetto inverso)
        if (Input.GetKey(KeyCode.D))
            // Se premo, andiamo verso 1 (100% di potenza)
            leftThrustPercent = Mathf.MoveTowards(leftThrustPercent, 1f, thrusterRampSpeed * Time.deltaTime);
        else
            // Se rilascio, torniamo dolcemente a 0 (spento)
            leftThrustPercent = Mathf.MoveTowards(leftThrustPercent, 0f, thrusterRampSpeed * Time.deltaTime);

        if (engineFlareLeft != null) UpdateSingleEngineVFX(engineFlareLeft, leftThrustPercent, false);


        // 3. MOTORE DESTRO (A per effetto inverso)
        if (Input.GetKey(KeyCode.A))
            rightThrustPercent = Mathf.MoveTowards(rightThrustPercent, 1f, thrusterRampSpeed * Time.deltaTime);
        else
            rightThrustPercent = Mathf.MoveTowards(rightThrustPercent, 0f, thrusterRampSpeed * Time.deltaTime);

        if (engineFlareRight != null) UpdateSingleEngineVFX(engineFlareRight, rightThrustPercent, false);
    }

    /// <summary>
    /// Funzione che aggiorna un singolo motoreSFX.
    /// Il parametro isMainEngine determina se il motoreSFX rimane acceso al minimo da fermo (true) o si spegne del tutto (false).
    /// </summary>
    private void UpdateSingleEngineVFX(ParticleSystem ps, float speedPercent, bool isMainEngine)
    {
        if (ps == null) return; 

        var flareMain = ps.main;
        var flareEmission = ps.emission;

        // I motori laterali devono avere come minimo 0 se sono spenti, mentre il motoreSFX centrale può rimanere acceso al minimo anche a velocità 0
        float actualMinSpeed = isMainEngine ? minFlareSpeed : 0f;
        float actualMinEmission = isMainEngine ? minEmission : 0f;

        // Applichiamo i calcoli
        flareMain.startSpeed = Mathf.Lerp(actualMinSpeed, maxFlareSpeed, speedPercent);
        flareEmission.rateOverTime = Mathf.Lerp(actualMinEmission, maxEmission, speedPercent);
        flareMain.startColor = Color.Lerp(minFlareColor, maxFlareColor, speedPercent);
    }
}