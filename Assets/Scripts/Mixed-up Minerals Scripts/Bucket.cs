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
            containedMinerals.Add(mineral);
            
        mineral.SetBucket(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OrganizerMineral mineral = other.GetComponent<OrganizerMineral>();
        if(mineral == null) return;

        if (mineral.CurrentBucket == this)
        {
            mineral.SetBucket(null);
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

    public void ResetVisual()
    {
        bucketSprite.sprite = bucketNormal;
    }
    
    public void AddMineral(OrganizerMineral mineral)
    {
        if (!containedMinerals.Contains(mineral))
            containedMinerals.Add(mineral);
    }

    public void RemoveMineral(OrganizerMineral mineral)
    {
        containedMinerals.Remove(mineral);
    }
}
