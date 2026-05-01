using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class TooltipManager : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject tooltipPanel;
    public GameObject objectToFollow;
    
    [Header("Tools")]
    public StructureTool structureTool;
    public HardnessTool hardnessTool;
    
    [Header("Misc")]
    public TextMeshProUGUI tooltipText;
    public Vector2 offset;
    
    private RectTransform panelRect;
    private bool _hardnessActive = false;
    private bool _structureActive = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tooltipPanel.SetActive(false);
        panelRect = tooltipPanel.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!structureTool && !hardnessTool) return;
        if (!objectToFollow) return;
        
        HandlePanel();
        ShowTooltip();
    }

    private void HandlePanel()
    {
        Vector2 toolBase = objectToFollow.transform.position;
        Vector2 desiredPos = toolBase + offset;
        panelRect.position = desiredPos;
    }

    public void ShowTooltip()
    {
        System.Text.StringBuilder sb = new();

        bool hasContent = false;

        tooltipPanel.SetActive(true);
        tooltipText.text = sb.ToString();
        
        tooltipPanel.SetActive(true);
        
        if(_structureActive)
        {
            hasContent = true;
            foreach (var minerals in structureTool.mineralsInRange)
            {
                var values = minerals.mineralValues;

                // Localized mineral name
                string localizedMineralName = values.mineralName.GetLocalizedString();

                // Localized crystal structure
                LocalizedString structureLocalized = SOHelpers.GetCrystalStructureFromData(values);
                string localizedStructure = structureLocalized.GetLocalizedString();

                var nameLabel = new LocalizedString { TableReference = "MixedMineral_TranslationTable", TableEntryReference = "MuM_Reader_01 (D)" }.GetLocalizedString();
                var structureLabel = new LocalizedString { TableReference = "MixedMineral_TranslationTable", TableEntryReference = "MuM_Reader_03 (D)" }.GetLocalizedString();

                sb.AppendLine($"{nameLabel}{localizedMineralName}");
                sb.AppendLine($"{structureLabel}{localizedStructure}");
                sb.AppendLine("");
            }
        }

        if (_hardnessActive && hardnessTool && hardnessTool.currentMineral)
        {
            hasContent = true;

            var mineral = hardnessTool.currentMineral;
            var values = mineral.mineralValues;

            string localizedMineralName = values.mineralName.GetLocalizedString();

            var nameLabel2 = new LocalizedString { TableReference = "MixedMineral_TranslationTable", TableEntryReference = "MuM_Reader_01 (D)" }.GetLocalizedString();
            var hardnessLabel = new LocalizedString { TableReference = "MixedMineral_TranslationTable", TableEntryReference = "MuM_Reader_02 (D)" }.GetLocalizedString();

            sb.AppendLine($"{nameLabel2}{localizedMineralName}");

            if (mineral.hardnessDiscovered)
            {
                string hardnessValue = SOHelpers.GetHardnessFromData(values);
                sb.AppendLine($"{hardnessLabel}{hardnessValue}");
            }
            else
            {
                sb.AppendLine(hardnessLabel + "???");
            }

            sb.AppendLine("");
        }
        
        if (!hasContent)
        {
            tooltipPanel.SetActive(false);
            return;
        }

        tooltipPanel.SetActive(true);
        tooltipText.text = sb.ToString();
    }

    public void ScannerActive()
    {
        _structureActive = true;
        structureTool = FindAnyObjectByType<StructureTool>();
    }

    public void HardnessActive()
    {
        _hardnessActive = true;
        hardnessTool = FindAnyObjectByType<HardnessTool>();
    }
}
