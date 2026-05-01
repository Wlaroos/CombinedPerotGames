using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField] private float _dragSpeed = 25f; // How fast the object follows the mouse

    private Vector2 _mouseOffset; // Distance from mouse to object when dragging starts
    private Element _element;     // Reference if this is an element
    private Compound _compound;   // Reference if this is a compound
    private Mineral _mineral;     // Reference if this is a mineral
    private Collider2D _collider; // The collider we're overlapping with
    private SortingGroup _sg;     // For controlling draw order

    private InputSystem_Actions _inputActions;

    private void Awake()
    {
        _element = GetComponent<Element>();
        _compound = GetComponent<Compound>();
        _mineral = GetComponent<Mineral>();

        if (_element == null && _compound == null && _mineral == null)
        {
            Debug.LogError("DragAndDrop: Missing Element, Compound, or Mineral component");
        }

        _sg = GetComponent<SortingGroup>();
    }

    private void OnEnable()
    {
        _inputActions = new InputSystem_Actions();
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
        _collider = null;
    }

    private Vector2 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDown()
    {
        _mouseOffset = (Vector2)transform.position - GetMouseWorldPosition();
        if (_sg != null)
            _sg.sortingLayerName = "Dragging";
    }

    private void OnMouseDrag()
    {
        Vector2 mouseWorld = GetMouseWorldPosition() + _mouseOffset;

        Vector3 current = transform.position;
        Vector3 desired = new Vector3(mouseWorld.x, mouseWorld.y, current.z);
        Vector3 interpolated = Vector3.Lerp(current, desired, _dragSpeed * Time.deltaTime);

        transform.position = ClampToMainArea(interpolated);
    }

    private bool _externalDragging = false;

    public void StartExternalDrag(Vector3 worldPos)
    {
        _mouseOffset = (Vector2)transform.position - (Vector2)worldPos;
        if (_sg != null)
            _sg.sortingLayerName = "Dragging";
        _externalDragging = true;
    }

    public void UpdateExternalDrag(Vector3 worldPos)
    {
        if (!_externalDragging) return;
        Vector3 newPos = new Vector3(worldPos.x, worldPos.y, transform.position.z);
        transform.position = ClampToMainArea(newPos);
    }

    public void EndExternalDrag()
    {
        if (!_externalDragging) return;
        _externalDragging = false;
        transform.position = ClampToMainArea(transform.position);
        Release();
        if (_sg != null)
            _sg.sortingLayerName = "Default";
    }

    private ScriptableObject GetDataFromGameObject(GameObject obj)
    {
        var element = obj.GetComponent<Element>();
        if (element != null) return element.data;
        var compound = obj.GetComponent<Compound>();
        if (compound != null) return compound.data;
        var mineral = obj.GetComponent<Mineral>();
        if (mineral != null) return mineral.data;
        return null;
    }

    private void OnMouseUp()
    {
        Release();
        if (_sg != null)
            _sg.sortingLayerName = "Default";
    }

    private void Release()
    {
        if (_collider != null)
        {
            GameObject otherObj = _collider.gameObject;

            transform.position = ClampToMainArea(transform.position);
            otherObj.transform.position = ClampToMainArea(otherObj.transform.position);

            ScriptableObject dataA = (ScriptableObject)_element?.data ?? (ScriptableObject)_compound?.data ?? (ScriptableObject)_mineral?.data;
            ScriptableObject dataB = GetDataFromGameObject(otherObj);

            if (dataB == null)
            {
                if (otherObj.name == "TrashButtonBG")
                {
                    Destroy(gameObject);
                }
                return;
            }

            Transform originalParentA = transform.parent;
            Transform originalParentB = otherObj != null ? otherObj.transform.parent : null;

            transform.SetParent(null);
            if (otherObj != null) otherObj.transform.SetParent(null);

            transform.SetParent(originalParentA);
            if (otherObj != null) otherObj.transform.SetParent(originalParentB);

            if (otherObj != null && otherObj.TryGetComponent<DragAndDrop>(out _))
            {
                Vector3 separationDirection = (otherObj.transform.position - transform.position);
                if (separationDirection == Vector3.zero)
                {
                    separationDirection = Random.insideUnitCircle.normalized;
                }
                else
                {
                    separationDirection = separationDirection.normalized;
                }
                
                float separationDistance = 0.33f;
                if (TryGetComponent(out Collider2D colA) && otherObj.TryGetComponent(out Collider2D colB))
                {
                    separationDistance += (colA.bounds.size.magnitude + colB.bounds.size.magnitude) / 4;
                }
                
                transform.position -= separationDirection * separationDistance;
                otherObj.transform.position += separationDirection * separationDistance;

                transform.position = ClampToMainArea(transform.position);
                otherObj.transform.position = ClampToMainArea(otherObj.transform.position);

                Vector3 failPosition = (transform.position + otherObj.transform.position) / 2f;
                EffectManager.Instance.PlayFailEffect(failPosition);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _collider = collision;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_collider == collision)
        {
            _collider = null;
        }
    }

    private void Update()
    {
        if (_inputActions.UI.RightClick.WasPressedThisFrame())
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            if (hit != null && hit.gameObject == gameObject)
            {
                Destroy(gameObject);
            }
        }
    }

    private Vector3 ClampToMainArea(Vector3 worldPosition)
    {
        // Use the BoxCollider2D from CraftingZone as the master clamp reference
        if (CraftingZone.Instance == null || CraftingZone.Instance.Collider == null)
        {
            return worldPosition;
        }

        Bounds zoneBounds = CraftingZone.Instance.Collider.bounds;
        Vector3 halfSize = Vector3.zero;
        
        // Use full extents of the current object's collider to clamp perfectly
        if (TryGetComponent(out Collider2D myCollider))
        {
            halfSize = myCollider.bounds.extents;
        }

        Vector3 min = zoneBounds.min + halfSize;
        Vector3 max = zoneBounds.max - halfSize;

        float x = Mathf.Clamp(worldPosition.x, min.x, max.x);
        float y = Mathf.Clamp(worldPosition.y, min.y, max.y);

        return new Vector3(x, y, worldPosition.z);
    }
}