using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class CodexNavigation : MenuNavigation
{
    [SerializeField] protected CodexManager codexManager; // Il "Cervello Centrale" da cui leggere il database dei pianeti/fenomeni

    protected override void Awake()
    {
        base.Awake();
        if (codexManager == null) Debug.LogError("CodexManager non trovato! CodexNavigation ha bisogno di CodexManager per leggere i dati.");
    }

    #region "metodi astratti"
    protected override bool IsManagerValid() => codexManager == null || codexManager.categoryLists == null || codexManager.categoryLists.Length == 0;
    protected override int GetCategoryCount() => codexManager.categoryLists.Length;
    protected override int GetEntryCount(int categoryIndex) => codexManager.categoryLists[categoryIndex].entries.Length;
    protected override CategoryMenu GetMenuCategory(int index) => codexManager.categoryLists[index];
    protected override TMP_Text GetEntryUIText(int categoryIndex, int entryIndex) => codexManager.categoryLists[categoryIndex].entries[entryIndex].uiTextElement;
    protected override string GetEntryDisplayName(int categoryIndex, int entryIndex)
    {
        var entry = codexManager.categoryLists[categoryIndex].entries[entryIndex];
        return entry.isDiscovered ? entry.realName : "???";
    }
    #endregion

    protected override void UpdateRightPanelDisplay()
    {
        base.UpdateRightPanelDisplay();

        // Evita errori se la lista è vuota
        if (selectableTexts.Count == 0 || currentCategoryIndex >= selectableTexts.Count) return;

        bool isCategory = isNodeCategory[currentCategoryIndex];
        int linkedCatIndex = indexLinkedCategory[currentCategoryIndex];
        int linkedEntryIndex = indexLinkedEntry[currentCategoryIndex];

        if (isCategory)
        {
            // Se siamo sopra "[+] PIANETI", svuotiamo lo schermo di destra
            ClearRightPanel();
        }
        else
        {
            // Se siamo sopra un pianeta specifico, peschiamo i suoi dati dal Manager
            CodexEntry selectedData = codexManager.categoryLists[linkedCatIndex].entries[linkedEntryIndex];

            if (selectedData.isDiscovered)
            {
                // Dato scoperto: mostriamo Titolo e Descrizione
                if (rightPanelTitleText != null) rightPanelTitleText.text = selectedData.realName;
                if (rightPanelDescriptionText != null) rightPanelDescriptionText.text = selectedData.description;
            }
            else
            {
                // Dato nascosto: mostriamo l'avviso di dati mancanti
                if (rightPanelTitleText != null) rightPanelTitleText.text = "???";
                if (rightPanelDescriptionText != null) rightPanelDescriptionText.text = "DATI MANCANTI.\nESPLORARE IL SISTEMA PER ACQUISIRE INFORMAZIONI.";
            }
        }
    }
}