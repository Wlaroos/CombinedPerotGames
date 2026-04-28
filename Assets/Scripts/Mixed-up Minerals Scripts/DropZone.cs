using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DropZone : MonoBehaviour
{
    [HideInInspector] public OrganizerMineral currentMineral;
    [SerializeField] private float exitDistance; // tune this as needed
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        OrganizerMineral mineral = other.GetComponent<OrganizerMineral>();
        if (mineral == null) return;
        if (currentMineral != null) return; // if zone is already occupied ignore
        
        mineral.SetDropZone(this);
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OrganizerMineral mineral = other.GetComponent<OrganizerMineral>();
        if (mineral == null) return;
        if (mineral != currentMineral) return;

        if (mineral.CurrentDropZone == this)
        {
            mineral.SetDropZone(null);
        }
    }
    
    public void AssignMineral(OrganizerMineral mineral)
    {
        currentMineral = mineral;
    }

    public void ClearMineral(OrganizerMineral mineral)
    {
        if (currentMineral == mineral)
            currentMineral = null;
    }
}
