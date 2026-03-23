using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Strumento di Editor per la duplicazione e rimescolamento di gruppi di asteroidi.
/// Analizza il volume occupato dalla selezione attuale e genera una nuova variante 
/// posizionale all'interno di un'area scalabile, mantenendo i legami con i Prefab originali.
/// </summary>
public class AsteroidGroupCloner : EditorWindow
{
    // ─── Configurazione Spaziale ─────────────────────────────────────────────

    [Header("Parametri Area e Distanza")]
    [SerializeField] private float _minDistance = 5f;

    /// <summary>
    /// Fattore di scala applicato al volume di spawn rispetto ai bounds originali.
    /// 1.0f mantiene le dimensioni identiche; valori superiori disperdono il gruppo.
    /// </summary>
    [Range(0.1f, 5f)] [SerializeField] private float _extentScale = 1f;

    // ─── Gerarchia e Risorse ─────────────────────────────────────────────────

    [Header("Impostazioni di Output")]
    [SerializeField] private GameObject _targetParent; 
    [SerializeField] private Material _commonMaterial;

    // ─── Lifecycle Window ────────────────────────────────────────────────────

    [MenuItem("Tools/Asteroid Group Cloner")]
    public static void ShowWindow()
    {
        GetWindow<AsteroidGroupCloner>("Group Cloner");
    }

    // ─── Interfaccia Utente (GUI) ────────────────────────────────────────────

    private void OnGUI()
    {
        GUILayout.Label("Clona e Rimescola Gruppo", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Crea una variante posizionale della selezione corrente rispettando l'estensione volumetrica originale.", MessageType.Info);

        EditorGUILayout.Space(5);
        GUILayout.Label("Impostazioni Gerarchia", EditorStyles.boldLabel);
        _targetParent = (GameObject)EditorGUILayout.ObjectField("Padre Destinazione", _targetParent, typeof(GameObject), true);
        _commonMaterial = (Material)EditorGUILayout.ObjectField("Materiale Comune", _commonMaterial, typeof(Material), false);

        EditorGUILayout.Space(5);
        GUILayout.Label("Parametri Area e Distanza", EditorStyles.boldLabel);
        _extentScale = EditorGUILayout.Slider("Scala Estensione Area", _extentScale, 0.1f, 5f);
        _minDistance = EditorGUILayout.FloatField("Distanza Minima", _minDistance);

        EditorGUILayout.Space(10);

        // Il tasto è attivo solo se c'è almeno un oggetto selezionato per definire i Bounds
        GUI.enabled = Selection.gameObjects.Length > 0;
        if (GUILayout.Button("Clona e Genera Variante", GUILayout.Height(40)))
        {
            CloneAndRandomize();
        }
        GUI.enabled = true;
    }

    // ─── Logica di Clonazione ────────────────────────────────────────────────

    /// <summary>
    /// Calcola l'inviluppo convesso (Bounds) della selezione, scala il volume risultante
    /// e istanzia nuovi cloni in posizioni casuali non sovrapposte.
    /// </summary>
    private void CloneAndRandomize()
    {
        GameObject[] selected = Selection.gameObjects;

        if (selected.Length < 1)
        {
            Debug.LogWarning("[GroupCloner] Seleziona almeno un oggetto da clonare!");
            return;
        }

        // 1. CALCOLO VOLUME ORIGINALE (BOUNDS)
        // Encapsulate permette di espandere progressivamente il volume includendo ogni oggetto
        Bounds bounds = new Bounds(selected[0].transform.position, Vector3.zero);
        foreach (GameObject obj in selected)
        {
            bounds.Encapsulate(obj.transform.position);
        }

        // 2. APPLICAZIONE SCALA E CENTRO
        Vector3 scaledSize = bounds.size * _extentScale;
        Vector3 center = bounds.center;

        // 3. GESTIONE OGGETTO PADRE
        // Se non specificato, viene creato un nuovo container per mantenere l'ordine nell'Hierarchy
        GameObject parentToUse = _targetParent;
        if (parentToUse == null)
        {
            parentToUse = new GameObject("New_Asteroid_Group_Variant");
            parentToUse.transform.position = center;
            Undo.RegisterCreatedObjectUndo(parentToUse, "Create Asteroid Group");
        }

        List<Vector3> newPositions = new List<Vector3>();

        // 4. GENERAZIONE E POSIZIONAMENTO
        foreach (GameObject sourceObj in selected)
        {
            Vector3 randomPos = Vector3.zero;
            bool found = false;
            int attempts = 0;

            // Algoritmo di Rejection Sampling per la ricerca di spazio libero
            while (!found && attempts < 100)
            {
                attempts++;
                randomPos = center + new Vector3(
                    Random.Range(-scaledSize.x / 2, scaledSize.x / 2),
                    Random.Range(-scaledSize.y / 2, scaledSize.y / 2),
                    Random.Range(-scaledSize.z / 2, scaledSize.z / 2)
                );

                found = true;
                foreach (Vector3 p in newPositions)
                {
                    if (Vector3.Distance(randomPos, p) < _minDistance)
                    {
                        found = false;
                        break;
                    }
                }
            }

            // 5. CLONAZIONE (PREFAB COMPATIBLE)
            // Fondamentale: InstantiatePrefab mantiene il legame con l'asset, permettendo modifiche massive post-spawn
            GameObject newObj;
            if (PrefabUtility.IsPartOfAnyPrefab(sourceObj))
            {
                newObj = (GameObject)PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(sourceObj));
            }
            else
            {
                newObj = Instantiate(sourceObj);
            }

            // 6. TRASFORMAZIONE E OTTIMIZZAZIONE
            newObj.transform.position = randomPos;
            newObj.transform.rotation = Random.rotation;
            newObj.transform.localScale = sourceObj.transform.localScale;
            newObj.transform.SetParent(parentToUse.transform);

            // Applicazione automatica dei parametri per SRP Batcher
            MeshRenderer mr = newObj.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                if (_commonMaterial != null) mr.sharedMaterial = _commonMaterial;
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
            }

            newPositions.Add(randomPos);
            Undo.RegisterCreatedObjectUndo(newObj, "Clone Asteroid Group");
        }

        Debug.Log($"[GroupCloner] Variante di {selected.Length} oggetti generata con successo.");
    }
}