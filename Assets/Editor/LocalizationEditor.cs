using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

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
        
        // Left panel - Keys list
        EditorGUILayout.BeginVertical("box", GUILayout.Width(280));
        DrawKeysList();
        EditorGUILayout.EndVertical();
        
        // Right panel - Edit area
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

        var allKeys = GetAllKeys();
        
        keysScrollPosition = EditorGUILayout.BeginScrollView(keysScrollPosition, GUILayout.Height(350));
        
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
        EditorGUILayout.LabelField("Add New Key", EditorStyles.boldLabel);
        
        GUI.backgroundColor = Color.cyan;
        newKeyName = EditorGUILayout.TextField("Key Name:", newKeyName);
        GUI.backgroundColor = Color.white;
        
        newKeyRussian = EditorGUILayout.TextArea(newKeyRussian, GUILayout.Height(40));
        EditorGUILayout.LabelField("Russian text", EditorStyles.miniLabel);
        
        newKeyEnglish = EditorGUILayout.TextArea(newKeyEnglish, GUILayout.Height(40));
        EditorGUILayout.LabelField("English text", EditorStyles.miniLabel);
        
        GUI.backgroundColor = Color.green;
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
        
        // Key name (readonly)
        EditorGUILayout.LabelField("Key:", EditorStyles.boldLabel);
        EditorGUILayout.TextField(selectedKey);
        
        EditorGUILayout.Space();
        
        // Russian translation
        GUI.backgroundColor = new Color(0.8f, 0.9f, 1f);
        EditorGUILayout.LabelField("Russian (RU):", EditorStyles.boldLabel);
        selectedKeyRussian = EditorGUILayout.TextArea(selectedKeyRussian, GUILayout.Height(60));
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space();
        
        // English translation
        GUI.backgroundColor = new Color(0.8f, 1f, 0.8f);
        EditorGUILayout.LabelField("English (EN):", EditorStyles.boldLabel);
        selectedKeyEnglish = EditorGUILayout.TextArea(selectedKeyEnglish, GUILayout.Height(60));
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space();
        
        // Save button
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("💾 Save Changes", GUILayout.Height(35)))
        {
            SaveTranslation(selectedKey, selectedKeyRussian, selectedKeyEnglish);
            EditorUtility.DisplayDialog("Saved", "Translation saved!", "OK");
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space();
        
        // Action buttons
        EditorGUILayout.BeginHorizontal();
        
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("📋 Duplicate"))
        {
            DuplicateKey(selectedKey);
        }
        GUI.backgroundColor = Color.white;
        
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("🗑️ Delete"))
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

    List<string> GetAllKeys()
    {
        var keys = new List<string>();
        
        // Read from the dictionaries using reflection
        var dictField = typeof(LocalizationManager).GetField("russianTranslations", BindingFlags.NonPublic | BindingFlags.Instance);
        if (dictField != null)
        {
            var dict = dictField.GetValue(localizationManager) as Dictionary<string, string>;
            if (dict != null)
            {
                keys.AddRange(dict.Keys);
            }
        }
        
        keys.Sort();
        return keys;
    }

    void LoadSelectedKeyTranslations()
    {
        var translations = GetTranslations(selectedKey);
        selectedKeyRussian = translations.Item1;
        selectedKeyEnglish = translations.Item2;
    }

    (string, string) GetTranslations(string key)
    {
        string russian = "";
        string english = "";
        
        var russianDictField = typeof(LocalizationManager).GetField("russianTranslations", BindingFlags.NonPublic | BindingFlags.Instance);
        var englishDictField = typeof(LocalizationManager).GetField("englishTranslations", BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (russianDictField != null)
        {
            var dict = russianDictField.GetValue(localizationManager) as Dictionary<string, string>;
            if (dict != null && dict.ContainsKey(key))
            {
                russian = dict[key];
            }
        }
        
        if (englishDictField != null)
        {
            var dict = englishDictField.GetValue(localizationManager) as Dictionary<string, string>;
            if (dict != null && dict.ContainsKey(key))
            {
                english = dict[key];
            }
        }
        
        return (russian, english);
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
        var russianDictField = typeof(LocalizationManager).GetField("russianTranslations", BindingFlags.NonPublic | BindingFlags.Instance);
        var englishDictField = typeof(LocalizationManager).GetField("englishTranslations", BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (russianDictField != null)
        {
            var dict = russianDictField.GetValue(localizationManager) as Dictionary<string, string>;
            if (dict != null) dict.Remove(key);
        }
        
        if (englishDictField != null)
        {
            var dict = englishDictField.GetValue(localizationManager) as Dictionary<string, string>;
            if (dict != null) dict.Remove(key);
        }
        
        EditorUtility.SetDirty(localizationManager);
        selectedKey = "";
        selectedKeyRussian = "";
        selectedKeyEnglish = "";
    }

    void DuplicateKey(string key)
    {
        string newKey = key + "_copy";
        var translations = GetTranslations(key);
        AddNewKey(newKey, translations.Item1, translations.Item2);
    }

    void CreateLocalizationManager()
    {
        GameObject go = new GameObject("LocalizationManager");
        localizationManager = go.AddComponent<LocalizationManager>();
        DontDestroyOnLoad(go);
    }
}
