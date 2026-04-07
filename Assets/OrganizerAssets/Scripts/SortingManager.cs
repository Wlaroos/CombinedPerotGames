using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class SortingManager : MonoBehaviour
{
    [SerializeField] private SortingRule hardnessRules;
    [SerializeField] private SortingRule structureRules;
    [SerializeField] private SortingRule colorRules;
    [SerializeField] private TextMeshProUGUI organizeText;
    
    public List<SortingRule> rules = new List<SortingRule>();
    public Transform[] slots;

    public void HardnessArrangement()
    {
        if (rules.Count > 0)
            rules.Clear();
        rules.Add(hardnessRules);
        organizeText.text = "Organize by Hardness";
    }
    
    public void StructureArrangement()
    {
        if (rules.Count > 0)
            rules.Clear();
        rules.Add(structureRules);
        organizeText.text = "Organize by Crystal Structure";
    }
    public void ColorArrangement()
    {
        if (rules.Count > 0)
            rules.Clear();
        rules.Add(colorRules);
        organizeText.text = "Organize by Color";
    }

    public void AllArrangement()
    {
        if (rules.Count > 0)
            rules.Clear();
        HardnessArrangement();
        StructureArrangement();
        ColorArrangement();
        organizeText.text = "Organize by Any";
    }
    
    public void CheckArrangement()
    {
        // Gather minerals in slot order
        List<OrganizerMineral> arranged = new List<OrganizerMineral>();
        foreach (Transform slot in slots)
        {
            if (slot.childCount > 0)
            {
                OrganizerMineral m = slot.GetChild(0).GetComponent<OrganizerMineral>();
                if (m) arranged.Add(m);
            }
        }
        
        // Now test against each rule
        foreach (var rule in rules)
        {
            if (MatchesRule(arranged, rule))
            {
                //Debug.Log($"Player discovered {rule.name}");
            }
        }
    }

    public bool MatchesRule(List<OrganizerMineral> minerals, SortingRule rule)
    {
        if (minerals.Count < 2) return false;
        
        // Extract attribute values
        List<float> values = minerals.Select(m =>
        {
            switch (rule.attribute)
            {
                case SortingRule.AttributeType.Hardness: return m.mineralValues.hardness;
                case SortingRule.AttributeType.crystalStructure: return m.mineralValues.crystalStructure;
                default: return 0f;
            }
        }).ToList();
        
        // Check order
        for (int i = 0; i < values.Count - 1; i++)
        {
            if (rule.ascending && values[i] > values[i + 1]) return false;
            if (!rule.ascending && values[i] < values[i + 1]) return false;
        }

        return true;
    }
}
