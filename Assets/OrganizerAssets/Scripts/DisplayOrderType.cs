using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DisplayOrderType : MonoBehaviour
{
    public SortingManager sortingManager; // assign in inspector
    public DropZone[] slots;              // assign all drop zones in order
    public Bucket[] buckets;              // assign all buckets
    public TextMeshProUGUI orderText;     // assign your UI text here
    public bool win;

    private void Update()
    {
        if (!sortingManager) return;
        
        if (sortingManager.rules.Count == 0) return;

        if (sortingManager.rules[0].attribute == SortingRule.AttributeType.Hardness)
        {
            HandleHardnessCheck();
        }
        else if (sortingManager.rules[0].attribute == SortingRule.AttributeType.crystalStructure)
        {
            HandleStructureCheck();
        }
    }

    private bool AllSlotsFilled()
    {
        foreach (var slot in slots)
        {
            if (!slot.currentMineral)
                return false;
        }
        return true;
    }

    private void ShowCurrentOrderTypes()
    {
        List<OrganizerMineral> arranged = slots.Select(slot => slot.currentMineral).ToList();

        string display = "";
        
        foreach (var rule in sortingManager.rules)
        {
            // Only consider ascending rules
            if (!rule.ascending) continue;
            
            if (sortingManager.MatchesRule(arranged, rule))
            {
                display = ($"Current arrangement matches: {rule.name}");
                win = true;
                break; // stop at first match
                // TODO: You can replace this with UI text display instead of Debug.Log
            }
        }

        if (string.IsNullOrEmpty(display))
            display = "No ascending order matched.";
        
        orderText.text = display;
    }

    private void HandleHardnessCheck()
    {
        if (AllSlotsFilled())
        {
            ShowCurrentOrderTypes();
        }
        else
        {
            orderText.text = ""; //clear text if not all slots are filled
            win = false;
        }
    }

    private bool AllBucketsFilled()
    {
        foreach (var bucket in buckets)
        {
            if (bucket.GetMinerals().Count == 0)
                return false;
        }
        return true;
    }

    private bool AllBucketsCorrect()
    {
        foreach (var bucket in buckets)
        {
            foreach (var mineral in bucket.GetMinerals())
            {
                if (mineral.mineralValues.crystalStructure != bucket.crystalStructureValue)
                    return false;
            }
        }
        return true;
    }
    
    private void HandleStructureCheck()
    {
        if (!AllBucketsFilled())
        {
            orderText.text = "";
            win = false;
            return;
        }

        if (AllBucketsCorrect())
        {
            orderText.text = "All minerals are in the right buckets.";
            win = true;
        }
        else
        {
            orderText.text = "Some minerals are in the wrong buckets.";
            win = false;
        }
    }
}