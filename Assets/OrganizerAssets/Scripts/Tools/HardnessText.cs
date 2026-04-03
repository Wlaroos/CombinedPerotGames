using TMPro;
using UnityEngine;

public class HardnessText : MonoBehaviour
{
    public TextMeshProUGUI hardnessText;
    public HardnessDial dial;

    void Start()
    {
        hardnessText = GetComponent<TextMeshProUGUI>();
        dial = FindAnyObjectByType<HardnessDial>();
    }
    void Update()
    {
        hardnessText.text = $"{dial.GetHardnessValue()}";
    }
}
