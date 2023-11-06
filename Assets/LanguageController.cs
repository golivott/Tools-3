using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

public static class LanguageController
{
    public static List<Language> languages;
    private static int _selectedLanguage;

    public static int selectedLanguage
    {
        get { return _selectedLanguage; }
        set
        {
            _selectedLanguage = value;
            PlayerPrefs.SetInt("SelectedLanguage", _selectedLanguage);
        }
    }

    static LanguageController()
    {
        // Load the selected language from PlayerPrefs when the class is first accessed
        _selectedLanguage = PlayerPrefs.GetInt("SelectedLanguage", 0);
        
        // Load languages from Resources folder
        languages = new List<Language>(Resources.LoadAll<Language>("Languages"));
    }
    
    public static void UpdateTextLanguage()
    {
        TextMeshProUGUI[] textBoxes = Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        
        foreach (TextMeshProUGUI textBox in textBoxes)
        {
            string key = textBox.gameObject.name;
            if (languages[selectedLanguage].dict.TryGetValue(key, out string value))
                textBox.text = value;
        }
    }
}
