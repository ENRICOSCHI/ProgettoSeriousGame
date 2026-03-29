using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class MenuAsthetics : MonoBehaviour
{

    [Header("Estetica TabBar (Asteroid Style)")]
    [Tooltip("Inserire i testi dei bottoni nell'ordine corretto: 0 = Minimap, 1 = Missioni, 2 = Codex, 3 = Opzioni")]
    [SerializeField] private TMP_Text[] tabButtonTexts;  // Array di testi dei bottoni della TabBar, da assegnare in Inspector

    [Header("Colori")]
    [SerializeField] private Color activeTabColor = Color.cyan;  // Colore del testo del bottone attivo
    [SerializeField] private Color inactiveTabColor = Color.gray;  // Colore del testo dei bottoni inattivi


    /// <summary>
    /// Chiamato dal controller principale per aggiornare i colori dei bottoni della TabBar in base al pannello attivo
    /// </summary>
    public void UpdateTabVisuals(int activeIndex)
    {
        // Controllo di sicurezza
        if(tabButtonTexts == null || tabButtonTexts.Length == 0)
        {
            Debug.LogWarning("Tab Button Texts non assegnati in MenuAsthetics. Assicurati di assegnarli in Inspector.");
            return;
        }

        // Ciclo su tutti i testi per accendere quello giusto e spegnere gli altri
        for(int i = 0; i < tabButtonTexts.Length; i++)
        {
            if(i == activeIndex)
            {
                tabButtonTexts[i].color = activeTabColor;  // Colore attivo per il bottone attivo
            }
            else
            {
                tabButtonTexts[i].color = inactiveTabColor;  // Colore inattivo per gli altri bottoni
            }
        }
    }


    /// <summary>
    /// Sostituisce l'array dei testi da colorare. Utile per liste dinamiche come il menu codex
    /// </summary>
    public void SetTexts(TMP_Text[] newTabButtonTexts)
    {
        tabButtonTexts = newTabButtonTexts;
    }
}
