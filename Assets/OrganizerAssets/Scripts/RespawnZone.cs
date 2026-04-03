using System.Collections.Generic;
using UnityEngine;

public class RespawnZone : MonoBehaviour
{
    private readonly HashSet<OrganizerMineral> mineralsInside = new();

    private void OnTriggerEnter2D(Collider2D other)
    {
        OrganizerMineral mineral = other.GetComponent<OrganizerMineral>();
        if (mineral == null) return;

        mineralsInside.Add(mineral);
        mineral.SetRespawnZone(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OrganizerMineral mineral = other.GetComponent<OrganizerMineral>();
        if (mineral == null) return;

        mineralsInside.Remove(mineral);
        mineral.ClearRespawnZone(this);
    }

    public Vector3 GetRespawnPoint()
    {
        return transform.position;
    }
}
