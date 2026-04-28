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

    private bool showingResults = false;

    private void Update()
    {
        if (!sortingManager) return;
        
        if (!sortingManager.currentRule) return;

        if (showingResults && Input.GetMouseButtonDown(0))
        {
            ResetAllBucketVisuals();
            ResetText();
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
        
        if (sortingManager.MatchesRule(arranged, sortingManager.currentRule))
        {
            win = true;
        }
        else
            display = "No order matched.";
        
        orderText.text = display;
    }

    public void HandleHardnessCheck()
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

    private bool AllMineralsUsed()
    {
        int totalInBuckets = 0;

        foreach (var bucket in buckets)
            totalInBuckets += bucket.GetMinerals().Count;
        
        int totalMinerals = FindObjectsByType<OrganizerMineral>((FindObjectsSortMode)FindObjectsInactive.Include).Length;

        return totalInBuckets == totalMinerals;
    }

    public void HandleStructureCheck()
    {
        if (!AllBucketsFilled() || !AllMineralsUsed())
        {
            orderText.text = "";
            win = false;
            return;
        }

        if (AllBucketsCorrect())
        {
            win = true;
        }
        else
        {
            orderText.text = "Some minerals are in the wrong buckets.";
            win = false;
        }
    }

    private void ResetText()
    {
        orderText.text = "";
    }
    
    public void ResetAllBucketVisuals()
    {
        orderText.text = "";
        foreach (var bucket in buckets)
            bucket.ResetVisual();
    }

    public void SetShowingResults(bool value)
    {
        showingResults = value;
    }
}