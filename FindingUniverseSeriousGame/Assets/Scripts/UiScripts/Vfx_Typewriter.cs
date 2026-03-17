using UnityEngine;
using TMPro;
using System.Collections;

public class Vfx_Typewriter : MonoBehaviour  // Effetto graifco per scrivere il testo lettera per lettera
{
    [SerializeField] private TMP_Text textComponent;
    public float typingSpeed = 0.07f;  //Velocità di scrittura (in secondi per lettera)
    public int MAX_CHAR_TEXT { private set; get; } = 47;

    void Awake()
    {
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();
    }

    /// <summary>
    /// Coroutine per scrivere il testo lettera per lettera
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public IEnumerator TypeText(string message)
    {
        for (int i = 1; i <= message.Length; i++)
        {
            textComponent.text = message.Substring(0, i);
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private void OnEnable()  // Svuota il testo ogni volta che l'oggetto viene attivato
    {
        // Recuperiamo il componente se è nullo
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();

        // Svuotiamo il testo nell'istante esatto dell'accensione
        if (textComponent != null)
            textComponent.text = "";
    }

    /// <summary>
    /// Funzione pubblica per svuotare il testo
    /// </summary>
    public void ClearText() 
    {
        if (textComponent != null)
            textComponent.text = "";
    }
}