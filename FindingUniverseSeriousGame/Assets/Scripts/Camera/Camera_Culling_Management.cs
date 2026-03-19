using UnityEngine;

public class CameraCullingManager : MonoBehaviour
{

    [SerializeField] private float debrisCullDistance = 500f;  // Distanza alla quale i detriti spariscono
    [SerializeField] private float planetsCullDistance = 50000f; // Distanza alla quale i pianeti restano visibili

    

    void LateUpdate()
    {
        
    }

    void Start()
    {
        // Ottieni i layer "Planet" e "Debris"
        int layerPlanet = LayerMask.NameToLayer("Planet");
        int layerDebris = LayerMask.NameToLayer("Debris");

        Camera cam = GetComponent<Camera>();
        
        // Un array di 32 float (uno per ogni layer di Unity)
        float[] distances = new float[32];

        distances[layerPlanet] = planetsCullDistance;   // I detriti spariscono a 500 unità
        distances[layerDebris] = debrisCullDistance; // I pianeti restano visibili fino a 50km

        // Applica le distanze alla camera
        cam.layerCullDistances = distances;
    }
}