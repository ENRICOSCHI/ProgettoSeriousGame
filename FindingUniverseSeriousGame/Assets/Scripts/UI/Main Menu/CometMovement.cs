using UnityEngine;

public class CometMovement : MonoBehaviour
{
    [Header("Impostazioni Movimento")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 8f; // Dopo quanti secondi sparisce

    [Header("Impostazioni Rotazione")]
    [Tooltip("Trascina qui l'oggetto figlio 'Cubo' che deve ruotare")]
    [SerializeField] private Transform cube; 
    [SerializeField] private float rotationVelocity = 150f;

    private void Start()
    {
        // Dopo X secondi verrà distrutta
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // Movimento sempre in avanti
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

        // Rotazione del cubo
        if (cube != null)
        {
            cube.Rotate(new Vector3(1, 1, 1) * rotationVelocity * Time.deltaTime);
        }
    }
}