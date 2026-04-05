using UnityEngine;

public class OrganizerMineral : MonoBehaviour
{
    public MineralData mineralValues;
    private Animator _animator;
    private SpriteRenderer _sr;
    
    [SerializeField] private float snapSpeed = 10f;
    private bool _isSnapping;
    private Vector3 _snapTarget;
    private OmniTool _omniTool;
    private StructureTool _structureTool;

    public GameObject clickIcon;
    public GameObject structureIcon;

    private RespawnZone _currentRespawnZone;
    
    private DropZone _currentDropZone;
    public bool CanBeDragged { get; private set; } = true;
    public bool IsUnderScanner { get; private set; }
    
    public bool hardnessDiscovered;

    public void Awake()
    {
        _omniTool = FindAnyObjectByType<OmniTool>();
        _structureTool = FindAnyObjectByType<StructureTool>();
        clickIcon = GetComponentInChildren<ClickIcon>().gameObject;
        structureIcon = GetComponentInChildren<StructureIcon>().gameObject;
        
        clickIcon.SetActive(false);
        structureIcon.SetActive(false);
    }
    
    private void Start()
    {
        if (mineralValues != null)
        {
            GetComponent<SpriteRenderer>().sprite = mineralValues.mineralBigSprite;
            _animator = GetComponent<Animator>();
            gameObject.name = mineralValues.mineralName;
        }
        
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleSnapMovement();
    }

    private void OnMouseDown()
    {
        if (!IsUnderScanner) return;
        if (!_omniTool) return;
        _omniTool.SelectMineral(this);
    }

    private void HandleSnapMovement()
    {
        if (!_isSnapping) return;

        // Stop snapping when close enough
        if (Vector3.Distance(transform.position, _snapTarget) < 0.01f)
        {
            transform.position = _snapTarget;
            _isSnapping = false;
        }
        
        transform.position = Vector3.Lerp(transform.position, _snapTarget, snapSpeed * Time.deltaTime);
    }
    
    public void LerpTo(Vector3 target)
    {
        _snapTarget = target;
        _isSnapping = true;
    }

    public void AssignMineral(MineralData newValues)
    {
        mineralValues = newValues;
        
        // Update animator if needed
        if (_animator == null)
            _animator = GetComponent<Animator>();
        
        // Update sprite + name
        if (_sr == null)
            _sr = GetComponent<SpriteRenderer>();

        _sr.sprite = mineralValues.mineralBigSprite;
        _sr.color = mineralValues.defaultColor;
        gameObject.name = mineralValues.mineralName;
    }

    public void SetUnderScanner(bool value)
    {
        IsUnderScanner = value;
        CanBeDragged = !value; // Scanner overrides dragging

        if (clickIcon && _omniTool)
            clickIcon.SetActive(value);
        
        if (structureIcon && _structureTool)
            structureIcon.SetActive(value);
    }
    
    public void SetRespawnZone(RespawnZone zone)
    {
        _currentRespawnZone = zone;
    }

    public void ClearRespawnZone(RespawnZone zone)
    {
        if (_currentRespawnZone == zone)
            _currentRespawnZone = null;
    }
    
    public void SetDropZone(DropZone zone)
    {
        _currentDropZone = zone;
    }

    public void ClearDropZone(DropZone zone)
    {
        if (_currentDropZone == zone)
            _currentDropZone = null;
    }
}
