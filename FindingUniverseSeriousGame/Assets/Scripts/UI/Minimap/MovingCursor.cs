using UnityEngine;

public class MovingCursor : MonoBehaviour
{
    [Header("Riferimenti")]
    public Transform realSpaceship;
    public BoxCollider worldVolume;
    public BoxCollider minimapVolume;

    void Update()
    {
        if (realSpaceship == null || worldVolume == null || minimapVolume == null) return;

        // Dove si trova la nave rispetto al centro dei confini del livello
        Vector3 posLocalShip = worldVolume.transform.InverseTransformPoint(realSpaceship.position);

        // Trasforma la posizione in percentuale rispetto alle dimensioni dei confini
        float pctX = posLocalShip.x / worldVolume.size.x;
        float pctY = posLocalShip.y / worldVolume.size.y;
        float pctZ = posLocalShip.z / worldVolume.size.z;

        // Proietta la percentuale sulle dimensioni dei confini della minimappa
        Vector3 posLocalMap = new Vector3(
            pctX * minimapVolume.size.x,
            pctY * minimapVolume.size.y,
            pctZ * minimapVolume.size.z
        );

        // Applichiamo la posizione assoluta (ignora le scale dei genitori)
        transform.position = minimapVolume.transform.TransformPoint(posLocalMap);
        
        // Calcolo delle direzioni corrette della nave rispetto al mondo
        // InverseTransformDirection ci dà la direzione della nave rispetto al mondo, ma in coordinate locali del volume del mondo
        Vector3 directionForwardLocal = worldVolume.transform.InverseTransformDirection(realSpaceship.forward);
        Vector3 directionUpLocal = worldVolume.transform.InverseTransformDirection(realSpaceship.up);

        // Traduce queste stesse direzioni dentro la minimappa
        // TransformDirection ci dà la direzione assoluta in coordinate del mondo, ma partendo da quelle locali del volume della minimappa
        Vector3 directionForwardMap = minimapVolume.transform.TransformDirection(directionForwardLocal);
        Vector3 directionUpMap = minimapVolume.transform.TransformDirection(directionUpLocal);

        // Ruota il cursore in quella precisa direzione assoluta per seguire la rotazione della nave
        if (directionForwardMap != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionForwardMap, directionUpMap);
        }
    }
}