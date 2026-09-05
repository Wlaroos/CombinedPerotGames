using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LocaleButton : MonoBehaviour
{
    private TextMeshProUGUI _buttonTMP;
    private Button _button;
    private bool _isChangingLocale = false;
    private int _currentLocaleIndex = 0;

    private void Awake()
    {
        _buttonTMP = GetComponentInChildren<TextMeshProUGUI>();
        _button = GetComponent<Button>();
    }

    private IEnumerator Start()
    {
        // Wait until Unity Localization is fully initialized
        yield return LocalizationSettings.InitializationOperation;

        UpdateLocaleState(LocalizationSettings.SelectedLocale);
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnButtonClick);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) ChangeLocale(0);
        else if (Input.GetKeyDown(KeyCode.X)) ChangeLocale(1);
        else if (Input.GetKeyDown(KeyCode.C)) ChangeLocale(2);
    }

    private void OnButtonClick()
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        if (locales.Count == 0) return;

        int nextIndex = (_currentLocaleIndex + 1) % locales.Count;
        ChangeLocale(nextIndex);
    }

    public void ChangeLocale(int localeIndex)
    {
        if (_isChangingLocale) return;
        
        var locales = LocalizationSettings.AvailableLocales.Locales;
        if (localeIndex < 0 || localeIndex >= locales.Count) return;

        StartCoroutine(SetLocaleRoutine(localeIndex));
    }

    private IEnumerator SetLocaleRoutine(int localeIndex)
    {
        _isChangingLocale = true;

        yield return LocalizationSettings.InitializationOperation;

        Locale targetLocale = LocalizationSettings.AvailableLocales.Locales[localeIndex];
        LocalizationSettings.SelectedLocale = targetLocale;

        UpdateLocaleState(targetLocale);

        _isChangingLocale = false;
    }

    private void UpdateLocaleState(Locale currentLocale)
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        _currentLocaleIndex = locales.IndexOf(currentLocale);

        if (_currentLocaleIndex == -1) _currentLocaleIndex = 0;

        // Dynamically grabs the ISO code (e.g., "EN", "ES", "JA")
        if (_buttonTMP != null && currentLocale != null)
        {
            _buttonTMP.text = currentLocale.Identifier.Code.ToUpper();
        }
    }
}