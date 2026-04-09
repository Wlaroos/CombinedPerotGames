using System.Collections.Generic;
using UnityEngine;

public class Bucket : MonoBehaviour
{
    public int crystalStructureValue; // the structure this bucket accepts

    private List<OrganizerMineral> containedMinerals = new List<OrganizerMineral>();

    public SpriteRenderer structureIcon;
    
    [System.Serializable]
    public struct  StructureIcon
    {
        
        public int structureValue;
        public Sprite sprite;
    }

    public List<StructureIcon> icons;

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

    public void UpdateIcon()
    {
        foreach (var icon in icons)
        {
            if (icon.structureValue == crystalStructureValue)
            {
                structureIcon.sprite = icon.sprite;
            }
        }
    }
}
