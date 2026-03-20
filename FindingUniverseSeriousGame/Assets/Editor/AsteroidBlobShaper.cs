using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AsteroidBlobShaper : EditorWindow
{
    private float _minDistance = 5f;
    [Range(0.5f, 5f)] private float _sizeMultiplier = 1.2f; // Parametro richiesto per l'espansione
    [Range(0f, 2f)] private float _noiseIntensity = 0.8f; 
    [Range(0.1f, 5f)] private float _noiseScale = 1.0f; 
    private Material _commonMaterial;
    private bool _autoOptimize = true;

    [MenuItem("Tools/Asteroid Blob Shaper (Organic)")]
    public static void ShowWindow() => GetWindow<AsteroidBlobShaper>("Blob Shaper");

    private void OnGUI()
    {
        GUILayout.Label("Modellatore Organico (Fixed Size)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Questo script mantiene o espande l'area originale evitando il restringimento.", MessageType.Info);

        EditorGUILayout.Space(5);
        GUILayout.Label("Dimensioni e Forma", EditorStyles.boldLabel);
        _sizeMultiplier = EditorGUILayout.Slider("Moltiplicatore Dimensione", _sizeMultiplier, 0.5f, 5f);
        _minDistance = EditorGUILayout.FloatField("Distanza Minima", _minDistance);
        
        EditorGUILayout.Space(5);
        GUILayout.Label("Deformazione Blob", EditorStyles.boldLabel);
        _noiseIntensity = EditorGUILayout.Slider("Intensità Rumore", _noiseIntensity, 0f, 2f);
        _noiseScale = EditorGUILayout.Slider("Scala Rumore", _noiseScale, 0.1f, 5f);

        EditorGUILayout.Space(5);
        GUILayout.Label("Asset", EditorStyles.boldLabel);
        _commonMaterial = (Material)EditorGUILayout.ObjectField("Materiale Comune", _commonMaterial, typeof(Material), false);
        _autoOptimize = EditorGUILayout.Toggle("Ottimizza Ombre/Mat", _autoOptimize);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Applica Forma a Bolla", GUILayout.Height(40)))
        {
            ShapeSelectionToBlob();
        }
    }

    private void ShapeSelectionToBlob()
    {
        GameObject[] selected = Selection.gameObjects;
        if (selected.Length < 2) return;

        Undo.RecordObjects(Selection.transforms, "Shape to Blob");
        
        // 1. Calcola il volume originale
        Bounds bounds = new Bounds(selected[0].transform.position, Vector3.zero);
        foreach (GameObject obj in selected) bounds.Encapsulate(obj.transform.position);

        Vector3 center = bounds.center;
        // Applichiamo il moltiplicatore subito per definire l'area di lavoro finale
        Vector3 finalExtents = (bounds.size * _sizeMultiplier) / 2f; 
        
        List<Vector3> newPositions = new List<Vector3>();
        Vector3 noiseSeed = new Vector3(Random.Range(0, 500f), Random.Range(0, 500f), Random.Range(0, 500f));

        foreach (GameObject obj in selected)
        {
            Vector3 finalPos = obj.transform.position; // Fallback alla posizione attuale
            bool accepted = false;
            int attempts = 0;

            while (!accepted && attempts < 300)
            {
                attempts++;
                
                // Genera un punto nel volume ESPANSO
                Vector3 candidate = center + new Vector3(
                    Random.Range(-finalExtents.x, finalExtents.x),
                    Random.Range(-finalExtents.y, finalExtents.y),
                    Random.Range(-finalExtents.z, finalExtents.z)
                );

                // Coordinate normalizzate per il rumore (rispetto al volume espanso)
                Vector3 normPos = new Vector3(
                    (candidate.x - center.x) / finalExtents.x,
                    (candidate.y - center.y) / finalExtents.y,
                    (candidate.z - center.z) / finalExtents.z
                );

                float distSqr = normPos.sqrMagnitude;
                float noiseVal = Perlin3D((normPos + noiseSeed) * _noiseScale);

                // LOGICA: Più ci allontaniamo dal centro, più il rumore deve essere "severo"
                // La soglia 1.0f rappresenta il bordo del volume.
                float threshold = 1.0f + (noiseVal - 0.5f) * _noiseIntensity;

                if (distSqr < threshold)
                {
                    accepted = true;
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

            // Applicazione
            obj.transform.position = finalPos;
            obj.transform.rotation = Random.rotation;

            if (_autoOptimize)
            {
                MeshRenderer mr = obj.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    Undo.RecordObject(mr, "Optimize");
                    if (_commonMaterial != null) mr.sharedMaterial = _commonMaterial;
                    mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    mr.receiveShadows = false;
                }
            }
            newPositions.Add(finalPos);
        }
        Debug.Log("Forma applicata con successo.");
    }

    private float Perlin3D(Vector3 p)
    {
        float ab = Mathf.PerlinNoise(p.x, p.y);
        float bc = Mathf.PerlinNoise(p.y, p.z);
        float ac = Mathf.PerlinNoise(p.x, p.z);
        return (ab + bc + ac) / 3f;
    }
}