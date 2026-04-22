using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OrganizerGameManager : MonoBehaviour
{
    public static OrganizerGameManager Instance;

    [Header("Hardness Reference")]
    [SerializeField] private GameObject hardnessTutorial;
    [SerializeField] private GameObject hardnessTool;
    [SerializeField] private GameObject hardnessSorting;
    [SerializeField] private GameObject whatIsHardness;
    [SerializeField] private GameObject hardToolFText;
    public bool hardnessChosen;
    [SerializeField, TextArea(3, 10)] private string hardnessToolText;

    [Header("Structure Reference")]
    [SerializeField] private GameObject structureTutorial;
    [SerializeField] private GameObject structureTool;
    [SerializeField] private GameObject structureSorting;
    [SerializeField] private GameObject whatIsStructure;
    [SerializeField] private GameObject structToolFText;
    public bool structureChosen;
    [SerializeField, TextArea(3, 10)] private string structureToolText;

    [Header("Win UI")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private FinalMineralInfoCard _infoCardPrefab; 
    [SerializeField] private Transform _infoCardContainer;
    
    [Header("General UI")]
    [SerializeField] private GameObject startScreen;
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
        SceneManager.LoadScene("LevelSelect");
    }

    public void ChooseHardness()
    {
        hardnessChosen = true;
        hardnessTutorial.SetActive(true);
        sortingManager.HardnessArrangement();
        infoPanel.SetActive(true);
        whatIsHardness.SetActive(true);
        toolInfo.text = hardnessToolText;
        startScreen.SetActive(false);
        lever.SetActive(true);
        hardnessSorting.SetActive(true);
    }

    public void ChooseStructure()
    {
        structureChosen = true;
        structureTutorial.SetActive(true);
        sortingManager.StructureArrangement();
        infoPanel.SetActive(true);
        whatIsStructure.SetActive(true);
        toolInfo.text = structureToolText;
        startScreen.SetActive(false);
        structureSorting.SetActive(true);
    }

    public void StartGame()
    {
        if(hardnessChosen)
        {
            hardnessTutorial.SetActive(false);
            hardnessTool.SetActive(true);
            tooltipManager.HardnessActive();
        }
        
        if(structureChosen)
        {
            structureTutorial.SetActive(false);
            structureTool.SetActive(true);
            tooltipManager.ScannerActive();
        }
        
        wire.Activate();
        infoPanel.SetActive(false);
        hardFlavorPanel.SetActive(false);
        structFlavorPanel.SetActive(false);
        toolFlavorPanel.SetActive(false);
        mineralManager.RespawnMinerals();
    }

    public void HardnessFlavor()
    {
        hardnessTutorial.SetActive(false);
        toolFlavorPanel.SetActive(false);
        hardFlavorPanel.SetActive(true);
    }

    public void StructureFlavor()
    {
        structureTutorial.SetActive(false);
        toolFlavorPanel.SetActive(false);
        structFlavorPanel.SetActive(true);
    }

    public void ToolFlavor()
    {
        toolFlavorPanel.SetActive(true);
        
        if(hardnessChosen)
        {
            hardnessTutorial.SetActive(false);
            hardFlavorPanel.SetActive(false);
            hardToolFText.SetActive(true);
        }
        else
        {
            hardToolFText.SetActive(false);
        }
        
        if(structureChosen)
        {
            structureTutorial.SetActive(false);
            structFlavorPanel.SetActive(false);
            structToolFText.SetActive(true);
        }
        else
        {
            structToolFText.SetActive(false);
        }
    }
    
    public void Back()
    {
        if (hardnessTutorial.activeSelf || structureTutorial.activeSelf)
        {
            if(hardnessChosen)
            {
                hardnessTutorial.SetActive(false);
                hardnessChosen = false;
            }
            if(structureChosen)
            {
                structureTutorial.SetActive(false);
                structureChosen = false;
            }
            
            hardFlavorPanel.SetActive(false);
            structFlavorPanel.SetActive(false);
            startScreen.SetActive(true);
            infoPanel.SetActive(false);
        }
        
        if (hardFlavorPanel.activeSelf || toolFlavorPanel.activeSelf || structFlavorPanel.activeSelf)
        {
            if(hardnessChosen)
                hardnessTutorial.SetActive(true);
            if(structureChosen)
                structureTutorial.SetActive(true);
            
            hardFlavorPanel.SetActive(false);
            structFlavorPanel.SetActive(false);
            toolFlavorPanel.SetActive(false);
        }
    }
}
