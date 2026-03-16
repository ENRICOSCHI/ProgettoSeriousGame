using UnityEngine;
using TMPro;
using System.Collections;

public class Vfx_Typewriter : MonoBehaviour  // Effetto graifco per scrivere il testo lettera per lettera
{
    [SerializeField] private TMP_Text textComponent;
    public float typingSpeed = 0.04f;  //Velocità di scrittura (in secondi per lettera)

    void Awake()
    {
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();
    }

    public IEnumerator TypeText(string message)  // Coroutine per scrivere il testo lettera per lettera
    {
        textComponent.text = "";
        foreach (char c in message.ToCharArray())
        {
            textComponent.text += c;
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

    public void ClearText()  // Funzione pubblica per svuotare il testo
    {
        if (textComponent != null)
            textComponent.text = "";
    }
}