using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CodexManager : MonoBehaviour, IHandleJSON
{
    #region Inizializzazione variabili
    [Header("Struttura Codex (Il Database)")]
    [Tooltip("Configura qui le macro-categorie (Pianeti, Fenomeni, Musica).")]
    public CategoryCodex[] categoryLists;

    private Dictionary<string, bool> OggettiSbloccatiDizionario = new Dictionary<string, bool>();
    #endregion


    #region Metodi Unity (Ciclo di Vita)

    void OnEnable()
    {
        // Sottoscrivo il metodo SaveGame all'evento di salvataggio
        DelegateClass.SaveEventHandler += Save;
        DelegateClass.LoadEventHandler += Load;
    }

    void OnDisable()
    {
        // Rimuovo la sottoscrizione quando l'oggetto viene disabilitato
        DelegateClass.SaveEventHandler -= Save;
        DelegateClass.LoadEventHandler -= Load;
    }


    /*void Start()
    {
        Load(PersistentSceneData.Instance.isChangingScene);
    }*/
    #endregion

    #region Metodi Pubblici
    /// <summary>
    /// Sblocco l'oggetto indicato nel codex
    /// </summary>
    /// <param name="categoryIndex"> indice della categoria 0: pianeti, 1: eventi, 2: musica</param>
    /// <param name="entryIndex"> indice della entrata 0: Mercurio, 1: Venere,ecc..</param>
    public void UnlockMenuEntry(int categoryIndex, int entryIndex)
    {
        if (categoryIndex >= 0 && categoryIndex < categoryLists.Length)
        {
            if (entryIndex >= 0 && entryIndex < categoryLists[categoryIndex].entries.Length)
            {
                categoryLists[categoryIndex].entries[entryIndex].isDiscovered = true;

                if (!OggettiSbloccatiDizionario.ContainsKey(categoryLists[categoryIndex].entries[entryIndex].ID))
                {
                    OggettiSbloccatiDizionario.Add(
                        categoryLists[categoryIndex].entries[entryIndex].ID,
                        categoryLists[categoryIndex].entries[entryIndex].isDiscovered
                    );
                }

                Debug.Log($"Menu Aggiornato: Sbloccato {categoryLists[categoryIndex].entries[entryIndex].realName}!");
            }
        }
    }
    #endregion

    #region Implementazione Interfaccia IHandleJSON
    public void SaveGame<TKey, TValue>(Dictionary<TKey, TValue> data)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);

        string path = ManagerHandler.ManagerInstance.SaveManager.GetPathForCodex();

        File.WriteAllText(path, json);

        Debug.Log("salvato in: " + path);
    }


    public void Save(bool isChangingLevel)
    {
        SaveGame<string, bool>(OggettiSbloccatiDizionario);
    }

    public bool CheckJsonFile()
    {
        return File.Exists(ManagerHandler.ManagerInstance.SaveManager.GetPathForCodex());
    }

    public Dictionary<TKey, TValue> LoadJson<TKey, TValue>()
    {
        string path = ManagerHandler.ManagerInstance.SaveManager.GetPathForCodex();

        if (path != null)
        {
            string json = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(json);
        }
        else return new Dictionary<TKey, TValue>();
    }

    public void Load(bool isChangingLevel)
    {
        #region Fetch Dati dal JSON

        if (CheckJsonFile())
        {
            OggettiSbloccatiDizionario = LoadJson<string, bool>();
        }
        #endregion

        #region Caricamento Dati


        foreach (var category in categoryLists)
        {
            if (category.categoryList != null)
            {
                foreach (var entry in category.entries)
                {
                    if (OggettiSbloccatiDizionario.TryGetValue(entry.ID, out bool isDiscovered))
                    {
                        entry.isDiscovered = isDiscovered;
                    }
                    else
                    {
                        category.categoryList.SetActive(false);
                        category.isOpen = false;
                    }
                }
            }
        }
        #endregion
        Debug.Log("Codex Manager Caricato");

        PersistentSceneData.Instance.isChangingScene = false;
    }
    #endregion
}
