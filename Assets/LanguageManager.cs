using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LanguageManager : EditorWindow
{
    public List<Language> languages = new List<Language>();
    public int selectedLanguage;
    public string keyField;
    public string searchField = "";
    public string languageField;
    
    private SerializedObject so;
    private SerializedProperty propSelectedLanguage;
    
    private Vector2 scrollPos;
    
    [MenuItem("Tools/Language Manager")]
    public static void OpenWindow() => GetWindow<LanguageManager>( "Language Manager");

    private void OnEnable()
    {
        languages = PopulateLanguages();
        so = new SerializedObject(this);
        propSelectedLanguage = so.FindProperty("selectedLanguage");
        
        // load saved config
        selectedLanguage = EditorPrefs.GetInt("LANGUAGE_MANAGER_TOOL_selectedLanguage", 0);
        if(selectedLanguage > languages.Count - 1) 
            selectedLanguage = 0;

        LanguageController.languages = languages;
        LanguageController.selectedLanguage = selectedLanguage;
    }

    private void OnDisable()
    {
        // save config
        EditorPrefs.SetInt("LANGUAGE_MANAGER_TOOL_selectedLanguage", selectedLanguage);
    }
    
    private void OnGUI()
    {
        so.Update();
        
        // Selecting Language
        EditorGUILayout.LabelField("Language:");
        EditorGUILayout.BeginHorizontal();
        selectedLanguage = EditorGUILayout.IntPopup(selectedLanguage, GetLanguageNames(),
            Enumerable.Range(0, GetLanguageNames().Length + 1).ToArray());
        propSelectedLanguage.intValue = selectedLanguage;
        LanguageController.selectedLanguage = selectedLanguage;
        if (GUILayout.Button("Delete", GUILayout.Width(50)))
        {
            DeleteLanguage();
        }
        EditorGUILayout.EndHorizontal();
        
        // Add Language
        EditorGUILayout.BeginHorizontal();
        languageField = EditorGUILayout.TextField("", languageField);
        if (GUILayout.Button("Add", GUILayout.Width(50)))
        {
            AddLanguage();
            languageField = "";
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Separator();
        
        // Adding keys
        EditorGUILayout.LabelField("Add Key:");
        EditorGUILayout.BeginHorizontal();
        keyField = EditorGUILayout.TextField(keyField, GUILayout.ExpandWidth(true));
        if (GUILayout.Button("Add", GUILayout.Width(50)))
        {
            AddKey(keyField);
            keyField = "";
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Separator();
        
        // Editing key values
        EditorGUILayout.LabelField("Values:");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Search: ", GUILayout.Width(50));
        searchField = EditorGUILayout.TextField("", searchField);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Key",GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("Value",GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("",GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        if (languages.Count > 0)
        {
            foreach (KeyValuePair<string, string> e in languages[selectedLanguage].dict)
            {
                if (e.Key.Contains(searchField))
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayoutOption[] options = { GUILayout.MaxWidth(300), GUILayout.ExpandWidth(true) };
                    EditorGUILayout.LabelField(e.Key,EditorStyles.wordWrappedLabel,options);
                    EditorGUILayout.LabelField(e.Value,EditorStyles.wordWrappedLabel,options);
                    if (GUILayout.Button("Edit",GUILayout.Width(50)))
                    {
                        LanguageManagerEditWindow window = CreateInstance<LanguageManagerEditWindow>();
                        window.OpenWindow(languages[selectedLanguage],e.Key);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("APPLY"))
        {
            UpdateTextLanguage();
            LanguageController.languages = languages;
            LanguageController.selectedLanguage = selectedLanguage;
        }

        so.ApplyModifiedProperties();
    }

    public static List<Language> PopulateLanguages()
    {
        List<Language> languages = new List<Language>();
        string[] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/Resources/Languages" });
        languages.Clear();
        foreach (string SOName in assetNames)
        {
            string SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            Language language = AssetDatabase.LoadAssetAtPath<Language>(SOpath);
            languages.Add(language);
        }

        return languages;
    }

    string[] GetLanguageNames()
    {
        List<string> names = new List<string>();
        foreach (Language lang in languages)
        {
            names.Add(lang.name);
        }

        return names.ToArray();
    }

    public void UpdateTextLanguage()
    {
        TextMeshProUGUI[] textBoxes = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        
        foreach (TextMeshProUGUI textBox in textBoxes)
        {
            string key = textBox.gameObject.name;
            if (languages[selectedLanguage].dict.TryGetValue(key, out string value))
                textBox.text = value;
        }
    }

    private void AddKey(string key)
    {
        foreach (Language language in languages)
        {
            language.dict.Add(key, "");
            EditorUtility.SetDirty(language);
        }
    }

    private void AddLanguage()
    {
        Language language = CreateInstance<Language>();
        AssetDatabase.CreateAsset(language,"Assets/Resources/Languages/"+languageField+".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (languages.Count > 0)
        {
            foreach (string key in languages[selectedLanguage].dict.Keys.ToArray())
            {
                language.dict.Add(key,"");
                EditorUtility.SetDirty(language);
            } 
        }
        
        languages.Add(language);
    }

    private void DeleteLanguage()
    {
        Language language = languages[selectedLanguage];
        selectedLanguage = 0;

        languages.Remove(language);
        AssetDatabase.DeleteAsset("Assets/Resources/Languages/" + language.name + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
