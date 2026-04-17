using UnityEngine;
using TMPro;

public class MissionNavigation : MenuNavigation
{
    [SerializeField] MissionManager missionManager; // Il "Cervello Centrale" da cui leggere il database dei pianeti/fenomeni

    #region "metodi astratti"
    protected override bool IsManagerValid() => missionManager == null || missionManager.categoryLists == null || missionManager.categoryLists.Length == 0;
    protected override int GetCategoryCount() => missionManager.categoryLists.Length;
    protected override int GetEntryCount(int categoryIndex) => missionManager.categoryLists[categoryIndex].entries.Length;
    protected override CategoryMenu GetMenuCategory(int index) => missionManager.categoryLists[index];
    protected override TMP_Text GetEntryUIText(int categoryIndex, int entryIndex) => missionManager.categoryLists[categoryIndex].entries[entryIndex].uiTextElement;
    protected override string GetEntryDisplayName(int categoryIndex, int entryIndex)
    {
        var entry = missionManager.categoryLists[categoryIndex].entries[entryIndex];
        return entry.isDiscovered ? entry.realName : "???";
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        if (missionManager == null) Debug.LogError("CodexManager non trovato! CodexNavigation ha bisogno di CodexManager per leggere i dati.");
    }


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
            // Se siamo sopra una categoria, svuotiamo lo schermo di destra
            ClearRightPanel();
        }
        else
        {
            // Se siamo sopra un elemento specifico, peschiamo i suoi dati dal Manager
            MissionEntry selectedData = missionManager.categoryLists[linkedCatIndex].entries[linkedEntryIndex];

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
