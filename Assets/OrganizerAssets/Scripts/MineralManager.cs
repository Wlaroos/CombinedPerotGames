using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MineralManager : MonoBehaviour
{
    public static MineralManager instance;
    
    [Header("All available minerals (Scriptable Objects)")]
    public List<MineralData>allMinerals;
    
    [Header("Mineral slots in the scene")]
    public List<OrganizerMineral> minerals = new List<OrganizerMineral>();

    [Header("Game manager")]
    [SerializeField] private OrganizerGameManager gameManager;
    
    [Header("Misc")]
    public List<int> allowedStructures = new List<int>();
    public BoxCollider2D spawnZone;
    
    [SerializeField] private bool enforceUniqueHardness = true;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        LoadAllMinerals();
    }
    
    private void LoadAllMinerals()
    {
        allMinerals = new List<MineralData>(
            Resources.LoadAll<MineralData>("SOs/Minerals"));
    }

    public void RefreshMinerals()
    {
        if (allMinerals.Count < minerals.Count)
        {
            Debug.LogError("There aren't enough unique minerals");
            return;
        }

        if(gameManager.hardnessChosen)
            AssignHardnessMinerals();
        
        if(gameManager.structureChosen)
            AssignStructureMinerals();
    }
    
    public void RespawnMinerals()
    {
        foreach (var mineral in minerals)
        {
            Vector2 newPos = GetRandomPointInBox(spawnZone);
            mineral.transform.position = newPos;
        }
        RefreshMinerals();
    }
    
    private Vector2 GetRandomPointInBox(BoxCollider2D box)
    {
        Vector2 size = box.size;
        Vector2 center = (Vector2)box.transform.position + box.offset;

        float x = Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
        float y = Random.Range(center.y - size.y / 2f, center.y + size.y / 2f);

        return new Vector2(x, y);
    }

    private void SetAllowedStructureFromBuckets()
    {
        allowedStructures.Clear();

        foreach (var bucket in FindObjectsOfType<Bucket>())
        {
            if (!allowedStructures.Contains(bucket.crystalStructureValue))
                allowedStructures.Add(bucket.crystalStructureValue);
        }
    }

    private void AssignBucketStructures()
    {
        Bucket[] buckets = FindObjectsOfType<Bucket>();

        List<int> available = Enumerable.Range(1, 7).ToList();

        foreach (var bucket in buckets)
        {
            if (available.Count == 0) break;
            
            int index = Random.Range(0, available.Count);
            bucket.crystalStructureValue = available[index];

            bucket.UpdateIcon();
            
            available.RemoveAt(index);
        }
    }

    private void AssignHardnessMinerals()
    {
        List<MineralData> pool = new(allMinerals);
        HashSet<int> usedHardness = new();

        foreach (var slot in minerals)
        {
            // Filter valid minerals based on rules
            List<MineralData> validChoices = pool.FindAll(m =>
                (!enforceUniqueHardness || !usedHardness.Contains(m.hardness))
            );

            if (validChoices.Count == 0) return;

            // Pick a random valid mineral
            int index = Random.Range(0, validChoices.Count);
            MineralData chosen = validChoices[index];

            // Assign it
            slot.AssignMineral(chosen);

            // Mark values as used
            usedHardness.Add(chosen.hardness);

            // Remove from master pool
            pool.Remove(chosen);
        }
    }

    private void AssignStructureMinerals()
    {
        List<MineralData> pool = new(allMinerals);
        
        AssignBucketStructures();

        Bucket[] buckets = FindObjectsOfType<Bucket>();
        SetAllowedStructureFromBuckets();

        if (allowedStructures.Count > 0)
        {
            pool.Where(m => allowedStructures.Contains(m.crystalStructure)).ToList();

            int mineralIndex = 0;

            foreach (var bucket in buckets)
            {
                List<MineralData> valid = pool.FindAll(m =>
                    m.crystalStructure == bucket.crystalStructureValue);

                MineralData chosen = valid[Random.Range(0, valid.Count)];
                minerals[mineralIndex].AssignMineral(chosen);
                
                pool.Remove(chosen);
                mineralIndex++;
            }

            for (int i = mineralIndex; i < minerals.Count; i++)
            {
                if (pool.Count == 0) break;

                MineralData chosen = pool[Random.Range(0, pool.Count)];
                minerals[i].AssignMineral(chosen);

                pool.Remove(chosen);
            }
        }
    }
}
