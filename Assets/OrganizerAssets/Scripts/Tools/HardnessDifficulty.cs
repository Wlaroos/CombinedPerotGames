using TMPro;
using UnityEngine;

public class HardnessDifficulty : MonoBehaviour
{
    public TextMeshProUGUI text;
    [HideInInspector] public bool isHard;
    [SerializeField] private Sprite[] levers;
    private SpriteRenderer _sp;

    private void Start()
    {
        _sp = GetComponent<SpriteRenderer>();

        isHard = true;
        _sp.sprite = levers[0];
        
        if (isHard)
            text.text = "Hard";
        else
            text.text = "Easy";
    }
    
    private void OnMouseDown()
    {
        isHard = !isHard;
        _sp.sprite = isHard ? levers[0] : levers[1];
        if (isHard)
            text.text = "Hard";
        else
            text.text = "Easy";
    }
}
