using System.Collections;
using UnityEngine;

public class HardnessTool : MonoBehaviour
{
    [Header("Dial")]
    public HardnessDial hardnessDial;

    [Header("Visuals")]
    public SpriteRenderer indicator;
    public Color scratchColor = Color.green;
    public Color noScratchColor = Color.red;
    public Color neutralColor = Color.white;

    [HideInInspector]
    public OrganizerMineral currentMineral;
    
    private float revealTimer = 0f;
    [Header("Delay")]
    [SerializeField] private float revealDelay = 1f;
    
    [Header("Lever")]
    [SerializeField] private HardnessDifficulty difficulty;

    [SerializeField] private float spinSpeed;

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        OrganizerMineral m = other.GetComponent<OrganizerMineral>();
        if (m)
        {
            currentMineral = m;
            Evaluate();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OrganizerMineral m = other.GetComponent<OrganizerMineral>();
        if (m == currentMineral)
        {
            currentMineral = null;
            indicator.color = neutralColor;
        }
    }

    private void Update()
    {
        Evaluate();
    }
    
    private void Evaluate()
    {
        if (!currentMineral) return;
        if (!difficulty.isHard && currentMineral.hardnessDiscovered) return;
        
        float mineralHardness = currentMineral.mineralValues.hardness;
        
        if(difficulty.isHard)
        {
            int toolHardness = hardnessDial.GetHardnessValue();

            bool scratches = toolHardness >= mineralHardness;
            bool correct = Mathf.Approximately(toolHardness, mineralHardness);

            indicator.color = scratches ? scratchColor : noScratchColor;

            if (correct && !currentMineral.hardnessDiscovered)
            {
                revealTimer += Time.deltaTime;

                if (revealTimer >= revealDelay)
                {
                    currentMineral.hardnessDiscovered = true;
                }
            }
            else
            {
                revealTimer = 0f;
            }
        }

        if (!difficulty.isHard)
        {
            int mineralHardnessInt = Mathf.RoundToInt(mineralHardness);
            
            hardnessDial.SetStepSmooth(mineralHardnessInt, spinSpeed);
            currentMineral.hardnessDiscovered = true;
            indicator.color = scratchColor;
        }
    }
}
