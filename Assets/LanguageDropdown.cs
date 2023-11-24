using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    void Start()
    {
        // Clear the old options of the dropdown
        dropdown.ClearOptions();

        // Create a new list of options
        List<string> options = new List<string>();
        foreach (var language in LanguageController.languages)
        {
            options.Add(language.name);
        }

        // Add the options to the dropdown
        dropdown.AddOptions(options);

        // Set the current value of the dropdown
        dropdown.value = LanguageController.selectedLanguage;

        // Make sure the dropdown displays the current value
        dropdown.RefreshShownValue();

        // Add listener for when the selected option changes
        dropdown.onValueChanged.AddListener(delegate {
            LanguageController.selectedLanguage = dropdown.value;
            LanguageController.UpdateTextLanguage();
        });
    }
}