using UnityEngine;

public class SolarSystemManagement : MonoBehaviour
{
    readonly float G = 100f; //costante usata per i calcoli
    GameObject[] planets;

    private void Start()
    {
        planets = GameObject.FindGameObjectsWithTag("Pianeta");
        InitalVelocity();
    }

    private void FixedUpdate()
    {
        Gravity();
    }

    /// <summary>
    /// Calcolo gravità dei pianeti attorno al sole 
    /// </summary>
    void Gravity()
    {
        foreach(GameObject a in planets)
        {
            foreach(GameObject b in planets)
            {
                //se a è diverso da b...
                if (!a.Equals(b))
                {
                    //prendo i dati che mi servono per caloclare la Forza di gravità
                    float m1 = a.GetComponent<Rigidbody>().mass;
                    float m2 = b.GetComponent<Rigidbody>().mass;
                    float r = Vector3.Distance(a.transform.position, b.transform.position);
                    //calcolo della forza di gravità -> F = G*(m1*m2)/(r^2)
                    a.GetComponent<Rigidbody>().AddForce((b.transform.position - a.transform.position).normalized * (G * (m1 * m2) / (r * r)));
                }
            }
        }
    }

    /// <summary>
    /// calcolo velocità angolare iniziale
    /// </summary>
    void InitalVelocity()
    {
        foreach (GameObject a in planets)
        {
            foreach (GameObject b in planets)
            {
                //se a è diverso da b...
                if (!a.Equals(b))
                {
                    float m2 = b.GetComponent<Rigidbody>().mass;
                    float r = Vector3.Distance(a.transform.position, b.transform.position);
                    a.transform.LookAt(b.transform);
                    
                    //formula per calcolare la velocità angolare -> w = sqrt(G*m2)/r
                    a.GetComponent<Rigidbody>().linearVelocity += a.transform.right * Mathf.Sqrt((G * m2)/r); //uso linearVelocity al posto di velocity perchè Unity6 lo da come obsoleto
                }
            }
        }
    }
}
