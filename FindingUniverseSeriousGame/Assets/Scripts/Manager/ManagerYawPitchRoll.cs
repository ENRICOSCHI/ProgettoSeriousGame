using UnityEngine;
using TMPro;

public class ManagerRotation : MonoBehaviour
{
    [Header("Riferimenti UI")]
    [SerializeField] TMP_Text yawUIText;
    [SerializeField] TMP_Text pitchUIText;
    [SerializeField] TMP_Text rollUIText;

    /// <summary>
    /// Aggiorna la visualizzazione di Yaw, Pitch e Roll basandosi sulla rotazione della navicella.
    /// </summary>
    public void UpdateRotationDisplay(MovimentoNavicella spaceship)
    {
        if (spaceship != null)
        {
            // Prendiamo la rotazione locale della navicella
            Vector3 rotation = spaceship.transform.localEulerAngles;

            // In Unity: X = Pitch, Y = Yaw, Z = Roll
            // Applichiamo la formattazione richiesta: "Valore ° Etichetta (Asse)"
            
            if (pitchUIText != null)
                pitchUIText.text = "Pitch (x): " + rotation.x.ToString("F1") + " °";

            if (yawUIText != null)
                yawUIText.text = "Yaw (y) " + rotation.y.ToString("F1") + " °" ;

            if (rollUIText != null)
                rollUIText.text = "Roll (z) " + rotation.z.ToString("F1") + " °";
        }
    }
}