using UnityEngine;
using UnityEditor;
using UnityEngine.Localization.Components;
using TMPro;
using UnityEditor.Events;
using UnityEngine.Events;

public class UGUILocalizeFixer : Editor
{
    [MenuItem("Tools/Localization/Force Hook UGUI")]
public static void FixHooks()
    {
        // Find every LocalizeStringEvent in the current scene
        var localizers = FindObjectsByType<LocalizeStringEvent>(FindObjectsInactive.Include,FindObjectsSortMode.None);
        int count = 0;

        foreach (var loc in localizers)
        {
            // TMP_Text is the parent class for BOTH TextMeshPro and TextMeshProUGUI
            var tmp = loc.GetComponent<TMP_Text>();
            
            if (tmp != null)
            {

                // Clear any broken or existing listeners to prevent double-firing
                while (loc.OnUpdateString.GetPersistentEventCount() > 0)
                {
                    UnityEventTools.RemovePersistentListener(loc.OnUpdateString, 0);
                }

                // Listener pointing to the SetText method
                UnityEventTools.AddPersistentListener(loc.OnUpdateString, tmp.SetText);
                
                // Ensure the listener is active in both the editor and at runtime
                loc.OnUpdateString.SetPersistentListenerState(0, UnityEventCallState.EditorAndRuntime);

                // Mark the object as "dirty" so Unity saves the change
                EditorUtility.SetDirty(loc);
                count++;
            }
        }
        
        Debug.Log($"Successfully synchronized {count} Localization Events!");
    }
}