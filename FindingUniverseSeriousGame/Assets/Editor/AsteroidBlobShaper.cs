using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Strumento di Editor per rimodellare gruppi di asteroidi esistenti in forme organiche (blob).
/// Utilizza un algoritmo di rejection sampling basato su Perlin Noise 3D per deformare 
/// il volume cubico originale senza causare il restringimento progressivo della selezione.
/// </summary>
public class AsteroidBlobShaper : EditorWindow
{
    // ─── Configurazione Geometria e Volume ────────────────────────────────────

    [Header("Parametri di Spazio")]
    [SerializeField] private float _minDistance = 5f;

    /// <summary>
    /// Moltiplicatore dell'area di lavoro rispetto ai bounds originali. 
    /// Serve a compensare l'arrotondamento degli angoli e a espandere il gruppo.
    /// </summary>
    [Range(0.5f, 5f)] [SerializeField] private float _sizeMultiplier = 1.2f;

    // ─── Parametri di Deformazione ──────────────────────────────────────────

    [Header("Deformazione Organica")]
    [Range(0f, 2f)] [SerializeField] private float _noiseIntensity = 0.8f; 
    [Range(0.1f, 5f)] [SerializeField] private float _noiseScale = 1.0f; 

    // ─── Performance e Asset ────────────────────────────────────────────────

    [Header("Ottimizzazione")]
    [SerializeField] private Material _commonMaterial;
    [SerializeField] private bool _autoOptimize = true;

    // ─── Lifecycle Window ────────────────────────────────────────────────────

    [MenuItem("Tools/Asteroid Blob Shaper (Organic)")]
    public static void ShowWindow() => GetWindow<AsteroidBlobShaper>("Blob Shaper");

    // ─── Interfaccia Utente (GUI) ────────────────────────────────────────────

    private void OnGUI()
    {
        GUILayout.Label("Modellatore Organico (Fixed Size)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Modifica la disposizione degli asteroidi selezionati in una forma a bolla disomogenea.", MessageType.Info);

        EditorGUILayout.Space(5);
        GUILayout.Label("Dimensioni e Forma", EditorStyles.boldLabel);
        _sizeMultiplier = EditorGUILayout.Slider("Espansione Area", _sizeMultiplier, 0.5f, 5f);
        _minDistance = EditorGUILayout.FloatField("Distanza Minima", _minDistance);
        
        EditorGUILayout.Space(5);
        GUILayout.Label("Deformazione Blob", EditorStyles.boldLabel);
        _noiseIntensity = EditorGUILayout.Slider("Intensità Rumore", _noiseIntensity, 0f, 2f);
        _noiseScale = EditorGUILayout.Slider("Scala Rumore", _noiseScale, 0.1f, 5f);

        EditorGUILayout.Space(5);
        GUILayout.Label("Asset e Rendering", EditorStyles.boldLabel);
        _commonMaterial = (Material)EditorGUILayout.ObjectField("Materiale Comune", _commonMaterial, typeof(Material), false);
        _autoOptimize = EditorGUILayout.Toggle("Ottimizza Renderer", _autoOptimize);

        EditorGUILayout.Space(10);

        // Disabilita il bottone se non ci sono abbastanza oggetti selezionati
        GUI.enabled = Selection.gameObjects.Length >= 2;
        if (GUILayout.Button("Applica Forma a Bolla", GUILayout.Height(40)))
        {
            ShapeSelectionToBlob();
        }
        GUI.enabled = true;
    }

    // ─── Logica di Rimodellazione ───────────────────────────────────────────

    /// <summary>
    /// Calcola i nuovi punti di posizionamento all'interno di un volume deformato proceduralmente.
    /// </summary>
    private void ShapeSelectionToBlob()
    {
        GameObject[] selected = Selection.gameObjects;
        if (selected.Length < 2) return;

        // Registrazione per il sistema di Undo
        Undo.RecordObjects(Selection.transforms, "Shape to Blob");
        
        // 1. Calcolo del Bounding Box originale della selezione
        Bounds bounds = new Bounds(selected[0].transform.position, Vector3.zero);
        foreach (GameObject obj in selected) bounds.Encapsulate(obj.transform.position);

        Vector3 center = bounds.center;
        
        // Definiamo i confini dell'area espansa moltiplicando le dimensioni originali
        Vector3 finalExtents = (bounds.size * _sizeMultiplier) / 2f; 
        
        List<Vector3> newPositions = new List<Vector3>();
        
        // Seed casuale per variare la forma del rumore ad ogni click
        Vector3 noiseSeed = new Vector3(Random.Range(0, 500f), Random.Range(0, 500f), Random.Range(0, 500f));

        foreach (GameObject obj in selected)
        {
            Vector3 finalPos = obj.transform.position; // Fallback di sicurezza
            bool accepted = false;
            int attempts = 0;

            // 2. Loop di Rejection Sampling
            // Cerchiamo una coordinata valida all'interno del volume warped
            while (!accepted && attempts < 300)
            {
                attempts++;
                
                // Generazione coordinata candidata nel volume espanso
                Vector3 candidate = center + new Vector3(
                    Random.Range(-finalExtents.x, finalExtents.x),
                    Random.Range(-finalExtents.y, finalExtents.y),
                    Random.Range(-finalExtents.z, finalExtents.z)
                );

                // Normalizzazione della posizione in range [-1, 1] per il campionamento matematico
                Vector3 normPos = new Vector3(
                    (candidate.x - center.x) / finalExtents.x,
                    (candidate.y - center.y) / finalExtents.y,
                    (candidate.z - center.z) / finalExtents.z
                );

                // Calcolo della distanza quadratica dal centro (superellisse di base)
                float distSqr = normPos.sqrMagnitude;
                
                // Campionamento della funzione di rumore per la deformazione organica
                float noiseVal = Perlin3D((normPos + noiseSeed) * _noiseScale);

                // Calcolo della soglia dinamica: 
                // La bolla accetta punti fino a 1.0f (bordo del cubo) modulati dall'intensità del rumore.
                float threshold = 1.0f + (noiseVal - 0.5f) * _noiseIntensity;

                // 3. Test di Appartenenza al Volume Deformato
                if (distSqr < threshold)
                {
                    accepted = true;
                    
                    // Controllo di prossimità fisica con gli asteroidi già posizionati
                    foreach (Vector3 p in newPositions)
                    {
                        if (Vector3.Distance(candidate, p) < _minDistance)
                        {
                            accepted = false;
                            break;
                        }
                    }

                    if (accepted) finalPos = candidate;
                }
            }

            // 4. Applicazione Trasformazioni
            obj.transform.position = finalPos;
            obj.transform.rotation = Random.rotation;

            // 5. Ottimizzazione automatica SRP Batcher e Ombre
            if (_autoOptimize)
            {
                MeshRenderer mr = obj.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    Undo.RecordObject(mr, "Optimize Renderer");
                    
                    if (_commonMaterial != null) mr.sharedMaterial = _commonMaterial;
                    
                    mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    mr.receiveShadows = false;
                }
            }
            newPositions.Add(finalPos);
        }
        Debug.Log($"[BlobShaper] Forma organica applicata con successo su {selected.Length} oggetti.");
    }

    // ─── Calcoli Matematici ──────────────────────────────────────────────────

    /// <summary>
    /// Approssimazione di una funzione Perlin Noise 3D mediando i piani 2D.
    /// </summary>
    /// <param name="p">Vettore di campionamento normalizzato.</param>
    /// <returns>Valore scalare in range [0, 1].</returns>
    private float Perlin3D(Vector3 p)
    {
        float ab = Mathf.PerlinNoise(p.x, p.y);
        float bc = Mathf.PerlinNoise(p.y, p.z);
        float ac = Mathf.PerlinNoise(p.x, p.z);
        
        // Media dei campionamenti sui tre piani principali (XY, YZ, XZ)
        return (ab + bc + ac) / 3f;
    }
}