using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OrganizerGameManager : MonoBehaviour
{
    public static OrganizerGameManager Instance;

    [Header("HardnessReference")]
    [SerializeField] private GameObject hardnessTutorial;
    [SerializeField] private GameObject hardnessTool;
    [SerializeField, TextArea(3, 10)] private string hardnessToolText;

    [Header("StructureReference")]
    [SerializeField] private GameObject structureTutorial;
    [SerializeField] private GameObject structureTool;
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

    [Header("Misc")]
    [SerializeField] private GameObject lever;
    [SerializeField] private ToolWire wire;

    private bool hardnessChosen;
    private bool structureChosen;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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
        //Time.timeScale = 0f; // pause game
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
        hardnessTutorial.SetActive(true);
        hardnessTool.SetActive(true);
        sortingManager.HardnessArrangement();
        tooltipManager.HardnessActive();
        infoPanel.SetActive(true);
        toolInfo.text = hardnessToolText;
        startScreen.SetActive(false);
        lever.SetActive(true);
        hardnessChosen = true;
    }

    public void ChooseStructure()
    {
        structureTutorial.SetActive(true);
        structureTool.SetActive(true);
        sortingManager.StructureArrangement();
        tooltipManager.ScannerActive();
        infoPanel.SetActive(true);
        toolInfo.text = structureToolText;
        startScreen.SetActive(false);
        structureChosen = true;
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
    }
}
