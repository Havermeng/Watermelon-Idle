using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LocalizationEditor : EditorWindow
{
    private LocalizationManager localizationManager;
    private Vector2 scrollPosition;
    private Vector2 keysScrollPosition;
    
    private string newKeyName = "";
    private string newKeyRussian = "";
    private string newKeyEnglish = "";
    
    private string searchFilter = "";
    private string selectedKey = "";
    private string selectedKeyRussian = "";
    private string selectedKeyEnglish = "";
    
    [MenuItem("Tools/Localization Editor")]
    public static void ShowWindow()
    {
        GetWindow<LocalizationEditor>("Localization Editor");
    }

    void OnEnable()
    {
        FindLocalizationManager();
    }

    void FindLocalizationManager()
    {
        LocalizationManager[] managers = FindObjectsOfType<LocalizationManager>(true);
        if (managers.Length > 0)
        {
            localizationManager = managers[0];
        }
    }

    void OnGUI()
    {
        if (localizationManager == null)
        {
            EditorGUILayout.HelpBox("LocalizationManager not found in scene!", MessageType.Warning);
            
            if (GUILayout.Button("Find in Scene"))
            {
                FindLocalizationManager();
            }
            
            if (GUILayout.Button("Create New"))
            {
                CreateLocalizationManager();
            }
            
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("=== Localization Editor ===", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.BeginVertical("box", GUILayout.Width(280));
        DrawKeysList();
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginVertical("box");
        DrawEditArea();
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndHorizontal();
    }

    void DrawKeysList()
    {
        EditorGUILayout.LabelField("Keys List", EditorStyles.boldLabel);
        
        searchFilter = EditorGUILayout.TextField("Search:", searchFilter);
        EditorGUILayout.Space();

        var allKeys = localizationManager.GetAllKeys();
        
        keysScrollPosition = EditorGUILayout.BeginScrollView(keysScrollPosition, GUILayout.Height(300));
        
        foreach (var key in allKeys)
        {
            if (!string.IsNullOrEmpty(searchFilter) && 
                !key.ToLower().Contains(searchFilter.ToLower()))
            {
                continue;
            }
            
            bool isSelected = (key == selectedKey);
            string displayName = key;
            
            GUI.backgroundColor = isSelected ? Color.yellow : Color.white;
            
            EditorGUILayout.BeginHorizontal("box");
            
            if (GUILayout.Button(displayName, isSelected ? "Button" : "Label", GUILayout.Height(25)))
            {
                selectedKey = key;
                LoadSelectedKeyTranslations();
            }
            
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Total: {allKeys.Count} keys", EditorStyles.miniLabel);
        
        EditorGUILayout.Space();
        DrawAddNewKey();
    }

    void DrawAddNewKey()
    {
        GUI.backgroundColor = new Color(0.9f, 1f, 0.9f);
        EditorGUILayout.BeginVertical("box");
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.LabelField("Add New Key", EditorStyles.boldLabel);
        
        GUI.backgroundColor = Color.cyan;
        newKeyName = EditorGUILayout.TextField("Key Name:", newKeyName);
        GUI.backgroundColor = Color.white;
        
        newKeyRussian = EditorGUILayout.TextArea(newKeyRussian, GUILayout.Height(40));
        EditorGUILayout.LabelField("Russian text", EditorStyles.miniLabel);
        
        newKeyEnglish = EditorGUILayout.TextArea(newKeyEnglish, GUILayout.Height(40));
        EditorGUILayout.LabelField("English text", EditorStyles.miniLabel);
        
        GUI.backgroundColor = new Color(0.4f, 0.9f, 0.4f);
        if (GUILayout.Button("+ Add Key", GUILayout.Height(30)))
        {
            if (!string.IsNullOrEmpty(newKeyName) && !string.IsNullOrEmpty(newKeyRussian) && !string.IsNullOrEmpty(newKeyEnglish))
            {
                AddNewKey(newKeyName, newKeyRussian, newKeyEnglish);
                newKeyName = "";
                newKeyRussian = "";
                newKeyEnglish = "";
                EditorUtility.DisplayDialog("Success", "Key added!", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Fill all fields!", "OK");
            }
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndVertical();
    }

    void DrawEditArea()
    {
        EditorGUILayout.LabelField("Edit Translation", EditorStyles.boldLabel);
        
        if (string.IsNullOrEmpty(selectedKey))
        {
            EditorGUILayout.HelpBox("Select a key from the list to edit", MessageType.Info);
            return;
        }
        
        EditorGUILayout.Space();
        
        GUI.backgroundColor = Color.yellow;
        EditorGUILayout.LabelField("Key: " + selectedKey, EditorStyles.boldLabel);
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space();
        
        GUI.backgroundColor = new Color(0.8f, 0.9f, 1f);
        EditorGUILayout.LabelField("Russian (RU):", EditorStyles.boldLabel);
        selectedKeyRussian = EditorGUILayout.TextArea(selectedKeyRussian, GUILayout.Height(60));
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space();
        
        GUI.backgroundColor = new Color(0.8f, 1f, 0.8f);
        EditorGUILayout.LabelField("English (EN):", EditorStyles.boldLabel);
        selectedKeyEnglish = EditorGUILayout.TextArea(selectedKeyEnglish, GUILayout.Height(60));
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space();
        
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Save Changes", GUILayout.Height(35)))
        {
            SaveTranslation(selectedKey, selectedKeyRussian, selectedKeyEnglish);
            EditorUtility.DisplayDialog("Saved", "Translation saved!", "OK");
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("Duplicate"))
        {
            DuplicateKey(selectedKey);
        }
        GUI.backgroundColor = Color.white;
        
        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
        if (GUILayout.Button("Delete"))
        {
            if (EditorUtility.DisplayDialog("Delete Key", 
                $"Delete key '{selectedKey}'?", "Delete", "Cancel"))
            {
                DeleteKey(selectedKey);
            }
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndHorizontal();
    }

    void LoadSelectedKeyTranslations()
    {
        selectedKeyRussian = localizationManager.GetRussian(selectedKey);
        selectedKeyEnglish = localizationManager.GetEnglish(selectedKey);
    }

    void SaveTranslation(string key, string russian, string english)
    {
        localizationManager.SetTranslation(key, russian, english);
        EditorUtility.SetDirty(localizationManager);
    }

    void AddNewKey(string key, string russian, string english)
    {
        localizationManager.SetTranslation(key, russian, english);
        EditorUtility.SetDirty(localizationManager);
        selectedKey = key;
        LoadSelectedKeyTranslations();
    }

    void DeleteKey(string key)
    {
        localizationManager.RemoveKey(key);
        EditorUtility.SetDirty(localizationManager);
        selectedKey = "";
        selectedKeyRussian = "";
        selectedKeyEnglish = "";
    }

    void DuplicateKey(string key)
    {
        string newKey = key + "_copy";
        string russian = localizationManager.GetRussian(key);
        string english = localizationManager.GetEnglish(key);
        AddNewKey(newKey, russian, english);
    }

    void CreateLocalizationManager()
    {
        GameObject go = new GameObject("LocalizationManager");
        localizationManager = go.AddComponent<LocalizationManager>();
        DontDestroyOnLoad(go);
    }
}
