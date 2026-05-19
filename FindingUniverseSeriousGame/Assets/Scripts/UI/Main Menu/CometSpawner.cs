using UnityEngine;

public class SpawnerComete : MonoBehaviour
{
    [Header("Riferimenti")]
    [SerializeField] private GameObject prefabComet;
    
    [Tooltip("Trascina qui l'Empty GameObject posizionato in basso a sinistra")]
    [SerializeField] private Transform destinationPoint;

    [Header("Range di Spawn")]
    [Tooltip("Di quanto si allontana la cometa dal centro dello spawner lungo l'asse Y")]
    [SerializeField] private float rangeSpawnY = 5f;

    [Header("Tempistiche")]
    [SerializeField] private float minWaitingTime = 1f;
    [SerializeField] private float maxWaitingTime = 3f;

    private float timer;

    private void Start()
    {
        NewTimer();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SpawnComet();
            NewTimer();
        }
    }

    private void NewTimer()
    {
        timer = Random.Range(minWaitingTime, maxWaitingTime);
    }

    private void SpawnComet()
    {
        // Calcolo di un offset casuale basato sul range
        float offsetRandomY = Random.Range(-rangeSpawnY, rangeSpawnY);

        // Somma l'offset ala posizione centrale dello spawner
        Vector3 startPosition = transform.position + new Vector3(0, offsetRandomY, 0);

        // Crea la cometa sullo spawner
        GameObject newComet = Instantiate(prefabComet, startPosition, Quaternion.identity);

        // 2. Ruota la cometa per farla puntare dritto verso l'angolo in basso a sinistra
        if (destinationPoint != null)
        {
            newComet.transform.LookAt(destinationPoint);
        }

        // Aggiunge una leggera deviazione casuale 
        float randomDeviation = Random.Range(-5f, 5f);
        newComet.transform.Rotate(randomDeviation, randomDeviation, 0);
    }
}