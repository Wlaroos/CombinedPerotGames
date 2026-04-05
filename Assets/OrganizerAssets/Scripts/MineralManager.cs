using UnityEngine;
using System.Collections.Generic;

public class MineralManager : MonoBehaviour
{
    public static MineralManager instance;
    
    [Header("All available minerals (Scriptable Objects)")]
    public List<MineralData>allMinerals;
    
    [Header("Mineral slots in the scene")]
    public List<OrganizerMineral> minerals = new List<OrganizerMineral>();

    public BoxCollider2D spawnZone;
    
    [SerializeField] private bool enforceUniqueHardness = true;
    [SerializeField] private bool enforceUniqueStructure = true;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        LoadAllMinerals();
    }

    private void Start()
    {
        RespawnMinerals();
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
        
        List<MineralData> pool = new(allMinerals);

        HashSet<int> usedHardness = new();
        HashSet<int> usedStructures = new();
        
        foreach (var slot in minerals)
        {
            // Filter valid minerals based on rules
            List<MineralData> validChoices = pool.FindAll(m =>
                (!enforceUniqueHardness || !usedHardness.Contains(m.hardness)) &&
                (!enforceUniqueStructure || !usedStructures.Contains(m.crystalStructure))
            );

            if (validChoices.Count == 0) return;
            
            // Pick a random valid mineral
            int index = Random.Range(0, validChoices.Count);
            MineralData chosen = validChoices[index];
            
            // Assign it
            slot.AssignMineral(chosen);
            
            // Mark values as used
            usedHardness.Add(chosen.hardness);
            //usedStructures.Add(chosen.crystalStructure);

            // Remove from master pool
            pool.Remove(chosen);
        }
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
}
