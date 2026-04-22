using UnityEngine;

public class ShaderAnim : MonoBehaviour
{
    private Renderer rend;
    private int timePropertyID;

    void Start()
    {
        // Prendiamo il componente Renderer del modello
        rend = GetComponent<Renderer>();
        
        // Ottimizzazione: invece di cercare la variabile per nome ogni frame, 
        // ne salviamo l'ID numerico all'avvio per non pesare sulla CPU.
        // ATTENZIONE: Il nome qui dentro deve essere UGUALE al campo "Reference" dello Shader Graph della variabile Unscaled Time
        timePropertyID = Shader.PropertyToID("_Unscaled_Time"); 
    }

    void Update()
    {
        // Se abbiamo un renderer e un materiale assegnato...
        if (rend != null && rend.sharedMaterial != null)
        {
            // ...iniettiamo il tempo non scalato (in quanto siamo dentro il menu) dentro la proprietà dello shader
            rend.sharedMaterial.SetFloat(timePropertyID, Time.unscaledTime);
        }
    }
}
