using UnityEngine;
using UnityEditor;

/// <summary>
/// Strumento di rifinitura per l'ottimizzazione massiva di asteroidi già presenti nella scena.
/// Gestisce l'allineamento dei materiali per l'SRP Batcher, la disattivazione delle ombre 
/// per il risparmio di Draw Calls e la risoluzione delle compenetrazioni fisiche.
/// </summary>
public class AsteroidFineTuner : EditorWindow
{
    // ─── Configurazione Asset ────────────────────────────────────────────────

    [Header("Impostazioni di Ottimizzazione")]
    [SerializeField] private Material _commonMaterial;

    // ─── Lifecycle Window ────────────────────────────────────────────────────

    [MenuItem("Tools/Asteroid Fine-Tuner Panel")]
    public static void ShowWindow()
    {
        GetWindow<AsteroidFineTuner>("Asteroid Tuner");
    }

    // ─── Interfaccia Utente (GUI) ────────────────────────────────────────────

    private void OnGUI()
    {
        GUILayout.Label("Ottimizzazione Asteroidi Esistenti", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Questo tool processa la selezione corrente per abbattere i Batch e correggere la fisica.", MessageType.Info);

        EditorGUILayout.Space(5);
        _commonMaterial = (Material)EditorGUILayout.ObjectField("Materiale Comune", _commonMaterial, typeof(Material), false);

        EditorGUILayout.Space(10);

        // Il tasto di esecuzione viene abilitato solo se c'è una selezione valida
        GUI.enabled = Selection.gameObjects.Length > 0;
        if (GUILayout.Button("Sistema Selezione (Fisica + Batches)", GUILayout.Height(40)))
        {
            ProcessSelection();
        }
        GUI.enabled = true;
    }

    // ─── Logica di Elaborazione ──────────────────────────────────────────────

    /// <summary>
    /// Itera sugli oggetti selezionati applicando trasformazioni e correzioni al Renderer.
    /// Utilizza il sistema di Undo per garantire la reversibilità delle modifiche in Editor.
    /// </summary>
    private void ProcessSelection()
    {
        GameObject[] selected = Selection.gameObjects;

        if (selected.Length == 0)
        {
            Debug.LogWarning("[AsteroidTuner] Nessun oggetto selezionato nell'Hierarchy.");
            return;
        }

        // Registrazione dello stato iniziale per permettere il ripristino tramite Ctrl+Z
        Undo.RecordObjects(Selection.transforms, "Fine-Tune Transforms");
        
        foreach (GameObject obj in selected)
        {
            // ─── 1. OTTIMIZZAZIONE PERFORMANCE (BATCHES) ───
            MeshRenderer mr = obj.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                Undo.RecordObject(mr, "Optimize Renderer");
                
                // L'assegnazione dello sharedMaterial è vitale: evita la creazione di 
                // istanze di materiale uniche che romperebbero l'SRP Batcher.
                if (_commonMaterial != null) mr.sharedMaterial = _commonMaterial;
                
                // Disattivazione del calcolo delle ombre. In scene con 1000+ oggetti, 
                // questo riduce i Batches di un fattore cinematico (fino al 70%).
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
            }

            // ─── 2. ROTAZIONE DELICATA ───
            // Applica una variazione angolare casuale per rompere la ripetitività visiva.
            obj.transform.Rotate(
                Random.Range(-10f, 10f),
                Random.Range(-10f, 10f),
                Random.Range(-10f, 10f)
            );

            // ─── 3. SEPARAZIONE FISICA ───
            SeparateFromNeighbors(obj);
        }

        Debug.Log($"[AsteroidTuner] Bonifica completata su {selected.Length} oggetti.");
    }

    // ─── Gestione Collisioni Editor ──────────────────────────────────────────

    /// <summary>
    /// Rileva eventuali compenetrazioni con collider adiacenti e sposta l'oggetto 
    /// lungo il vettore di minima separazione.
    /// </summary>
    /// <param name="obj">L'oggetto da testare e spostare.</param>
    private static void SeparateFromNeighbors(GameObject obj)
    {
        Collider col = obj.GetComponent<Collider>();
        if (col == null) return;

        // Esegue una scansione volumetrica leggermente superiore ai bounds dell'oggetto (buffer 1.2x)
        Collider[] neighbors = Physics.OverlapBox(col.bounds.center, col.bounds.extents * 1.2f);

        foreach (var other in neighbors)
        {
            // Salta il test se l'altro collider è lo stesso oggetto o un suo figlio
            if (other == col || other.gameObject.transform.IsChildOf(obj.transform)) continue;

            Vector3 direction;
            float distance;

            // Calcola il vettore di espulsione per risolvere l'overlap dei volumi
            bool overlapped = Physics.ComputePenetration(
                col, obj.transform.position, obj.transform.rotation,
                other, other.transform.position, other.transform.rotation,
                out direction, out distance
            );

            if (overlapped)
            {
                // Sposta l'oggetto della distanza necessaria più un offset di sicurezza ($0.1f$)
                obj.transform.position += direction * (distance + 0.1f);
                
                // Applica una rotazione tangenziale per una disposizione più naturale
                obj.transform.Rotate(direction * 5f);
            }
        }
    }
}