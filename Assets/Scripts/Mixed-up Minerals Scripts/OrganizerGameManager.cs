using System.Collections;
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

    [Header("Managers")]
    [SerializeField] private DisplayOrderType displayOrder;
    [SerializeField] private SortingManager sortingManager;
    [SerializeField] private TooltipManager tooltipManager;
    [SerializeField] private MineralManager mineralManager;

    [Header("Misc")]
    [SerializeField] private Tray tray;
    [SerializeField] private GameObject panelIndicator;

    private SpriteRenderer psr;
    private readonly HashSet<OrganizerMineral> _mineralData = new HashSet<OrganizerMineral>();
    private float panelTimer = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        OrganizerMineral[] minerals = FindObjectsByType<OrganizerMineral>((FindObjectsSortMode)FindObjectsInactive.Include);
        psr = panelIndicator.GetComponent<SpriteRenderer>();
        foreach (var mineral in minerals)
        {
            _mineralData.Add(mineral);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (psr.color != Color.white)
        {
            panelTimer += Time.deltaTime;
            if (panelTimer >= 5f)
            {
                psr.color = Color.white;
                panelTimer = 0;
            }
        }
    }

    public void ShowWin()
    {
        winPanel.SetActive(true);

        if (hardnessChosen)
            hardnessTool.SetActive(false);
        if (structureChosen)
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

    public void SubmitCheckButton()
    {
        StartCoroutine(SubmitCheck());
    }

    public IEnumerator SubmitCheck()
    {
        yield return tray.StartCoroutine(tray.SubmitSolution());
        yield return new WaitForSeconds(1f);

        if (sortingManager.currentRule.attribute == SortingRule.AttributeType.Hardness)
        {
            displayOrder.HandleHardnessCheck();
            displayOrder.SetShowingResults(true);
        }
        else if (sortingManager.currentRule.attribute == SortingRule.AttributeType.crystalStructure)
        {
            displayOrder.HandleStructureCheck();

            foreach (var bucket in displayOrder.buckets)
            {
                bucket.UpdateBucketVisual();
            }

            displayOrder.SetShowingResults(true);
        }



        if (displayOrder.win)
        {
            psr.color = Color.green;
            yield return new WaitForSeconds(1f);
            ShowWin();
        }
        else
        {
            psr.color = Color.red;
            yield return tray.StartCoroutine(tray.ResetPosition());
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("LevelSelect");
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ChooseHardness()
    {
        hardnessChosen = true;
        hardnessTutorial.SetActive(true);
        sortingManager.HardnessArrangement();
        startScreen.SetActive(false);
        hardnessSorting.SetActive(true);
        hardnessTool.SetActive(true);
        mineralManager.RespawnMinerals();
    }

    public void ChooseStructure()
    {
        structureChosen = true;
        structureTutorial.SetActive(true);
        sortingManager.StructureArrangement();
        startScreen.SetActive(false);
        structureSorting.SetActive(true);
        structureTool.SetActive(true);
        mineralManager.RespawnMinerals();
    }

    public void StartGame()
    {
        if (hardnessChosen)
        {
            hardnessTutorial.SetActive(false);
            tooltipManager.HardnessActive();
        }

        if (structureChosen)
        {
            structureTutorial.SetActive(false);
            tooltipManager.ScannerActive();
        }

        infoPanel.SetActive(false);
        hardFlavorPanel.SetActive(false);
        structFlavorPanel.SetActive(false);
        toolFlavorPanel.SetActive(false);
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

        if (hardnessChosen)
        {
            hardnessTutorial.SetActive(false);
            hardFlavorPanel.SetActive(false);
            hardToolFText.SetActive(true);
        }
        else
        {
            hardToolFText.SetActive(false);
        }

        if (structureChosen)
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
        if (hardFlavorPanel.activeSelf || toolFlavorPanel.activeSelf || structFlavorPanel.activeSelf)
        {
            hardFlavorPanel.SetActive(false);
            structFlavorPanel.SetActive(false);
            toolFlavorPanel.SetActive(false);

            infoPanel.SetActive(false);
        }
    }

    public void HowToPlay()
    {
        if(hardnessChosen)
        {
            hardnessTutorial.SetActive(true);
        }
        if(structureChosen)
        {
            structureTutorial.SetActive(true);
        }
    }

    public void LearnMore()
    {
        infoPanel.SetActive(true);
        if(hardnessChosen)
        {
            whatIsHardness.SetActive(true);
        }
        if(structureChosen)
        {
            whatIsStructure.SetActive(true);
        }
    }

    public void Home()
    {

    }
}
