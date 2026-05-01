using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Localization.Components;

// Controls how a compound looks and stores its info
public class Compound : MonoBehaviour
{
    public CompoundData data; // Info about this compound

    [SerializeField] private SpriteRenderer _compoundSprite;   // Main picture of the compound
    [SerializeField] private SpriteRenderer _backgroundSprite; // Background behind the compound
    [SerializeField] private TextMeshPro _nameText; // Text for the element's name

    // Runs when the object is created
    private void Awake()
    {
        UpdateDataVisuals(); // Set up how the compound looks
    }

    private void OnEnable() 
    {       
        StartCoroutine(StartFadeOut());
    }

    // Updates the compound's appearance based on its data
    public void UpdateDataVisuals()
    {
        if (data != null)
        {
            // Set the main sprite and color
            _compoundSprite.sprite = data.compoundSprite;
            _compoundSprite.color = data.defaultColor;

            // Make the background a dimmer version of the main color
            Color32 c = data.defaultColor;
            Color dimmed = new Color(c.r / 255f * 0.5f, c.g / 255f * 0.5f, c.b / 255f * 0.5f, c.a / 255f);
            _backgroundSprite.color = dimmed;

        if (_nameText != null)
        {
            // Set the localized name for the compound, allows dynamic updates if localization changes
            var localizer = FindFirstObjectByType<LocalizeStringEvent>(FindObjectsInactive.Include);
            localizer.StringReference = data.compoundName;

            _nameText.text = SOHelpers.GetBaseName(data.compoundName.GetLocalizedString());
            _nameText.color = data.defaultColor;
        }
        }
    }

    private IEnumerator StartFadeOut()
    {
        yield return new WaitForSeconds(3);

        float fadeDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            if (_nameText != null)
            {
                Color c = _nameText.color;
                c.a = alpha;
                _nameText.color = c;
            }
            yield return null;
        }
    }
}