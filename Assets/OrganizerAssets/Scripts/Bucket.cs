using System.Collections.Generic;
using UnityEngine;

public class Bucket : MonoBehaviour
{
    public int crystalStructureValue; // the structure this bucket accepts

    private int spacing;
    private List<OrganizerMineral> containedMinerals = new List<OrganizerMineral>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        OrganizerMineral mineral = other.GetComponent<OrganizerMineral>();
        if (mineral == null) return;

        if (!containedMinerals.Contains(mineral))
        {
            containedMinerals.Add(mineral);
            mineral.SetBucket(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OrganizerMineral mineral = other.GetComponent<OrganizerMineral>();
        if(mineral == null) return;

        if (containedMinerals.Contains(mineral))
        {
            containedMinerals.Remove(mineral);
            mineral.ClearBucket(this);
        }
    }

    public List<OrganizerMineral> GetMinerals()
    {
        return containedMinerals;
    }

    public void AssignRandomStructure(int min = 1, int max = 7)
    {
        crystalStructureValue = Random.Range(min, max);
    }
}
