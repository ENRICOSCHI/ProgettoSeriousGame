using UnityEngine;
using System.Collections.Generic;

public class ShaderAnimHierarchy : MonoBehaviour
{
    [Tooltip("Inserisci qui il materiale che vuoi cercare nei figli. Se lasciato vuoto, lo applicherà a tutti i figli.")]
    public Material targetMaterial;

    // Lista che conterrà solo i renderer dei figli che possiedono il materiale target
    private List<Renderer> targetRenderers = new List<Renderer>();
    
    private int timePropertyID;
    private MaterialPropertyBlock propBlock;

    void Start()
    {
        // Ottimizzazione: salviamo l'ID numerico
        timePropertyID = Shader.PropertyToID("_Unscaled_Time"); 
        
        // Inizializziamo il blocco di proprietà
        propBlock = new MaterialPropertyBlock();

        // Recuperiamo tutti i Renderer presenti in questo oggetto e in tutti i suoi figli
        Renderer[] allRenderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in allRenderers)
        {
            // Se non abbiamo specificato un materiale, li prendiamo tutti di default
            if (targetMaterial == null)
            {
                targetRenderers.Add(rend);
            }
            else
            {
                // Controlliamo se il renderer corrente utilizza il materiale specificato.
                // Usiamo un ciclo in caso l'oggetto abbia materiali multipli (es. sub-meshes).
                foreach (Material mat in rend.sharedMaterials)
                {
                    if (mat == targetMaterial)
                    {
                        targetRenderers.Add(rend);
                        break; // Trovato, passiamo al prossimo renderer
                    }
                }
            }
        }
    }

    void Update()
    {
        // Calcoliamo il tempo una sola volta per frame
        float currentTime = Time.unscaledTime;

        // Iteriamo solo sui renderer filtrati all'avvio
        foreach (Renderer rend in targetRenderers)
        {
            // 1. Leggiamo lo stato attuale delle proprietà del renderer
            rend.GetPropertyBlock(propBlock);
            
            // 2. Modifichiamo solo la nostra variabile target
            propBlock.SetFloat(timePropertyID, currentTime);
            
            // 3. Riapplichiamo il blocco di proprietà al renderer
            rend.SetPropertyBlock(propBlock);
        }
    }
}