using UnityEngine;

public class CameraCullingManager : MonoBehaviour
{

    [SerializeField] private float debrisCullDistance = 500f;  // Distanza alla quale i detriti spariscono
    [SerializeField] private float planetsCullDistance = 50000f; // Distanza alla quale i pianeti restano visibili

    void Start()
    {
        // Ottieni l'indice dei layer "Planet" e "Debris"
        int layerPlanet = LayerMask.NameToLayer("Planet");
        int layerDebris = LayerMask.NameToLayer("Debris");

        Camera cam = GetComponent<Camera>();
        
        // Un array float di 32 valori (uno per ogni layer di Unity)
        float[] distances = new float[32];

        distances[layerPlanet] = planetsCullDistance;   // I detriti spariscono a 500 unità
        distances[layerDebris] = debrisCullDistance; // I pianeti restano visibili fino a 50km

        // Applica ogni distanza di rendering assegnata in distances al layer della telecamera
        cam.layerCullDistances = distances;
    }
}