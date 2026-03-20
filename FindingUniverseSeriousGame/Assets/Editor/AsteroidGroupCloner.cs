using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AsteroidGroupCloner : EditorWindow
{
    private float _minDistance = 5f;
    private float _extentScale = 1f; // Moltiplicatore dell'area
    private GameObject _targetParent; // Padre di destinazione
    private Material _commonMaterial;

    [MenuItem("Tools/Asteroid Group Cloner")]
    public static void ShowWindow()
    {
        GetWindow<AsteroidGroupCloner>("Group Cloner");
    }

    private void OnGUI()
    {
        GUILayout.Label("Clona e Rimescola Gruppo", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Seleziona gli asteroidi sorgente nell'Hierarchy, imposta i parametri e clicca sul tasto.", MessageType.Info);

        EditorGUILayout.Space(5);
        GUILayout.Label("Impostazioni Gerarchia", EditorStyles.boldLabel);
        _targetParent = (GameObject)EditorGUILayout.ObjectField("Padre Destinazione", _targetParent, typeof(GameObject), true);
        _commonMaterial = (Material)EditorGUILayout.ObjectField("Materiale Comune", _commonMaterial, typeof(Material), false);

        EditorGUILayout.Space(5);
        GUILayout.Label("Parametri Area e Distanza", EditorStyles.boldLabel);
        _extentScale = EditorGUILayout.Slider("Scala Estensione Area", _extentScale, 0.1f, 5f);
        _minDistance = EditorGUILayout.FloatField("Distanza Minima", _minDistance);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Clona e Genera Variante", GUILayout.Height(40)))
        {
            CloneAndRandomize();
        }
    }

    private void CloneAndRandomize()
    {
        GameObject[] selected = Selection.gameObjects;

        if (selected.Length < 1)
        {
            Debug.LogWarning("Seleziona almeno un oggetto da clonare!");
            return;
        }

        // 1. Calcola il volume occupato (Bounds) originale
        Bounds bounds = new Bounds(selected[0].transform.position, Vector3.zero);
        foreach (GameObject obj in selected)
        {
            bounds.Encapsulate(obj.transform.position);
        }

        // 2. Applica la scala all'estensione
        Vector3 scaledSize = bounds.size * _extentScale;
        Vector3 center = bounds.center;

        // 3. Gestione dell'oggetto Padre
        GameObject parentToUse = _targetParent;
        if (parentToUse == null)
        {
            parentToUse = new GameObject("New_Asteroid_Group_Variant");
            parentToUse.transform.position = center;
            Undo.RegisterCreatedObjectUndo(parentToUse, "Create Asteroid Group");
        }

        List<Vector3> newPositions = new List<Vector3>();

        foreach (GameObject sourceObj in selected)
        {
            Vector3 randomPos = Vector3.zero;
            bool found = false;
            int attempts = 0;

            // Cerca una posizione casuale nell'area scalata
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

            // 4. Clonazione mantenendo il legame col Prefab
            GameObject newObj;
            if (PrefabUtility.IsPartOfAnyPrefab(sourceObj))
            {
                newObj = (GameObject)PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(sourceObj));
            }
            else
            {
                newObj = Instantiate(sourceObj);
            }

            // 5. Configurazione Trasformazione e Ottimizzazione
            newObj.transform.position = randomPos;
            newObj.transform.rotation = Random.rotation;
            newObj.transform.localScale = sourceObj.transform.localScale;
            newObj.transform.SetParent(parentToUse.transform);

            // Applica Materiale e spegne ombre per performance
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

        Debug.Log($"Generata variante di {selected.Length} oggetti con scala area {_extentScale}x.");
    }
}