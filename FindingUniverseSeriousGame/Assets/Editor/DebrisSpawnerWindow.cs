using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class DebrisSpawnerWindow : EditorWindow
{
    // Definizione dei tuoi Preset (Aggiungine quanti ne vuoi qui)
    private enum PresetType { Manuale, FasciaAsteroidi, NuvolaDensa, CampoDisperso }

    [Serializable]
    private class SpawnerData
    {
        public string[] prefabGuids = new string[4];
        public int spawnCount = 50;
        public Vector3 areaMin = new Vector3(-500, -500, -500);
        public Vector3 areaMax = new Vector3(500, 500, 500);
        public float minScale = 200f;
        public float maxScale = 400f;
        public float minDistance = 50f;
        public PresetType currentPreset = PresetType.Manuale;
    }

    [SerializeField] private GameObject[] _prefabs = new GameObject[4];
    [SerializeField] private int _spawnCount = 50;
    [SerializeField] private Vector3 _areaMin = new Vector3(-500, -500, -500);
    [SerializeField] private Vector3 _areaMax = new Vector3(500, 500, 500);
    [SerializeField] private float _minScale = 200f;
    [SerializeField] private float _maxScale = 400f;
    [SerializeField] private float _minDistance = 50f;
    [SerializeField] private GameObject _parentObject;
    [SerializeField] private PresetType _selectedPreset = PresetType.Manuale;

    private const string SaveKey = "DebrisSpawner_Settings_V2";

    [MenuItem("Tools/Debris Spawner Panel")]
    public static void ShowWindow() => GetWindow<DebrisSpawnerWindow>("Debris Spawner");

    private void OnEnable() => LoadState();
    private void OnDisable() => SaveState();

    private void OnGUI()
    {
        GUILayout.Label("Configurazione Preset", EditorStyles.boldLabel);
        
        // Sezione Preset
        EditorGUI.BeginChangeCheck();
        _selectedPreset = (PresetType)EditorGUILayout.EnumPopup("Usa Preset:", _selectedPreset);
        if (EditorGUI.EndChangeCheck() && _selectedPreset != PresetType.Manuale)
        {
            ApplyPreset(_selectedPreset);
        }

        EditorGUILayout.Space(10);
        GUILayout.Label("Asset dei Detriti", EditorStyles.boldLabel);
        for (int i = 0; i < 4; i++)
        {
            _prefabs[i] = (GameObject)EditorGUILayout.ObjectField($"Prefab {i + 1}", _prefabs[i], typeof(GameObject), false);
        }

        EditorGUILayout.Space(10);
        GUILayout.Label("Parametri di Generazione", EditorStyles.boldLabel);
        _parentObject = (GameObject)EditorGUILayout.ObjectField("Oggetto Padre", _parentObject, typeof(GameObject), true);
        
        // Se l'utente cambia un valore manualmente, il preset torna su "Manuale"
        EditorGUI.BeginChangeCheck();
        
        _spawnCount = EditorGUILayout.IntField("Quantità", _spawnCount);
        _areaMin = EditorGUILayout.Vector3Field("Area Min", _areaMin);
        _areaMax = EditorGUILayout.Vector3Field("Area Max", _areaMax);
        _minScale = EditorGUILayout.FloatField("Scala Minima", _minScale);
        _maxScale = EditorGUILayout.FloatField("Scala Massima", _maxScale);
        _minDistance = EditorGUILayout.FloatField("Distanza Minima", _minDistance);

        if (EditorGUI.EndChangeCheck())
        {
            _selectedPreset = PresetType.Manuale;
        }

        EditorGUILayout.Space(20);
        GUI.enabled = (_parentObject != null && _prefabs[0] != null);
        if (GUILayout.Button("Genera Detriti", GUILayout.Height(30)))
        {
            SpawnDebris();
            SaveState();
        }
        GUI.enabled = true;
    }

    // --- SEZIONE MODIFICABILE MANUALMENTE ---
    private void ApplyPreset(PresetType type)
    {
        switch (type)
        {
            case PresetType.FasciaAsteroidi:
                _spawnCount = 120;
                _areaMin = new Vector3(-1500, -50, -1500);
                _areaMax = new Vector3(1500, 50, 1500);
                _minScale = 250f;
                _maxScale = 500f;
                _minDistance = 100f;
                break;

            case PresetType.NuvolaDensa:
                _spawnCount = 150;
                _areaMin = new Vector3(-20, -20, -20);
                _areaMax = new Vector3(70, 70, 70);
                _minScale = 2f;
                _maxScale = 5f;
                _minDistance = 6f;
                break;

            case PresetType.CampoDisperso:
                _spawnCount = 30;
                _areaMin = new Vector3(-3000, -3000, -3000);
                _areaMax = new Vector3(3000, 3000, 3000);
                _minScale = 400f;
                _maxScale = 800f;
                _minDistance = 300f;
                break;
        }
    }

    // --- LOGICA DI SALVATAGGIO E SPAWN (Invariata) ---
    private void SaveState()
    {
        SpawnerData data = new SpawnerData {
            spawnCount = _spawnCount, areaMin = _areaMin, areaMax = _areaMax,
            minScale = _minScale, maxScale = _maxScale, minDistance = _minDistance,
            currentPreset = _selectedPreset
        };
        for (int i = 0; i < 4; i++) if (_prefabs[i] != null) data.prefabGuids[i] = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_prefabs[i]));
        EditorPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
    }

    private void LoadState()
    {
        if (!EditorPrefs.HasKey(SaveKey)) return;
        SpawnerData data = JsonUtility.FromJson<SpawnerData>(EditorPrefs.GetString(SaveKey));
        _spawnCount = data.spawnCount; _areaMin = data.areaMin; _areaMax = data.areaMax;
        _minScale = data.minScale; _maxScale = data.maxScale; _minDistance = data.minDistance;
        _selectedPreset = data.currentPreset;
        for (int i = 0; i < 4; i++) if (!string.IsNullOrEmpty(data.prefabGuids[i])) _prefabs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(data.prefabGuids[i]));
    }

    private void SpawnDebris()
    {
        List<GameObject> validPrefabs = new List<GameObject>();
        foreach (var p in _prefabs) if (p != null) validPrefabs.Add(p);
        int spawned = 0; int attempts = 0; List<Vector3> spawnedPositions = new List<Vector3>();
        while (spawned < _spawnCount && attempts < _spawnCount * 20)
        {
            attempts++;
            Vector3 randomPos = new Vector3(UnityEngine.Random.Range(_areaMin.x, _areaMax.x), UnityEngine.Random.Range(_areaMin.y, _areaMax.y), UnityEngine.Random.Range(_areaMin.z, _areaMax.z));
            bool tooClose = false;
            foreach (Vector3 pos in spawnedPositions) if (Vector3.Distance(randomPos, pos) < _minDistance) { tooClose = true; break; }
            if (!tooClose)
            {
                GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(validPrefabs[UnityEngine.Random.Range(0, validPrefabs.Count)]);
                newObj.transform.position = randomPos; newObj.transform.rotation = UnityEngine.Random.rotation;
                float s = UnityEngine.Random.Range(_minScale, _maxScale); newObj.transform.localScale = new Vector3(s, s, s);
                newObj.transform.SetParent(_parentObject.transform); newObj.layer = 7;
                Undo.RegisterCreatedObjectUndo(newObj, "Spawn Debris");
                spawnedPositions.Add(randomPos); spawned++;
            }
        }
    }
}