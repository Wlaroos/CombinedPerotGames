using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingZone : MonoBehaviour
{
    private static CraftingZone _instance;
    public static CraftingZone Instance => _instance;
    private List<GameObject> _objectsInZone = new List<GameObject>();

    // multi-press crafting state
    private bool _craftingInProgress = false;
    private int _requiredPresses = 0;
    private int _currentPresses = 0;
    private List<GameObject> _objectsSnapshot = null;
    private List<ScriptableObject> _snapshotIngredients = null;
    
    private BoxCollider2D _bc;
    // Public property so other scripts can access the collider bounds
    public BoxCollider2D Collider => _bc != null ? _bc : (_bc = GetComponent<BoxCollider2D>());

    [SerializeField] private GameObject[] _dotIndicators; // Visual indicators for crafting progress

    [SerializeField] private int _pressesForCrafting = 5; // Number of presses required to craft an item

    [SerializeField] private GameObject _slagPrefab; // Prefab for slag byproduct
    
    [Tooltip("Total buffer to reduce the spawn area. Half is applied to each side.")]
    [SerializeField] private Vector2 _spawnAreaBuffer = new Vector2(1f, 1f); 
    
    [SerializeField] private Button _craftButton; // Button to trigger crafting
    [SerializeField] private ParticleSystem[] _lavaParticles; // Particle system for lava effect
    private Image _caveBGImage; // Reference to the cave background image
    private Color _craftButtonDefaultColor = Color.white;
    private Color _caveBGDefaultColor = new Color32(210, 210, 210, 255);

    private void Awake()
    {
        _bc = GetComponent<BoxCollider2D>();
        _instance = this;
        _caveBGImage = GetComponent<Image>();

        if (_craftButton != null)
        {
            var img = _craftButton.GetComponent<Image>();
            if (img != null) _craftButtonDefaultColor = img.color;
            UpdateCraftButtonState();
        }
    }

    // Called when an object enters the zone
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_objectsInZone.Contains(collision.gameObject) && collision.CompareTag("Draggable"))
        {
            _objectsInZone.Add(collision.gameObject);
            ResetCraftingState();
            UpdateCraftButtonState();
        }
    }

    // Called when an object exits the zone
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_objectsInZone.Contains(collision.gameObject) && collision.CompareTag("Draggable"))
        {
            _objectsInZone.Remove(collision.gameObject);
            ResetCraftingState();
            UpdateCraftButtonState();
        }
    }

    // Get the data of all objects in the zone
    public List<ScriptableObject> GetIngredients()
    {
        List<ScriptableObject> ingredients = new List<ScriptableObject>();
        foreach (var obj in _objectsInZone)
        {
            var element = obj.GetComponent<Element>();
            if (element != null) ingredients.Add(element.data);

            var compound = obj.GetComponent<Compound>();
            if (compound != null) ingredients.Add(compound.data);

            var mineral = obj.GetComponent<Mineral>();
            if (mineral != null) ingredients.Add(mineral.data);
        }
        return ingredients;
    }

    // Trigger crafting when the button is pressed
    public void Craft()
    {
        List<ScriptableObject> ingredients = GetIngredients();
        if (ingredients.Count == 0)
        {
            ResetCraftingState();
            return;
        }

        // If we are not already in a multi-press sequence, initialize crafting state
        if (!_craftingInProgress)
        {
            _craftingInProgress = true;
            _currentPresses = 1;
            _requiredPresses = _pressesForCrafting;
            _objectsSnapshot = new List<GameObject>(_objectsInZone);
            _snapshotIngredients = new List<ScriptableObject>(ingredients);

            UpdateDotIndicators();
            MoveObjectsCloser(_objectsSnapshot);
        }
        else
        {
            // Already in a multi-press sequence
            _currentPresses++;
            UpdateDotIndicators();

            // Use the snapshot so players cannot change ingredients mid-way
            if (_objectsSnapshot == null || _objectsSnapshot.Count == 0)
            {
                // fallback to current zone if snapshot was lost
                _objectsSnapshot = new List<GameObject>(_objectsInZone);
            }

            MoveObjectsCloser(_objectsSnapshot);

            if (_currentPresses >= _requiredPresses)
            {
                // Finalize crafting at the end of the sequence
                FinalizeCraft(_snapshotIngredients ?? ingredients, _objectsSnapshot);
                _caveBGImage.color = _caveBGDefaultColor;
            }
        }

    }

    private void MoveObjectsCloser(List<GameObject> objects)
    {
        if (objects == null || objects.Count == 0) return;

        // compute center of existing objects
        Vector3 center = Vector3.zero;
        int count = 0;
        foreach (var obj in objects)
        {
            if (obj != null)
            {
                center += obj.transform.position;
                count++;
            }
        }
        if (count == 0) return;
        center /= count;

        // Move each object a fraction toward the center; fraction chosen so they converge after required presses
        float t = _requiredPresses > 0 ? 1f / _requiredPresses : 0.25f;
        foreach (var obj in objects)
        {
            if (obj == null) continue;
            obj.transform.position = Vector3.Lerp(obj.transform.position, center, t);
        }
    }

    private void FinalizeCraft(List<ScriptableObject> ingredients, List<GameObject> objectsToConsume)
    {
        // Center of all objects in a zone [ONE OR THE OTHER]
        Vector3 spawnPosition = ComputeCenterOf(objectsToConsume) ?? transform.position;

        // Unparent the objects first so they no longer count toward the DraggableHolder child count.
        List<Transform> originalParents = new List<Transform>();
        foreach (var obj in objectsToConsume)
        {
            originalParents.Add(obj != null ? obj.transform.parent : null);
            if (obj != null)
            {
                obj.transform.SetParent(null);
            }
        }

        // Attempt to craft the result.
        GameObject craftedObj = CraftingManager.Instance != null ? CraftingManager.Instance.TryCraft(ingredients, spawnPosition) : null;

        if (craftedObj != null)
        {
            // Successful craft, remove the consumed objects
            foreach (var obj in objectsToConsume)
            {
                if (obj != null)
                {
                    Destroy(obj);
                    _objectsInZone.Remove(obj);
                }
            }

            ResetCraftingState();
        }
        else
        {
            // Craft failed, restore original parenting
            foreach (var obj in objectsToConsume)
            {
                if (obj != null)
                {
                    Destroy(obj);
                    _objectsInZone.Remove(obj);
                }
            }

            Instantiate(_slagPrefab, spawnPosition, Quaternion.identity, DraggableHolder.Instance.transform);
            CraftedPopupManager.Instance?.ShowSlagPopup();

            Debug.Log("Crafting failed at finalization: No matching recipe -- Created Slag as byproduct.");
            ResetCraftingState();
        }
    }

    private Vector3? ComputeCenterOf(List<GameObject> objects)
    {
        if (objects == null || objects.Count == 0) return null;
        Vector3 center = Vector3.zero;
        int count = 0;
        foreach (var obj in objects)
        {
            if (obj != null)
            {
                center += obj.transform.position;
                count++;
            }
        }
        if (count == 0) return null;
        return center / count;
    }

    // Returns a random world-space position inside the box collider, inset by _spawnAreaBuffer (half buffer per side)
    public Vector3 GetRandomSpawnPosition()
    {
        BoxCollider2D bc = Collider;
        if (bc == null) return transform.position;

        // Apply half buffer to each side to reduce spawn area by the total buffer value
        Vector2 halfBuffer = _spawnAreaBuffer * 0.5f;

        Vector3 min = bc.bounds.min + new Vector3(halfBuffer.x, halfBuffer.y, 0f);
        Vector3 max = bc.bounds.max - new Vector3(halfBuffer.x, halfBuffer.y, 0f);

        // Ensure valid ranges: if buffer too large, collapse to center
        if (min.x > max.x) { float mid = (min.x + max.x) * 0.5f; min.x = max.x = mid; }
        if (min.y > max.y) { float mid = (min.y + max.y) * 0.5f; min.y = max.y = mid; }

        float x = Random.Range(min.x, max.x);
        float y = Random.Range(min.y, max.y);
        return new Vector3(x, y, transform.position.z);
    }

    private void ResetCraftingState()
    {
        _craftingInProgress = false;
        _requiredPresses = 0;
        _currentPresses = 0;
        _objectsSnapshot = null;
        _snapshotIngredients = null;

        _caveBGImage.color = _caveBGDefaultColor;

        ResetDotIndicators();
        UpdateCraftButtonState();
    }

    private void UpdateCraftButtonState()
    {
        if (_craftButton == null) return;

        var img = _craftButton.GetComponent<Image>();
        if (img == null) return;

        var ingredients = GetIngredients();
        bool hasRecipe = false;
        if (ingredients != null && ingredients.Count > 0 && CraftingManager.Instance != null)
        {
            hasRecipe = CraftingManager.Instance.FindMatchingRecipe(ingredients) != null;
        }

        if (hasRecipe)
        {
            img.color = new Color32(132, 255, 181, 255);
        }
        else
        {
            img.color = _craftButtonDefaultColor;
        }
    }

    private void UpdateDotIndicators()
    {
        if (_dotIndicators == null || _dotIndicators.Length == 0) return;

        for (int i = 0; i < _dotIndicators.Length; i++)
        {
            if (_dotIndicators[i] != null)
            {
                _dotIndicators[i].GetComponent<Image>().color = (i < _currentPresses) ? Color.green : Color.red;
            }
        }

        _caveBGImage.color = Color.Lerp(_caveBGDefaultColor, new Color32(255, 100, 100, 255), (float)_currentPresses / _requiredPresses);
        foreach (var ps in _lavaParticles) ps.Play();
    }

    private void ResetDotIndicators()
    {
        if (_dotIndicators == null || _dotIndicators.Length == 0) return;

        for (int i = 0; i < _dotIndicators.Length; i++)
        {
            if (_dotIndicators[i] != null)
            {
                _dotIndicators[i].GetComponent<Image>().color = Color.red;
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the BoxCollider2D bounds and the inner spawn area rectangle
        BoxCollider2D bc = GetComponent<BoxCollider2D>();
        if (bc == null) return;

        // Full collider bounds
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(bc.bounds.center, bc.bounds.size);

        // Buffered inner area
        Vector2 halfBuffer = _spawnAreaBuffer * 0.5f;
        Vector3 min = bc.bounds.min + new Vector3(halfBuffer.x, halfBuffer.y, 0f);
        Vector3 max = bc.bounds.max - new Vector3(halfBuffer.x, halfBuffer.y, 0f);

        if (min.x > max.x) { float mid = (min.x + max.x) * 0.5f; min.x = max.x = mid; }
        if (min.y > max.y) { float mid = (min.y + max.y) * 0.5f; min.y = max.y = mid; }

        Vector3 center = (min + max) * 0.5f;
        Vector3 size = new Vector3(max.x - min.x, max.y - min.y, 0.1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);
    }
}