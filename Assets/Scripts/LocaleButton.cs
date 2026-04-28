using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class LocaleButton : MonoBehaviour
{
    private int _currentLocaleIndex = 0;
    private string[] _localeKeys = { "en", "es", "jp" };
    private bool _active = false;
    private TextMeshProUGUI _buttonTMP;
    private Button _button;

    private void Awake()
    {
        _buttonTMP = GetComponentInChildren<TextMeshProUGUI>();
        _button = GetComponent<Button>();
        
    }

    private void Start()
    {
        _currentLocaleIndex = LocalizationSettings.SelectedLocale == null ? 0 : LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        _buttonTMP.text = _localeKeys[_currentLocaleIndex].ToUpper();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnButtonClick);
    }

    public void ChangeLocale(int localeID)
    {
        if (_active) return;
        StartCoroutine(SetLocale(localeID));
    }

    IEnumerator SetLocale(int _localeID)
    {
        _active = true;
        // Wait for the localization system to initialize
        yield return LocalizationSettings.InitializationOperation;
        
        // Set the selected locale based on the index in your Project Settings
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        
        _active = false;
    }
    void OnButtonClick()
    {
        if(_currentLocaleIndex >= _localeKeys.Length - 1)
        {
            _currentLocaleIndex = 0;
        }
        else
        {
            _currentLocaleIndex++;
        }

        _buttonTMP.text = _localeKeys[_currentLocaleIndex].ToUpper();

        ChangeLocale(_currentLocaleIndex);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ChangeLocale(0); // Switch to the first locale
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            ChangeLocale(1); // Switch to the second locale
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeLocale(2); // Switch to the third locale 
        }
    }
}