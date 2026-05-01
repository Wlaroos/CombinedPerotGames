using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class ElementSpawner : MonoBehaviour
{
    private static ElementSpawner _instance;
    public static ElementSpawner Instance => _instance;

    [SerializeField] private GameObject elementPrefab;
    [SerializeField] private Button[] _spawnButtons;
    [SerializeField] private Sprite _hiddenButtonSprite;
    [SerializeField] private bool _unlockAllElements = false;
    [SerializeField] private bool _canFlipTile = true;

    private List<ElementData> _elementDataList = new List<ElementData>();
    private Sprite[] _spawnButtonSprites;
    private ElementData[] _buttonDataMap;
    private bool _isDragging;

    public bool IsDragging => _isDragging;
    public void SetDragging(bool value) => _isDragging = value;
    
    private void Awake()
    {
        SetupSingleton();
        InitializeButtons();
        LoadElementData();
        SetupButtonListeners();

        if (_unlockAllElements) UnlockAllButtons();
    }

    private void SetupSingleton()
    {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else _instance = this;
    }

    private void InitializeButtons()
    {
        if (_spawnButtons == null) _spawnButtons = new Button[0];
        _spawnButtonSprites = new Sprite[_spawnButtons.Length];
        _buttonDataMap = new ElementData[_spawnButtons.Length];

        for (int i = 0; i < _spawnButtons.Length; i++)
        {
            var button = _spawnButtons[i];
            if (button == null) continue;
            var img = button.targetGraphic?.GetComponent<Image>();
            _spawnButtonSprites[i] = img != null ? img.sprite : null;
            button.interactable = false;
            if (img != null) img.sprite = _hiddenButtonSprite;
        }
    }

    private void LoadElementData()
    {
        _elementDataList.Clear();
        _elementDataList.AddRange(Resources.LoadAll<ElementData>("SOs/Elements"));
    }

    private void SetupButtonListeners()
    {
        for (int i = 0; i < _spawnButtons.Length; i++)
        {
            var button = _spawnButtons[i];
            if (button == null) continue;

            var matchedData = FindMatchingElementData(button.name);
            if (matchedData != null)
            {
                _buttonDataMap[i] = matchedData;
                SetupButton(button, matchedData);
            }
        }
    }

    private ElementData FindMatchingElementData(string buttonName)
    {
        return _elementDataList.Find(data =>
            data != null && buttonName.IndexOf(SOHelpers.StripCommonPrefix(data.name), System.StringComparison.OrdinalIgnoreCase) >= 0);
    }

    private void SetupButton(Button button, ElementData data)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnSpawnButtonClicked(button, data));

        var dragHandler = button.gameObject.GetComponent<SpawnDragHandler>() ?? button.gameObject.AddComponent<SpawnDragHandler>();
        dragHandler.Init(this, data);
    }

    private void OnSpawnButtonClicked(Button button, ElementData data)
    {
        if (_isDragging) return;

        if(_canFlipTile)
        {
            button.transform.localScale = new Vector3(1f, 1f, 1f);
            StartCoroutine(FlipTile(button, data));
        }
        else
        {
            SpawnElementAtRandomPosition(data);
        }
    }

    private IEnumerator FlipTile(Button button, ElementData data)
    {
        float elapsedTime = 0f;
        float flipDuration = 0.25f;
        Image childImage = button.transform.GetChild(0).gameObject.GetComponent<Image>();

        while (elapsedTime < flipDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / flipDuration);
            float scaleX = Mathf.Lerp(1f, 0f, t);
            button.transform.localScale = new Vector3(scaleX, 1f, 1f);
            yield return null;
        }

        if (childImage != null)
        {
            childImage.sprite = childImage.sprite == data.altElementSprite ? data.elementSprite : data.altElementSprite;
        }

        elapsedTime = 0f;
        while (elapsedTime < flipDuration)        
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / flipDuration);
            float scaleX = Mathf.Lerp(0f, 1f, t);
            button.transform.localScale = new Vector3(scaleX, 1f, 1f);
            yield return null;
        }
    }

    public GameObject SpawnElementAtRandomPosition(ElementData data, int isotopeNumber = -1)
    {
        // Use the shared random position logic from CraftingZone
        if (CraftingZone.Instance != null)
        {
            Vector3 worldPosition = CraftingZone.Instance.GetRandomSpawnPosition();
            return SpawnElementAtPosition(data, isotopeNumber, worldPosition);
        }
        return SpawnElementAtPosition(data, isotopeNumber, Vector3.zero);
    }

    public GameObject SpawnElementAtPosition(ElementData data, int isotopeNumber, Vector3 position)
    {
        if (elementPrefab == null)
        {
            Debug.LogError("ElementSpawner: Cannot spawn element, prefab is not assigned.");
            return null;
        }

        GameObject newElement = Instantiate(elementPrefab, position, Quaternion.identity);
        if (!InitializeElement(newElement, data, isotopeNumber)) return null;

        DraggableHolder.Instance?.AddDraggable(newElement);
        return newElement;
    }

    private bool InitializeElement(GameObject element, ElementData data, int isotopeNumber)
    {
        var elementComponent = element.GetComponent<Element>();
        if (elementComponent == null)
        {
            Destroy(element);
            return false;
        }

        elementComponent.data = data;
        elementComponent.isotopeNumber = isotopeNumber < 0 ? data.defaultIsotopeNumber : isotopeNumber;
        elementComponent.UpdateDataVisuals();
        return true;
    }

    public void UnlockAllButtons()
    {
        for (int i = 0; i < _spawnButtons.Length; i++)
        {
            var button = _spawnButtons[i];
            if (button == null) continue;
            button.interactable = true;

            var img = button.targetGraphic?.GetComponent<Image>();
            if (img != null)
            {
                var mapped = (i >= 0 && i < _buttonDataMap.Length) ? _buttonDataMap[i] : null;
                img.sprite = _spawnButtonSprites[i];
                if (mapped != null)
                {
                    img.color = SOHelpers.GetColorFromData(mapped);
                }
            }
        }
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