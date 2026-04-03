using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DisplayOrderType : MonoBehaviour
{
    public SortingManager sortingManager; // assign in inspector
    public DropZone[] slots;              // assign all drop zones in order
    public TextMeshProUGUI orderText;                // assign your UI text here
    public bool win;

    private void Update()
    {
        if (AllSlotsFilled())
        {
            ShowCurrentOrderTypes();
        }
        else
        {
            orderText.text = ""; //clear text if not all slots are filled
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
}