using System.Collections.Generic;
using UnityEngine;

public class Bucket : MonoBehaviour
{
    public int crystalStructureValue; // the structure this bucket accepts

    private List<OrganizerMineral> containedMinerals = new List<OrganizerMineral>();

    public SpriteRenderer structureIcon;

    public SpriteRenderer bucketSprite;
    public Sprite bucketNormal;
    public Sprite bucketWrong;
    
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
            
            UpdateBucketVisual();
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
            
            UpdateBucketVisual();
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

    public bool IsCorrect()
    {
        if (containedMinerals.Count == 0)
            return true;

        foreach (var mineral in containedMinerals)
        {
            if (mineral.mineralValues.crystalStructure != crystalStructureValue)
                return false;
        }

        return true;
    }

    public void UpdateBucketVisual()
    {
        if (containedMinerals.Count == 0)
        {
            bucketSprite.sprite = bucketNormal;
            return;
        }

        if (IsCorrect())
            bucketSprite.sprite = bucketNormal;
        else
            bucketSprite.sprite = bucketWrong;
        
    }
}
