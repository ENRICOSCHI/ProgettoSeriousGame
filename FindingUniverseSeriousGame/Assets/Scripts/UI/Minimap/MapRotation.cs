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

    // Torniamo a usare X (Verticale) e Y (Orizzontale) perché ora siamo su un Genitore Unity puro!
    private float currentRotationX = 0f;
    private float currentRotationY = 0f;

    void Start()
    {
        // Essendo sul Genitore, questo partirà pulito da 0,0,0
        Vector3 rotation = transform.localEulerAngles;

        currentRotationX = NormalizeAngle(rotation.x);
        currentRotationY = NormalizeAngle(rotation.y);

        ApplyRotation();
    }

    void Update()
    {
        float inputX = 0f; 
        float inputY = 0f; 

        if (Input.GetKey(upKey)) inputX += 1f;
        if (Input.GetKey(downKey)) inputX -= 1f;
        
        // Assegniamo Destra/Sinistra all'asse Y
        if (Input.GetKey(leftKey)) inputY += 1f;
        if (Input.GetKey(rightKey)) inputY -= 1f;

        if (invertVerticalAxe) inputX = -inputX;

        if (inputX != 0f || inputY != 0f)
        {
            currentRotationX += inputX * rotationSpeed * Time.unscaledDeltaTime;
            
            // Applichiamo la rotazione sull'asse Y
            // Se noti che gira al contrario, cambia il "-=" in "+="
            currentRotationY -= inputY * rotationSpeed * Time.unscaledDeltaTime;

            currentRotationX = Mathf.Clamp(currentRotationX, minVerticalAngle, maxVerticalAngle);

            ApplyRotation();
        }
    }

    private void ApplyRotation()
    {
        // Ordine Quaternion.Euler: (Verticale X, Orizzontale Y, Z fissa a 0 per non inclinarsi)
        transform.localRotation = Quaternion.Euler(currentRotationX, currentRotationY, 0f);
    }

    private float NormalizeAngle(float angle)
    {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }
}