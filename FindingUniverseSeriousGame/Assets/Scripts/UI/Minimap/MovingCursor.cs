using UnityEngine;

public class MovingCursor : MonoBehaviour
{
    [Header("Riferimenti")]
    public Transform naveReale;
    public BoxCollider volumeMondo;
    public BoxCollider volumeMinimappa;

    void Update()
    {
        if (naveReale == null || volumeMondo == null || volumeMinimappa == null) return;

        // --- 1. POSIZIONE PERFETTA ---
        // Dove si trova la nave rispetto al centro del cubo gigante?
        Vector3 posLocaleNave = volumeMondo.transform.InverseTransformPoint(naveReale.position);

        // Trasformiamo la posizione in percentuale (es. 0.5 = bordo)
        float pctX = posLocaleNave.x / volumeMondo.size.x;
        float pctY = posLocaleNave.y / volumeMondo.size.y;
        float pctZ = posLocaleNave.z / volumeMondo.size.z;

        // Proiettiamo la percentuale sulle dimensioni del cubo piccolo
        Vector3 posLocaleMappa = new Vector3(
            pctX * volumeMinimappa.size.x,
            pctY * volumeMinimappa.size.y,
            pctZ * volumeMinimappa.size.z
        );

        // Applichiamo la posizione assoluta (ignora le scale sballate dei genitori)
        transform.position = volumeMinimappa.transform.TransformPoint(posLocaleMappa);
        
        // --- 2. ROTAZIONE CORRETTA (Metodo dei Vettori Infallibili) ---
        // Qual è la direzione "Avanti" e "Alto" della nave rispetto al mondo?
        Vector3 direzioneAvantiLocale = volumeMondo.transform.InverseTransformDirection(naveReale.forward);
        Vector3 direzioneAltoLocale = volumeMondo.transform.InverseTransformDirection(naveReale.up);

        // Traduciamo queste stesse direzioni dentro la minimappa
        Vector3 direzioneAvantiMappa = volumeMinimappa.transform.TransformDirection(direzioneAvantiLocale);
        Vector3 direzioneAltoMappa = volumeMinimappa.transform.TransformDirection(direzioneAltoLocale);

        // Ruotiamo il cursore in quella precisa direzione assoluta
        if (direzioneAvantiMappa != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direzioneAvantiMappa, direzioneAltoMappa);
        }
    }
}