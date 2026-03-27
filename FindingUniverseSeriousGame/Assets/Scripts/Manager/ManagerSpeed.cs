using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ManagerSpeed : MonoBehaviour
{
    [Header("Riferimenti UI")]
    [SerializeField] TMP_Text speedUIText;


    /// <summary>
    /// Aggiorna la visualizzazione della velocit�
    /// </summary>
    public void UpdateSpeedDisplay(MovimentoNavicella spaceship)
    {
        if (spaceship != null && speedUIText != null)
        {
            speedUIText.text = spaceship.CurrentSpeed.ToString("F1"); // viene mostrato fino al primo decimale
        }
    }
}
