using UnityEngine;
using UnityEngine.UI;

public class CompoundSpawner : MonoBehaviour
{
    private static CompoundSpawner _instance;
    public static CompoundSpawner Instance => _instance;

    [SerializeField] private GameObject compoundPrefab;
    [SerializeField] private Button[] _spawnButtons;
    [SerializeField] private Sprite _hiddenButtonSprite;
    [SerializeField] private bool _unlockAllCompounds = false;
    [SerializeField] private bool _canClick = true;

    private CompoundData[] _compoundDataList;
    private Sprite[] _spawnButtonSprites;
    private CompoundData[] _buttonDataMap; // map per-button assigned SO
    private bool _isDragging;
    public bool IsDragging => _isDragging;
    public void SetDragging(bool value) => _isDragging = value;

    private void Awake()
    {
        SetupSingleton();
        EnsureArrays();
        CacheButtonSpritesAndLock();
        LoadCompoundDataIfNeeded();
        SetupSpawnButtons();
    }

    private void SetupSingleton()
    {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else _instance = this;
    }

    private void EnsureArrays()
    {
        if (_spawnButtons == null) _spawnButtons = new Button[0];
        _spawnButtonSprites = new Sprite[_spawnButtons.Length];
        _buttonDataMap = new CompoundData[_spawnButtons.Length];
    }

    private void CacheButtonSpritesAndLock()
    {
        for (int i = 0; i < _spawnButtons.Length; i++)
        {
            var btn = _spawnButtons[i];
            if (btn == null) continue;
            var img = btn.targetGraphic as Image;
            _spawnButtonSprites[i] = img != null ? img.sprite : null;
            btn.interactable = false;
            if (img != null) img.sprite = _hiddenButtonSprite;
        }
    }

    private void LoadCompoundDataIfNeeded()
    {
        if (_compoundDataList == null || _compoundDataList.Length == 0)
        {
            var loaded = Resources.LoadAll<CompoundData>("SOs/Compounds");
            if (loaded != null && loaded.Length > 0) _compoundDataList = loaded;
        }
    }

    private void SetupSpawnButtons()
    {
        for (int i = 0; i < _spawnButtons.Length; i++)
        {
            Button btn = _spawnButtons[i];
            if (btn == null) continue;
            string btnName = btn.name;

            CompoundData matchedData = null;
            if (_compoundDataList != null)
            {
                matchedData = System.Array.Find(_compoundDataList, c => c != null
                    && !string.IsNullOrEmpty(btnName)
                    && btnName.IndexOf(GetCompoundBaseName(c.name), System.StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (matchedData != null)
            {
                _buttonDataMap[i] = matchedData; 
                btn.onClick.RemoveAllListeners();
                CompoundData dataCopy = matchedData; 
                btn.onClick.AddListener(() => OnSpawnButtonClicked(dataCopy));

                SpawnDragHandler dragHandler = btn.gameObject.GetComponent<SpawnDragHandler>() ?? btn.gameObject.AddComponent<SpawnDragHandler>();
                dragHandler.Init(this, dataCopy);
            }
        }
    }

    private void OnSpawnButtonClicked(CompoundData data)
    {
        if (_isDragging) return;
        if (!_canClick) return;

        SpawnCompoundAtRandomPosition(data);
    }

    private void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }

    private void Start()
    {
        if (_unlockAllCompounds) UnlockAllButtons();
    }

    public GameObject SpawnCompoundAtRandomPosition(CompoundData data)
    {
        // Use the shared random position logic from CraftingZone
        if (CraftingZone.Instance != null)
        {
            Vector3 worldPosition = CraftingZone.Instance.GetRandomSpawnPosition();
            return SpawnCompoundAtPosition(data, worldPosition);
        }
        return SpawnCompoundAtPosition(data, Vector3.zero);
    }

    public GameObject SpawnCompoundAtPosition(CompoundData data, Vector3 position)
    {
        if (compoundPrefab == null)
        {
            Debug.LogError("CompoundSpawner: Cannot spawn compound, prefab is not assigned.");
            return null;
        }

        GameObject newCompound = Instantiate(compoundPrefab, position, Quaternion.identity);
        var comp = newCompound.GetComponent<Compound>();
        if (comp == null)
        {
            Destroy(newCompound);
            return null;
        }

        comp.data = data;
        comp.UpdateDataVisuals();
        DraggableHolder.Instance?.AddDraggable(newCompound);
        return newCompound;
    }

    public void UnlockAllButtons()
    {
        for (int i = 0; i < _spawnButtons.Length; i++)
        {
            var btn = _spawnButtons[i];
            if (btn == null) continue;
            btn.interactable = true;
            var img = btn.targetGraphic as Image ?? btn.image;
            if (img != null && _spawnButtonSprites != null && i < _spawnButtonSprites.Length && _spawnButtonSprites[i] != null)
            {
                img.sprite = _spawnButtonSprites[i];
            }

            var mapped = (i >= 0 && i < _buttonDataMap.Length) ? _buttonDataMap[i] : null;
            if (img != null)
            {
                img.color = (mapped != null) ? SOHelpers.GetColorFromData(mapped) : Color.white;
            }
        }
    }

    private string GetCompoundBaseName(string soName)
    {
        if (string.IsNullOrEmpty(soName)) return soName;
        int first = soName.IndexOf('_');
        return first >= 0 ? soName.Substring(first + 1) : soName;
    }

    public void EnableDragHandler(bool enable)
    {
        foreach (var button in _spawnButtons)
        {
            if (button == null) continue;
            var dragHandler = button.gameObject.GetComponent<SpawnDragHandler>();
            if (dragHandler != null) 
            {
                button.interactable = enable;
                dragHandler.enabled = enable;
            }
        }
    }
}