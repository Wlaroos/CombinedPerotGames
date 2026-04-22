using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class SortingManager : MonoBehaviour
{
    [SerializeField] private SortingRule hardnessRules;
    [SerializeField] private SortingRule structureRules;
    [SerializeField] private TextMeshProUGUI organizeText;
    [SerializeField] private string hardnessText;
    [SerializeField] private string structureText;

    public SortingRule currentRule;
    public List<Bucket> buckets = new List<Bucket>();
    public Transform[] slots;

    public void HardnessArrangement()
    {
        currentRule = hardnessRules;
        organizeText.text = "Organize by Hardness";
    }
    
    public void StructureArrangement()
    {
        currentRule = structureRules;
        organizeText.text = "Organize by Crystal Structure";
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
        
        if(rule.attribute == SortingRule.AttributeType.Hardness)
        {
            bool ascending = true;
            bool descending = true;
            
            // Check order
            for (int i = 0; i < values.Count - 1; i++)
            {
                if (values[i] > values[i + 1])
                    ascending = false;
                
                if (values[i] < values[i + 1])
                    descending = false;
            }

            return ascending || descending;
        }

        if (rule.attribute == SortingRule.AttributeType.crystalStructure)
        {
            foreach (var bucket in buckets)
            {
                foreach (var mineral in bucket.GetMinerals())
                {
                    if (mineral.mineralValues.crystalStructure != bucket.crystalStructureValue)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        return false;
    }
}
