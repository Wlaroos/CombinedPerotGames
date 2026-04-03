using System.Collections.Generic;
using UnityEngine;

public class OmniTool : MonoBehaviour
{
    private BoxCollider2D detector;
    public OrganizerMineral CurrentMineral { get; private set; }

    private readonly HashSet<OrganizerMineral> mineralsInRange = new();

    void Awake()
    {
        detector = GetComponent<BoxCollider2D>();
        detector.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OrganizerMineral mineral = other.GetComponent<OrganizerMineral>();
        if (!mineral) return;

        mineralsInRange.Add(mineral);
        mineral.SetUnderScanner(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OrganizerMineral mineral = other.GetComponent<OrganizerMineral>();
        if (!mineral) return;
        
        mineralsInRange.Remove(mineral);
        mineral.SetUnderScanner(false);

        if (CurrentMineral == mineral)
            ClearSelection();
    }

    public void SelectMineral(OrganizerMineral mineral)
    {
        if(!mineralsInRange.Contains(mineral)) return;
        
        CurrentMineral = mineral;
    }

    public void ClearSelection()
    {
        CurrentMineral = null;
    }
}
