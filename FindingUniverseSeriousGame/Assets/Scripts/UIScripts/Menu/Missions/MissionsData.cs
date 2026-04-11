using UnityEngine;
using TMPro;

// Questo script è una "Libreria" per le classi dati.

[System.Serializable]
public class MissionEntry  // Classe che rappresenta una voce nel Menu delle Missioni, da compilare in Inspector per ogni voce
{
    [Tooltip("ID di riconoscimento per i salvataggi")]
    public string ID;

    [Tooltip("Il testo UI nella lista di sinistra")]
    public TMP_Text uiTextElement; 
    
    [Tooltip("Il nome della missione (es. 'Recupero Dati')")]
    public string missionName;
    
    [Tooltip("L'obiettivo da leggere nello schermo di destra")]
    [TextArea(3, 10)] 
    public string objectiveDescription;

    [Tooltip("Cosa ottieni finendo la missione (es. Crediti, XP)")]
    public int rewardAmount;
    
    [Tooltip("Se falso: Da fare. Se vero: Completata.")]
    public bool isCompleted = false;
}

[System.Serializable]
public class CategoryMission  
{
    [Tooltip("Inserire il componente TextMeshPro del titolo espandibile. \nEs: L'oggetto di testo '[+] Scoperta'")]
    public TMP_Text categoryTitle;  
    [Tooltip("Inserire l'oggetto genitore (Empty GameObject) che contiene la lista. \nEs: L'oggetto 'List_Discovery'")]
    public GameObject categoryList;
    [Tooltip("Il database delle singole voci (Scoperta, Raccolta, ecc.) di questa categoria")]
    public MissionEntry[] entries;  

    [HideInInspector] public bool isOpen = false;  
}