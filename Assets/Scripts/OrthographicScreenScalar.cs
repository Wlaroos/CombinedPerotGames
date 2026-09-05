using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class OrthographicScreenScaler : MonoBehaviour
{
    [Header("Target Reference Settings")]
    [SerializeField] private float referenceOrthographicSize = 5f;
    [SerializeField] private Vector2 referenceResolution = new Vector2(16, 9);

    private Camera cam;
    private int lastScreenWidth;
    private int lastScreenHeight;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        UpdateCameraSize();
    }

    // LateUpdate guarantees this runs after game logic and right before rendering
    private void LateUpdate()
    {
        CheckScreenSizeChange();
    }

    private void CheckScreenSizeChange()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            UpdateCameraSize();
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }

    public void UpdateCameraSize()
    {
        if (cam == null) cam = GetComponent<Camera>();

        // Safety check
        if (Screen.height == 0 || referenceResolution.y == 0) return;

        float targetAspect = referenceResolution.x / referenceResolution.y;
        float currentAspect = (float)Screen.width / (float)Screen.height;

        // Adjust the orthographic size based on the aspect ratio difference
        cam.orthographicSize = referenceOrthographicSize * (targetAspect / currentAspect);
    }
}