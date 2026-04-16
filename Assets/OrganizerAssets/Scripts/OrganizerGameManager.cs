using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OrganizerGameManager : MonoBehaviour
{
    public static OrganizerGameManager Instance;

    [Header("HardnessReference")]
    [SerializeField] private GameObject hardnessTutorial;
    [SerializeField] private GameObject hardnessTool;
    [SerializeField] private GameObject hardnessSorting;
    [SerializeField] private GameObject arrangementText;
    [SerializeField, TextArea(3, 10)] private string hardnessToolText;

    [Header("StructureReference")]
    [SerializeField] private GameObject structureTutorial;
    [SerializeField] private GameObject structureTool;
    [SerializeField] private GameObject structureSorting;
    [SerializeField, TextArea(3, 10)] private string structureToolText;

    [Header("GeneralUI")]
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject hardFlavorPanel;
    [SerializeField] private GameObject structFlavorPanel;
    [SerializeField] private GameObject toolFlavorPanel;
    [SerializeField] private TextMeshProUGUI toolInfo;

    [Header("Managers")]
    [SerializeField] private DisplayOrderType displayOrder;
    [SerializeField] private SortingManager sortingManager;
    [SerializeField] private TooltipManager tooltipManager;
    [SerializeField] private MineralManager mineralManager;

    [Header("Misc")]
    [SerializeField] private GameObject lever;
    [SerializeField] private ToolWire wire;
    [SerializeField] private FinalMineralInfoCard _infoCardPrefab; 
    [SerializeField] private Transform _infoCardContainer;

    public bool hardnessChosen;
    public bool structureChosen;
    
    private readonly HashSet<OrganizerMineral> _mineralData = new HashSet<OrganizerMineral>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        OrganizerMineral[] minerals = FindObjectsByType<OrganizerMineral>((FindObjectsSortMode)FindObjectsInactive.Include);

        foreach (var mineral in minerals)
        {
            _mineralData.Add(mineral);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
    
    public void ShowWin()
    {
        winPanel.SetActive(true);
        
        if(hardnessChosen)
            hardnessTool.SetActive(false);
        if(structureChosen)
            structureTool.SetActive(false);
        
        // Clear existing cards if the win screen is reused
        foreach (Transform child in _infoCardContainer)
        {
            Destroy(child.gameObject);
        }

        // Loop through the minerals in the game
        foreach (var mineral in _mineralData)
        {
            FinalMineralInfoCard card = Instantiate(_infoCardPrefab, _infoCardContainer);
            card.Setup(mineral.mineralValues);
        }
    }
    
    public void SubmitCheck()
    {
        if (displayOrder.win)
            ShowWin();
        else
            Debug.Log("Not organized correctly yet.");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ChooseHardness()
    {
        hardnessChosen = true;
        hardnessTutorial.SetActive(true);
        hardnessTool.SetActive(true);
        sortingManager.HardnessArrangement();
        tooltipManager.HardnessActive();
        infoPanel.SetActive(true);
        toolInfo.text = hardnessToolText;
        startScreen.SetActive(false);
        lever.SetActive(true);
        hardnessSorting.SetActive(true);
        arrangementText.SetActive(true);
    }

    public void ChooseStructure()
    {
        structureChosen = true;
        structureTutorial.SetActive(true);
        structureTool.SetActive(true);
        sortingManager.StructureArrangement();
        tooltipManager.ScannerActive();
        infoPanel.SetActive(true);
        toolInfo.text = structureToolText;
        startScreen.SetActive(false);
        structureSorting.SetActive(true);
    }

    public void StartGame()
    {
        wire.Activate();
        if(hardnessChosen)
            hardnessTutorial.SetActive(false);
        
        if(structureChosen)
            structureTutorial.SetActive(false);
        
        infoPanel.SetActive(false);
        hardFlavorPanel.SetActive(false);
        structFlavorPanel.SetActive(false);
        toolFlavorPanel.SetActive(false);
        mineralManager.RespawnMinerals();
    }
}
