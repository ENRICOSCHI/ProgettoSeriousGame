using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ManagerSpeed : MonoBehaviour
{
    //singleton
    public static ManagerSpeed Instance { get; private set; }

    [Header("Riferimenti UI")]
    [SerializeField] TMP_Text speedUIText;

    //controllo che effettivamente sia l'unico oggetto attivo nelle scene (singleton)
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// Aggiorna la visualizzazione della velocità
    /// </summary>
    public void UpdateSpeedDisplay(MovimentoNavicella spaceship)
    {
        if (spaceship != null && speedUIText != null)
        {
            speedUIText.text = spaceship.CurrentSpeed.ToString("F1"); // viene mostrato fino al primo decimale
        }
    }
}
