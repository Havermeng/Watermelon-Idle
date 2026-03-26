using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LocalizationEditor : EditorWindow
{
    private LocalizationManager localizationManager;
    private List<string> cachedKeys = new List<string>();
    private bool needsReload = true;
    
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

    void OnFocus()
    {
        needsReload = true;
    }

    void FindLocalizationManager()
    {
        LocalizationManager[] managers = FindObjectsOfType<LocalizationManager>(true);
        if (managers.Length > 0)
        {
            localizationManager = managers[0];
            needsReload = true;
        }
    }

    void ReloadKeys()
    {
        if (localizationManager == null)
        {
            cachedKeys = new List<string>();
            return;
        }

        cachedKeys = localizationManager.GetAllKeys();
        needsReload = false;
        Repaint();
    }

    void OnGUI()
    {
        if (localizationManager == null)
        {
            FindLocalizationManager();
        }

        if (localizationManager == null)
        {
            EditorGUILayout.HelpBox("LocalizationManager not found!", MessageType.Warning);
            
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

        if (needsReload)
        {
            ReloadKeys();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("=== Localization Editor ===", EditorStyles.boldLabel);
        
        if (GUILayout.Button("🔄 Reload Keys"))
        {
            ReloadKeys();
        }
        
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
        EditorGUILayout.LabelField("Keys List (" + cachedKeys.Count + ")", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        searchFilter = EditorGUILayout.TextField(searchFilter, GUILayout.Width(150));
        if (GUILayout.Button("X", GUILayout.Width(25)))
        {
            searchFilter = "";
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();

        int displayCount = 0;
        foreach (var key in cachedKeys)
        {
            if (!string.IsNullOrEmpty(searchFilter) && 
                !key.ToLower().Contains(searchFilter.ToLower()))
            {
                continue;
            }
            displayCount++;
            
            bool isSelected = (key == selectedKey);
            
            if (isSelected)
            {
                GUI.backgroundColor = Color.yellow;
            }
            
            if (GUILayout.Button(key, GUILayout.Height(25)))
            {
                selectedKey = key;
                LoadSelectedKeyTranslations();
            }
            
            GUI.backgroundColor = Color.white;
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Showing: {displayCount} / {cachedKeys.Count} keys", EditorStyles.miniLabel);
        
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
        newKeyName = EditorGUILayout.TextField("Key:", newKeyName, GUILayout.Width(250));
        GUI.backgroundColor = Color.white;
        
        newKeyRussian = EditorGUILayout.TextArea(newKeyRussian, GUILayout.Height(35));
        EditorGUILayout.LabelField("Russian", EditorStyles.miniLabel);
        
        newKeyEnglish = EditorGUILayout.TextArea(newKeyEnglish, GUILayout.Height(35));
        EditorGUILayout.LabelField("English", EditorStyles.miniLabel);
        
        GUI.backgroundColor = new Color(0.4f, 0.9f, 0.4f);
        if (GUILayout.Button("+ Add Key", GUILayout.Height(25)))
        {
            if (!string.IsNullOrEmpty(newKeyName) && !string.IsNullOrEmpty(newKeyRussian) && !string.IsNullOrEmpty(newKeyEnglish))
            {
                AddNewKey(newKeyName, newKeyRussian, newKeyEnglish);
                newKeyName = "";
                newKeyRussian = "";
                newKeyEnglish = "";
                needsReload = true;
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
            EditorGUILayout.HelpBox("Select a key from the list", MessageType.Info);
            return;
        }
        
        EditorGUILayout.Space();
        
        GUI.backgroundColor = Color.yellow;
        EditorGUILayout.LabelField("Key: " + selectedKey, EditorStyles.boldLabel);
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space();
        
        GUI.backgroundColor = new Color(0.8f, 0.9f, 1f);
        EditorGUILayout.LabelField("Russian (RU):", EditorStyles.boldLabel);
        selectedKeyRussian = EditorGUILayout.TextArea(selectedKeyRussian, GUILayout.Height(50));
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space();
        
        GUI.backgroundColor = new Color(0.8f, 1f, 0.8f);
        EditorGUILayout.LabelField("English (EN):", EditorStyles.boldLabel);
        selectedKeyEnglish = EditorGUILayout.TextArea(selectedKeyEnglish, GUILayout.Height(50));
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space();
        
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Save Changes", GUILayout.Height(30)))
        {
            SaveTranslation(selectedKey, selectedKeyRussian, selectedKeyEnglish);
            needsReload = true;
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("Duplicate"))
        {
            DuplicateKey(selectedKey);
            needsReload = true;
        }
        GUI.backgroundColor = Color.white;
        
        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
        if (GUILayout.Button("Delete"))
        {
            if (EditorUtility.DisplayDialog("Delete", $"Delete '{selectedKey}'?", "Delete", "Cancel"))
            {
                DeleteKey(selectedKey);
                needsReload = true;
            }
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndHorizontal();
    }

    void LoadSelectedKeyTranslations()
    {
        if (localizationManager == null || string.IsNullOrEmpty(selectedKey)) return;
        
        selectedKeyRussian = localizationManager.GetRussian(selectedKey);
        selectedKeyEnglish = localizationManager.GetEnglish(selectedKey);
    }

    void SaveTranslation(string key, string russian, string english)
    {
        if (localizationManager == null) return;
        
        localizationManager.SetTranslation(key, russian, english);
        EditorUtility.SetDirty(localizationManager);
    }

    void AddNewKey(string key, string russian, string english)
    {
        if (localizationManager == null) return;
        
        localizationManager.SetTranslation(key, russian, english);
        EditorUtility.SetDirty(localizationManager);
        selectedKey = key;
        LoadSelectedKeyTranslations();
    }

    void DeleteKey(string key)
    {
        if (localizationManager == null) return;
        
        localizationManager.RemoveKey(key);
        EditorUtility.SetDirty(localizationManager);
        selectedKey = "";
        selectedKeyRussian = "";
        selectedKeyEnglish = "";
    }

    void DuplicateKey(string key)
    {
        if (localizationManager == null) return;
        
        string newKey = key + "_copy";
        string russian = localizationManager.GetRussian(key);
        string english = localizationManager.GetEnglish(key);
        
        localizationManager.SetTranslation(newKey, russian, english);
        EditorUtility.SetDirty(localizationManager);
        
        selectedKey = newKey;
        LoadSelectedKeyTranslations();
    }

    void CreateLocalizationManager()
    {
        GameObject go = new GameObject("LocalizationManager");
        localizationManager = go.AddComponent<LocalizationManager>();
        DontDestroyOnLoad(go);
        needsReload = true;
    }
}
