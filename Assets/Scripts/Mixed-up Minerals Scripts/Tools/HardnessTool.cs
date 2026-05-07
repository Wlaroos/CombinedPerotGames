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

    [SerializeField] private float spinDuration;

    
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
        
        float mineralHardness = currentMineral.mineralValues.hardness;

        int mineralHardnessInt = Mathf.RoundToInt(mineralHardness);
            
        if(!hardnessDial.coroutineStarted)
        {
            hardnessDial.SetStepSmooth(mineralHardnessInt, spinDuration);
        }

        currentMineral.hardnessDiscovered = true;
        indicator.color = hardnessDial.currentStep >= currentMineral.mineralValues.hardness ? scratchColor : noScratchColor;
    }
}
