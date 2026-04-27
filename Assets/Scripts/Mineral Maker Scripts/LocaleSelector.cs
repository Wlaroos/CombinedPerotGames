using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour
{
    private bool active = false;

    public void ChangeLocale(int localeID)
    {
        if (active) return;
        StartCoroutine(SetLocale(localeID));
    }

    IEnumerator SetLocale(int _localeID)
    {
        active = true;
        // Wait for the localization system to initialize
        yield return LocalizationSettings.InitializationOperation;
        
        // Set the selected locale based on the index in your Project Settings
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        
        active = false;
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