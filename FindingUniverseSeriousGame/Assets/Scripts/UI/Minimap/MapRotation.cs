using UnityEngine;

public class MapRotation : MonoBehaviour
{
    [Header("Impostazioni Rotazione")]
    [Tooltip("Velocità di rotazione in gradi al secondo")]
    [SerializeField] private float rotationSpeed = 100f;

    [Tooltip("Inverti la rotazione verticale (Stile cloche aereo)")]
    [SerializeField] private bool invertVerticalAxe = false;

    [Header("Limiti Rotazione Verticale")]
    [Tooltip("Angolo massimo verso l'alto (Asse X)")]
    [SerializeField] private float maxVerticalAngle = 30f;
    [Tooltip("Angolo massimo verso il basso (Asse X)")]
    [SerializeField] private float minVerticalAngle = -30f;

    [Header("Tasti Direzionali")]
    [SerializeField] private KeyCode upKey = KeyCode.I;
    [SerializeField] private KeyCode downKey = KeyCode.K;
    [SerializeField] private KeyCode leftKey = KeyCode.J;
    [SerializeField] private KeyCode rightKey = KeyCode.L;

    // Variabili per tracciare la rotazione accumulata (Ora usiamo X e Z)
    private float currentRotationX = 0f;
    private float currentRotationZ = 0f;

    /* Nota: essendo il modello importato da Blender, la rotazione orizzontale avviene attorno all'asse Z, mentre quella verticale attorno all'asse X.
        Bisogna mantenerlo così in quanto, se ruotiamo il modello,
        quando si apre il menu ha uno scatto di rotazione difficilmente risolvibile,
        dovuto all'assestamento tra il rotation che aveva prima e quello nuovo.
    */


    void Start()
    {
        // Sincronizzazione all'avvio per evitare lo scatto
        Vector3 rotation = transform.localEulerAngles;

        // Normalizziamo sia X che Z nel range -180 a 180
        currentRotationX = NormalizeAngle(rotation.x);
        currentRotationZ = NormalizeAngle(rotation.z);

        // Applichiamo subito la rotazione pulita per evitare salti al primo frame
        ApplyRotation();
    }

    void Update()
    {
        float inputX = 0f; // Input per il verticale (Asse X)
        float inputZ = 0f; // Input per l'orizzontale (Asse Z)

        /* Accumulo dell'input:
           Permette di sommare più tasti contemporaneamente per una rotazione più fluida,
           evitando conflitti e jittering se più tasti sono premuti insieme */

        if (Input.GetKey(upKey)) inputX += 1f;
        if (Input.GetKey(downKey)) inputX -= 1f;
        
        // Assegniamo i tasti Destra/Sinistra all'input Z
        if (Input.GetKey(leftKey)) inputZ += 1f;
        if (Input.GetKey(rightKey)) inputZ -= 1f;

        if (invertVerticalAxe) inputX = -inputX;

        // Se stiamo premendo almeno un tasto
        if (inputX != 0f || inputZ != 0f)
        {
            // Aggiorniamo i contatori
            currentRotationX += inputX * rotationSpeed * Time.unscaledDeltaTime;
            
            // A seconda di come è orientato il modello, potresti dover togliere il "meno" qui davanti 
            // se noti che Destra/Sinistra sono invertiti sullo schermo
            currentRotationZ -= inputZ * rotationSpeed * Time.unscaledDeltaTime;

            // Blocco verticale (Asse X) per evitare che la mappa si capovolga
            currentRotationX = Mathf.Clamp(currentRotationX, minVerticalAngle, maxVerticalAngle);

            ApplyRotation();
        }
    }

    /// <summary>
    /// Applica la rotazione calcolata alla mappa.
    /// Blocca l'asse Y a 0 per evitare inclinazioni (roll) indesiderate.
    /// </summary>
    private void ApplyRotation()
    {
        // Ordine degli assi in Quaternion.Euler: X, Y, Z. 
        // Passiamo 0f alla Y.
        transform.localRotation = Quaternion.Euler(currentRotationX, 0f, currentRotationZ);
    }

    /// <summary>
    /// Funzione per convertire angoli 0-360 in -180/180
    /// </summary>
    private float NormalizeAngle(float angle)
    {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }
}