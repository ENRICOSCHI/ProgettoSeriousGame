using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

/// <summary>
/// Finestra di Editor per la generazione procedurale di detriti e asteroidi.
/// Ottimizzata per garantire la compatibilità con l'SRP Batcher tramite l'uso di sharedMaterial.
/// </summary>
public class DebrisSpawnerWindow : EditorWindow
{
    // ─── Tipi e Strutture Dati ──────────────────────────────────────────────

    private enum PresetType { Manuale, FasciaAsteroidi, NuvolaDensa, CampoDisperso }

    /// <summary>
    /// Struttura dati per la serializzazione e persistenza delle impostazioni della finestra.
    /// </summary>
    [Serializable]
    private class SpawnerData
    {
        public string[] prefabGuids = new string[4];
        public string materialGuid; 
        public int spawnCount = 50;
        public Vector3 areaMin = new Vector3(-500, -500, -500);
        public Vector3 areaMax = new Vector3(500, 500, 500);
        public float minScale = 200f;
        public float maxScale = 400f;
        public float minDistance = 50f;
        public PresetType currentPreset = PresetType.Manuale;
    }

    // ─── Variabili di Configurazione ─────────────────────────────────────────

    [Header("Asset dei Detriti")]
    [SerializeField] private GameObject[] _prefabs = new GameObject[4];
    [SerializeField] private Material _debrisMaterial; 

    [Header("Parametri di Generazione")]
    [SerializeField] private int _spawnCount = 50;
    [SerializeField] private Vector3 _areaMin = new Vector3(-500, -500, -500);
    [SerializeField] private Vector3 _areaMax = new Vector3(500, 500, 500);
    [SerializeField] private float _minScale = 200f;
    [SerializeField] private float _maxScale = 400f;
    [SerializeField] private float _minDistance = 50f;

    [Header("Riferimenti Scena")]
    [SerializeField] private GameObject _parentObject;
    [SerializeField] private PresetType _selectedPreset = PresetType.Manuale;

    private const string SaveKey = "DebrisSpawner_Settings_V3";

    // ─── Lifecycle Window ────────────────────────────────────────────────────

    [MenuItem("Tools/Debris Spawner Panel")]
    public static void ShowWindow() => GetWindow<DebrisSpawnerWindow>("Debris Spawner");

    private void OnEnable() => LoadState();
    private void OnDisable() => SaveState();

    // ─── Interfaccia Utente (GUI) ────────────────────────────────────────────

    private void OnGUI()
    {
        // Sezione Asset
        GUILayout.Label("Asset dei Detriti", EditorStyles.boldLabel);
        for (int i = 0; i < 4; i++)
        {
            _prefabs[i] = (GameObject)EditorGUILayout.ObjectField($"Prefab {i + 1}", _prefabs[i], typeof(GameObject), false);
        }

        _debrisMaterial = (Material)EditorGUILayout.ObjectField("Materiale Comune", _debrisMaterial, typeof(Material), false);

        EditorGUILayout.Space(10);

        // Sezione Preset
        GUILayout.Label("Configurazione Preset", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        _selectedPreset = (PresetType)EditorGUILayout.EnumPopup("Usa Preset:", _selectedPreset);
        if (EditorGUI.EndChangeCheck() && _selectedPreset != PresetType.Manuale) 
            ApplyPreset(_selectedPreset);

        EditorGUILayout.Space(10);

        // Sezione Parametri
        GUILayout.Label("Parametri di Generazione", EditorStyles.boldLabel);
        _parentObject = (GameObject)EditorGUILayout.ObjectField("Oggetto Padre", _parentObject, typeof(GameObject), true);
        
        EditorGUI.BeginChangeCheck();
        _spawnCount = EditorGUILayout.IntField("Quantità", _spawnCount);
        _areaMin = EditorGUILayout.Vector3Field("Area Min", _areaMin);
        _areaMax = EditorGUILayout.Vector3Field("Area Max", _areaMax);
        _minScale = EditorGUILayout.FloatField("Scala Minima", _minScale);
        _maxScale = EditorGUILayout.FloatField("Scala Massima", _maxScale);
        _minDistance = EditorGUILayout.FloatField("Distanza Minima", _minDistance);

        // Se l'utente modifica un parametro, il preset torna in modalità manuale
        if (EditorGUI.EndChangeCheck()) _selectedPreset = PresetType.Manuale;

        EditorGUILayout.Space(20);

        // Validazione e Bottone di Spawn
        GUI.enabled = (_parentObject != null && _prefabs[0] != null);
        if (GUILayout.Button("Genera Detriti", GUILayout.Height(30)))
        {
            SpawnDebris();
            SaveState();
        }
        GUI.enabled = true;
    }

    // ─── Logica di Generazione ───────────────────────────────────────────────

    /// <summary>
    /// Applica i valori predefiniti in base al preset selezionato.
    /// </summary>
    private void ApplyPreset(PresetType type)
    {
        switch (type)
        {
            case PresetType.FasciaAsteroidi:
                _spawnCount = 120; _areaMin = new Vector3(-1500, -50, -1500); _areaMax = new Vector3(1500, 50, 1500);
                _minScale = 250f; _maxScale = 500f; _minDistance = 100f; break;

            case PresetType.NuvolaDensa:
                _spawnCount = 150; _areaMin = new Vector3(-20, -20, -20); _areaMax = new Vector3(70, 70, 70);
                _minScale = 2f; _maxScale = 5f; _minDistance = 6f; break;

            case PresetType.CampoDisperso:
                _spawnCount = 70; _areaMin = new Vector3(-100, -100, -100); _areaMax = new Vector3(200, 200, 200);
                _minScale = 10f; _maxScale = 20f; _minDistance = 20f; break;
        }
    }

    /// <summary>
    /// Esegue il loop di generazione dei detriti utilizzando un algoritmo di rejection sampling 
    /// per garantire la distanza minima tra gli oggetti.
    /// </summary>
    private void SpawnDebris()
    {
        List<GameObject> validPrefabs = new List<GameObject>();
        foreach (var p in _prefabs) if (p != null) validPrefabs.Add(p);
        
        int spawned = 0; 
        int attempts = 0; 
        List<Vector3> spawnedPositions = new List<Vector3>();
        
        // Loop di generazione con limite di tentativi per evitare loop infiniti in aree sature
        while (spawned < _spawnCount && attempts < _spawnCount * 20)
        {
            attempts++;
            Vector3 randomPos = new Vector3(
                UnityEngine.Random.Range(_areaMin.x, _areaMax.x), 
                UnityEngine.Random.Range(_areaMin.y, _areaMax.y), 
                UnityEngine.Random.Range(_areaMin.z, _areaMax.z)
            );

            // Controllo di prossimità per evitare sovrapposizioni visive e fisiche
            bool tooClose = false;
            foreach (Vector3 pos in spawnedPositions) 
                if (Vector3.Distance(randomPos, pos) < _minDistance) { tooClose = true; break; }
            
            if (!tooClose)
            {
                // Istanziazione del Prefab mantenendo il legame con l'asset originale
                GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(validPrefabs[UnityEngine.Random.Range(0, validPrefabs.Count)]);
                
                newObj.transform.position = randomPos; 
                newObj.transform.rotation = UnityEngine.Random.rotation;
                
                float s = UnityEngine.Random.Range(_minScale, _maxScale);
                newObj.transform.localScale = new Vector3(s, s, s);
                newObj.transform.SetParent(_parentObject.transform);
                newObj.layer = 7;

                // Ottimizzazione Batching: Assegnazione del materiale condiviso per attivare l'SRP Batcher
                if (_debrisMaterial != null)
                {
                    MeshRenderer mr = newObj.GetComponent<MeshRenderer>();
                    if (mr != null) mr.sharedMaterial = _debrisMaterial;
                }

                // Registrazione dell'operazione per il sistema di Undo di Unity
                Undo.RegisterCreatedObjectUndo(newObj, "Spawn Debris");
                spawnedPositions.Add(randomPos);
                spawned++;
            }
        }
        Debug.Log($"[DebrisSpawner] Generati {spawned} detriti. SRP Batching pronto.");
    }

    // ─── Persistenza Dati ────────────────────────────────────────────────────

    /// <summary>
    /// Salva lo stato attuale della finestra negli EditorPrefs in formato JSON.
    /// </summary>
    private void SaveState()
    {
        SpawnerData data = new SpawnerData {
            spawnCount = _spawnCount, areaMin = _areaMin, areaMax = _areaMax,
            minScale = _minScale, maxScale = _maxScale, minDistance = _minDistance,
            currentPreset = _selectedPreset
        };

        if (_debrisMaterial != null) 
            data.materialGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_debrisMaterial));
        
        for (int i = 0; i < 4; i++) 
            if (_prefabs[i] != null) data.prefabGuids[i] = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_prefabs[i]));

        EditorPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
    }

    /// <summary>
    /// Carica le impostazioni salvate precedentemente.
    /// </summary>
    private void LoadState()
    {
        if (!EditorPrefs.HasKey(SaveKey)) return;

        SpawnerData data = JsonUtility.FromJson<SpawnerData>(EditorPrefs.GetString(SaveKey));
        _spawnCount = data.spawnCount; _areaMin = data.areaMin; _areaMax = data.areaMax;
        _minScale = data.minScale; _maxScale = data.maxScale; _minDistance = data.minDistance;
        _selectedPreset = data.currentPreset;
        
        if (!string.IsNullOrEmpty(data.materialGuid)) 
            _debrisMaterial = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(data.materialGuid));
        
        for (int i = 0; i < 4; i++) 
            if (!string.IsNullOrEmpty(data.prefabGuids[i])) 
                _prefabs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(data.prefabGuids[i]));
    }
}