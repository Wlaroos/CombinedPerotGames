using UnityEngine;

public class OrganizerMineral : MonoBehaviour
{
    public MineralData mineralValues;
    private Animator _animator;
    private SpriteRenderer _sr;
    
    [SerializeField] private float snapSpeed = 10f;
    private bool _isSnapping;
    private Vector3 _snapTarget;
    private StructureTool _structureTool;

    public GameObject structureIcon;

    private RespawnZone _currentRespawnZone;
    
    private DropZone _currentDropZone;
    private Bucket _currentBucket;
    public bool CanBeDragged { get; private set; } = true;
    public bool IsUnderScanner { get; private set; }
    public DropZone CurrentDropZone => _currentDropZone;
    public Bucket CurrentBucket => _currentBucket;
    
    public bool hardnessDiscovered;
    public Tray tray;

    public void Awake()
    {
        _structureTool = FindAnyObjectByType<StructureTool>();
        structureIcon = GetComponentInChildren<StructureIcon>().gameObject;
        
        structureIcon.SetActive(false);
    }
    
    private void Start()
    {
        if (mineralValues != null)
        {
            GetComponent<SpriteRenderer>().sprite = mineralValues.mineralBigSprite;
            _animator = GetComponent<Animator>();
            gameObject.name = mineralValues.mineralName.GetLocalizedString();
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
    }

    private void HandleSnapMovement()
    {
        if (tray.isMoving) return;
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
        gameObject.name = mineralValues.mineralName.GetLocalizedString();
    }

    public void SetUnderScanner(bool value)
    {
        IsUnderScanner = value;
        //CanBeDragged = !value; // Scanner overrides dragging
        
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
    
    public void SetDropZone(DropZone newZone)
    {
        if (_currentDropZone == newZone) return;

        // Clear old zone
        if (_currentDropZone != null)
        {
            _currentDropZone.ClearMineral(this);
        }

        _currentDropZone = newZone;

        if (newZone != null)
        {
            newZone.AssignMineral(this);
            
            transform.SetParent(newZone.transform);
            LerpTo(newZone.transform.position);
        }
        else
        {
            if (newZone)
                if(newZone.gameObject.activeInHierarchy)
                    transform.SetParent(null);
        }
    }
    
    public void SetBucket(Bucket bucket)
    {
        if (_currentBucket == bucket) return;
        
        // Remove from old bucket
        if (_currentBucket != null)
            _currentBucket.RemoveMineral(this);
        
        _currentBucket = bucket;

        if (bucket != null)
        {
            bucket.AddMineral(this);
            
            transform.SetParent(bucket.transform);
        }
        else
        {
            transform.SetParent(null);
        }
    }

    public void ClearBucket(Bucket bucket)
    {
        if (_currentBucket == bucket)
            _currentBucket = null;
    }
}
