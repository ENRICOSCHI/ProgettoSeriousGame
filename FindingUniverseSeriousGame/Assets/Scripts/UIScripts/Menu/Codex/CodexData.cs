using UnityEngine;
using TMPro;

// Questo script è una "Libreria" per le classi dati.

[System.Serializable]
public class CodexEntry  // Classe che rappresenta una voce nel Codex, da compilare in Inspector per ogni voce
{
    [Tooltip("Il testo UI nella lista di sinistra (es. l'oggetto '???')")]
    public TMP_Text uiTextElement; 
    
    [Tooltip("Il vero nome che apparirà una volta scoperto")]
    public string realName;
    
    [Tooltip("La spiegazione dettagliata del fenomeno o pianeta")]
    [TextArea(3, 10)] 
    public string description;
    
    [Tooltip("Se falso, mostrerà solo ??? e nasconderà la descrizione")]
    public bool isDiscovered = false; 
}

[System.Serializable]
public class CategoryCodex  // Classe che rappresenta una categoria del Codex, da compilare in Inspector per ogni categoria
{
    [Tooltip("Inserire il componente TextMeshPro del titolo espandibile. \nEs: L'oggetto di testo '[+] PIANETI'")]
    public TMP_Text categoryTitle;  
    
    [Tooltip("Inserire l'oggetto genitore (Empty GameObject) che contiene la lista. \nEs: L'oggetto 'Lista_Voci_Pianeti'")]
    public GameObject categoryList;  
    
    [Tooltip("Il database delle singole voci (Pianeti, Fenomeni, ecc.) di questa categoria")]
    public CodexEntry[] entries;  
    
    [HideInInspector] public bool isOpen = false;  
}