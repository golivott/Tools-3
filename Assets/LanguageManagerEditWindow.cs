using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LanguageManagerEditWindow : EditorWindow
{
    private Language language;
    private string originalKey;
    private string key;
    private string value;
    
    public void OpenWindow(Language language, string key)
    {
        GetWindow<LanguageManagerEditWindow>( "Key Editor");
        this.language = language;
        this.key = key;
        originalKey = key;
        if(this.language.dict.TryGetValue(key, out string value))
        {
            this.value = value;
        }
    }

    private void OnGUI()
    {
        key = EditorGUILayout.TextArea(key,GUILayout.ExpandHeight(true));
        if (GUILayout.Button("Apply Key"))
        {
            UpdateDictKey();
        } 
        value = EditorGUILayout.TextArea(value,GUILayout.ExpandHeight(true));
        if (GUILayout.Button("Apply Value"))
        {
            UpdateDictValue();
        }

        if (GUILayout.Button("Delete Key"))
        {
            if (EditorUtility.DisplayDialog("Remove Key " + originalKey + "?", "Remove Key " + originalKey + "?", "Confirm",
                    "Cancel"))
            {
                RemoveKey();
            }
        }
    }

    private void UpdateDictValue()
    {
        language.dict[key] = value;
        EditorUtility.SetDirty(language);
        
        Close();
    }

    private void UpdateDictKey()
    {
        List<Language> languages = LanguageManager.PopulateLanguages();

        foreach (Language language in languages)
        {
            if (language.dict.TryAdd(key, value))
            {
                language.dict.Remove(originalKey);
                EditorUtility.SetDirty(language);
            }
            else
            {
                Debug.Log("Key already exists");
            }
        }
        
        Close();
    }
    
    private void RemoveKey()
    {
        List<Language> languages = LanguageManager.PopulateLanguages();
        
        foreach (Language language in languages)
        {
            language.dict.Remove(key);
            EditorUtility.SetDirty(language);
        }
        
        Close();
    }
}
