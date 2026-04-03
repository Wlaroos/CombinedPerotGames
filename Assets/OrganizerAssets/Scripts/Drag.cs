using UnityEngine;

public class Drag : MonoBehaviour
{
    private bool dragging;
    private Vector3 offset;
    private Camera cam;

    private OrganizerMineral mineral;

    private void Awake()
    {
        cam = Camera.main;
        mineral = GetComponent<OrganizerMineral>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dragging) return;
        
        // Move object, take into account original offset
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector3 targetPos = mousePosition + offset;
        
        // Camera bounds
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float minX = cam.transform.position.x - camWidth;
        float maxX = cam.transform.position.x + camWidth;
        float minY = cam.transform.position.y - camHeight;
        float maxY = cam.transform.position.y + camHeight;
        
        float halfW = GetComponent<SpriteRenderer>().bounds.extents.x;
        float halfH = GetComponent<SpriteRenderer>().bounds.extents.y;

        targetPos.x = Mathf.Clamp(targetPos.x, minX + halfW, maxX - halfW);
        targetPos.y = Mathf.Clamp(targetPos.y, minY + halfH, maxY - halfH);
        
        transform.position = targetPos;
    }

    private void OnMouseDown()
    {
        if (mineral && !mineral.CanBeDragged)
            return;
        
        // Record the difference between the objects center, and the clicked point on the camera plane
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        offset = transform.position - mousePosition;
        dragging = true;
    }

    private void OnMouseUp()
    {
        // Stop dragging
        dragging = false;
        
        // Check which DropZone we are over
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
        foreach (var hit in hits)
        {
            DropZone zone = hit.GetComponent<DropZone>();
            if (zone)
            {
                // Tell us we are dropped here
                zone.currentMineral = GetComponent<OrganizerMineral>();
            }
        }
        
        // Check sorting after release
        SortingManager manager = FindFirstObjectByType<SortingManager>();
        if (manager)
        {
            manager.CheckArrangement();
        }
    }
}
