using System.Collections.Generic;
using UnityEngine;

public class StructureTool : MonoBehaviour
{
    private BoxCollider2D detector;
    public List<OrganizerMineral> mineralsInRange = new();
    
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
    }
}
