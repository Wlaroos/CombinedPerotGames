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
        
        currentMineral = mineral;
        mineral.SetDropZone(this);
        mineral.transform.SetParent(transform);
        mineral.LerpTo(transform.position);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OrganizerMineral mineral = other.GetComponent<OrganizerMineral>();
        if (mineral == null) return;
        if (mineral != currentMineral) return;

        mineral.ClearDropZone(this);
        currentMineral = null;
        
        if(mineral.gameObject.activeInHierarchy)
        {
            mineral.transform.SetParent(null);
        }
    }
}
