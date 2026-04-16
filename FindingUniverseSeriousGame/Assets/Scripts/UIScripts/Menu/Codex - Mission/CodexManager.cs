using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CodexManager : MonoBehaviour//, IHandleJSON
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
    }

    void OnDisable()
    {
        // Rimuovo la sottoscrizione quando l'oggetto viene disabilitato
        DelegateClass.SaveEventHandler -= Save;
    }


    void Awake()
    {

        // Chiudiamo le cartelle fisicamente all'avvio
        foreach (var category in categoryLists)
        {
            if (category.categoryList != null) category.categoryList.SetActive(false);
            category.isOpen = false;
        }
    }
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

                OggettiSbloccatiDizionario.Add(
                    categoryLists[categoryIndex].entries[entryIndex].ID, 
                    categoryLists[categoryIndex].entries[entryIndex].isDiscovered
                );

                /*foreach (var item in OggettiSbloccatiDizionario)
                {
                    Debug.Log($"Chiave: {item.Key} - Valore: {item.Value}");
                }*/

                Debug.Log($"Menu Aggiornato: Sbloccato {categoryLists[categoryIndex].entries[entryIndex].realName}!");


                /*
                 Per il salvataggio si potrebbe fare una roba tipo: creo lista con ID di quello che ho sbloccato
                 poi al caricamento dei dati controllo gli ID che ho salvato e rimetto quelle parti attive nel codex con isDiscovered = true
                 */
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


    public void Save()
    {
        SaveGame<string, bool>(OggettiSbloccatiDizionario);
    }

    public bool CheckJsonFile()
    {
        return File.Exists(ManagerHandler.ManagerInstance.SaveManager.GetPathForCodex());
    }

    #endregion
}