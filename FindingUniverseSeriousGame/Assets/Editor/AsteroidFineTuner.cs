using UnityEngine;
using UnityEditor;

public class AsteroidFineTuner : EditorWindow
{
    [SerializeField] private Material _commonMaterial;

    [MenuItem("Tools/Asteroid Fine-Tuner Panel")]
    public static void ShowWindow()
    {
        GetWindow<AsteroidFineTuner>("Asteroid Tuner");
    }

    private void OnGUI()
    {
        GUILayout.Label("Ottimizzazione Asteroidi Esistenti", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("1. Seleziona gli asteroidi nella scena\n2. Assegna il materiale qui sotto\n3. Premi il tasto per sistemare tutto", MessageType.Info);

        _commonMaterial = (Material)EditorGUILayout.ObjectField("Materiale Comune", _commonMaterial, typeof(Material), false);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Sistema Selezione (Fisica + Batches)", GUILayout.Height(40)))
        {
            ProcessSelection();
        }
    }

    private void ProcessSelection()
    {
        GameObject[] selected = Selection.gameObjects;

        if (selected.Length == 0)
        {
            Debug.LogWarning("Devi selezionare gli asteroidi nell'Hierarchy!");
            return;
        }

        // Registriamo trasformazioni e mesh renderer per il Ctrl+Z
        Undo.RecordObjects(Selection.transforms, "Fine-Tune Transforms");
        
        foreach (GameObject obj in selected)
        {
            // --- 1. OTTIMIZZAZIONE PERFORMANCE (BATCHES) ---
            MeshRenderer mr = obj.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                Undo.RecordObject(mr, "Optimize Renderer");
                
                // Assegna il materiale comune (fondamentale per SRP Batching)
                if (_commonMaterial != null) mr.sharedMaterial = _commonMaterial;
                
                // Disattiva le ombre (Taglia i batches del 50-70%)
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
            }

            // --- 2. ROTAZIONE DELICATA ---
            obj.transform.Rotate(
                Random.Range(-10f, 10f),
                Random.Range(-10f, 10f),
                Random.Range(-10f, 10f)
            );

            // --- 3. SEPARAZIONE FISICA ---
            SeparateFromNeighbors(obj);
        }

        Debug.Log($"Bonifica completata su {selected.Length} asteroidi. Controlla ora le Stats!");
    }

    private static void SeparateFromNeighbors(GameObject obj)
    {
        Collider col = obj.GetComponent<Collider>();
        if (col == null) return;

        Collider[] neighbors = Physics.OverlapBox(col.bounds.center, col.bounds.extents * 1.2f);

        foreach (var other in neighbors)
        {
            if (other == col || other.gameObject.transform.IsChildOf(obj.transform)) continue;

            Vector3 direction;
            float distance;

            bool overlapped = Physics.ComputePenetration(
                col, obj.transform.position, obj.transform.rotation,
                other, other.transform.position, other.transform.rotation,
                out direction, out distance
            );

            if (overlapped)
            {
                obj.transform.position += direction * (distance + 0.1f);
                obj.transform.Rotate(direction * 5f);
            }
        }
    }
}