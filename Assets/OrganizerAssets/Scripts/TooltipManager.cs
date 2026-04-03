using TMPro;
using UnityEditor.TerrainTools;
using UnityEngine;

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
                
                string minStructure = values.crystalStructure switch
                {
                    1 => "Cubic",
                    2 => "Tetragonal",
                    3 => "Hexagonal",
                    4 => "Rhombohedral",
                    5 => "Orthorhombic",
                    6 => "Monoclinic",
                    7 => "Triclinic",
                    _ => "Unknown"
                };
                
                sb.AppendLine($"Mineral: {values.mineralName}");
                sb.AppendLine($"Structure: {minStructure}");
                sb.AppendLine("");
            }
        }

        if (_hardnessActive && hardnessTool && hardnessTool.currentMineral)
        {
            hasContent = true;

            var mineral = hardnessTool.currentMineral;
            var values = mineral.mineralValues;

            sb.AppendLine($"Mineral: {values.mineralName}");
            
            if(mineral.hardnessDiscovered)
                sb.AppendLine($"Hardness: {values.hardness}");
            else
                sb.AppendLine($"Hardness: ???");
            
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
