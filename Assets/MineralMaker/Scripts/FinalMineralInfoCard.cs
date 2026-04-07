using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class FinalMineralInfoCard : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image _backgroundPanel;
    [SerializeField] private Image _innerPanel;
    [Space(10)]
    [SerializeField] private Image _mineralIcon;
    [SerializeField] private TextMeshProUGUI _mineralNameText;
    [Space(10)]
    [SerializeField] private Image _mineralIconSmall;
    [SerializeField] private TextMeshProUGUI _mineralNameSmallText;
    [SerializeField] private TextMeshProUGUI _mineralDescriptionText;
    [SerializeField] private TextMeshProUGUI _mineralFunFactText;
    [Space(10)]
    [SerializeField] private GameObject _front;
    [SerializeField] private GameObject _back;

    private Button _flipButton;

    private void Awake()
    {
        _flipButton = GetComponent<Button>();
        _back.SetActive(false);

        if (_flipButton != null)
        {
            _flipButton.onClick.AddListener(() => StartCoroutine(FlipCard()));
        }
    }

    public void Setup(CraftingRecipe recipe)
    {
        if (recipe == null || recipe.output == null) return;

        // Set Text
        _mineralNameText.text = SOHelpers.GetFullStrippedName(recipe.output);
        _mineralNameSmallText.text = SOHelpers.GetFullStrippedName(recipe.output);
        _mineralDescriptionText.text = SOHelpers.GetDescriptionFromData(recipe.output);
        _mineralFunFactText.text = SOHelpers.GetFunFactFromData(recipe.output);

        // Set Icon
        _mineralIcon.sprite = SOHelpers.GetBigSpriteFromData(recipe.output);
        _mineralIcon.preserveAspect = true;
        _mineralIconSmall.sprite = SOHelpers.GetBigSpriteFromData(recipe.output);
        _mineralIconSmall.preserveAspect = true;

        // Set Colors (Following the logic in your SetupMineralUI)
        Color baseColor = SOHelpers.GetColorFromData(recipe.output);
        
        if (_backgroundPanel != null)
        {
            _backgroundPanel.color = baseColor;
        }

        if (_innerPanel != null)
        {
            _innerPanel.color = baseColor * new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }

    public void Setup(MineralData mineralData)
    {
        if (mineralData == null) return;

        // Set Text
        _mineralNameText.text = mineralData.mineralName;
        _mineralNameSmallText.text = mineralData.mineralName;
        _mineralDescriptionText.text = mineralData.description;
        _mineralFunFactText.text = mineralData.funFact;

        // Set Icon
        _mineralIcon.sprite = mineralData.mineralBigSprite;
        _mineralIcon.preserveAspect = true;
        _mineralIconSmall.sprite = mineralData.mineralSprite;
        _mineralIconSmall.preserveAspect = true;

        // Set Colors (Following the logic in your SetupMineralUI)
        Color baseColor = mineralData.defaultColor;

        if (_backgroundPanel != null)
        {
            _backgroundPanel.color = baseColor;
        }

        if (_innerPanel != null)
        {
            _innerPanel.color = baseColor * new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }

    private IEnumerator FlipCard()
    {
        float elapsedTime = 0f;
        float flipDuration = 0.25f;

        while (elapsedTime < flipDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / flipDuration);
            float scaleX = Mathf.Lerp(1f, 0f, t);

            transform.localScale = new Vector3(scaleX, 1f, 1f);
            yield return null;
        }

        if (_front.activeSelf)
        {
            _front.SetActive(false);
            _back.SetActive(true);
        }
        else
        {
            _front.SetActive(true);
            _back.SetActive(false);
        }

        elapsedTime = 0f;

        while (elapsedTime < flipDuration)        
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / flipDuration);
            float scaleX = Mathf.Lerp(0f, 1f, t);
            transform.localScale = new Vector3(scaleX, 1f, 1f);
            yield return null;
        }
    }
}