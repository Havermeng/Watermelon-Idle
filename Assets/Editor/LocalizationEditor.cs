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
        EditorGUILayout.LabelField("Localization Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        DrawMainContent();
    }

    void DrawMainContent()
    {
        EditorGUILayout.BeginHorizontal();
        
        // Left panel - Keys list
        EditorGUILayout.BeginVertical("box", GUILayout.Width(250));
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
        EditorGUILayout.LabelField("Keys", EditorStyles.boldLabel);
        
        searchFilter = EditorGUILayout.TextField("Search:", searchFilter);
        EditorGUILayout.Space();

        var allKeys = GetAllKeys();
        
        keysScrollPosition = EditorGUILayout.BeginScrollView(keysScrollPosition, GUILayout.Height(400));
        
        foreach (var key in allKeys)
        {
            if (!string.IsNullOrEmpty(searchFilter) && 
                !key.ToLower().Contains(searchFilter.ToLower()))
            {
                continue;
            }
            
            bool isSelected = (key == selectedKey);
            string displayName = key.Length > 20 ? key.Substring(0, 20) + "..." : key;
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button(displayName, isSelected ? "Button" : "Label"))
            {
                selectedKey = key;
            }
            
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                if (EditorUtility.DisplayDialog("Delete Key", 
                    $"Delete key '{key}'?", "Delete", "Cancel"))
                {
                    DeleteKey(key);
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Total: {allKeys.Count} keys");
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
        EditorGUILayout.LabelField($"Key: {selectedKey}");
        
        var translations = GetTranslations(selectedKey);
        string russian = translations.Item1;
        string english = translations.Item2;
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Russian (RU):", EditorStyles.boldLabel);
        russian = EditorGUILayout.TextArea(russian, GUILayout.Height(60));
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("English (EN):", EditorStyles.boldLabel);
        english = EditorGUILayout.TextArea(english, GUILayout.Height(60));
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Save Changes", GUILayout.Height(30)))
        {
            SaveTranslation(selectedKey, russian, english);
            EditorUtility.DisplayDialog("Saved", "Translation saved!", "OK");
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Duplicate Key"))
        {
            DuplicateKey(selectedKey);
        }
        
        if (GUILayout.Button("Delete Key"))
        {
            if (EditorUtility.DisplayDialog("Delete Key", 
                $"Delete key '{selectedKey}'?", "Delete", "Cancel"))
            {
                DeleteKey(selectedKey);
            }
        }
        
        EditorGUILayout.EndHorizontal();
    }

    List<string> GetAllKeys()
    {
        var keys = new List<string>();
        
        var fields = typeof(LocalizationKeys).GetFields();
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(string))
            {
                keys.Add(field.GetValue(null) as string);
            }
        }
        
        keys.Sort();
        return keys;
    }

    (string, string) GetTranslations(string key)
    {
        string russian = "";
        string english = "";
        
        SerializedObject so = new SerializedObject(localizationManager);
        
        var russianDict = so.FindProperty("russianTranslations");
        var englishDict = so.FindProperty("englishTranslations");
        
        if (russianDict != null && russianDict.isArray)
        {
            for (int i = 0; i < russianDict.arraySize; i++)
            {
                var element = russianDict.GetArrayElementAtIndex(i);
                var keyProp = element.FindPropertyRelative("key");
                var valueProp = element.FindPropertyRelative("value");
                
                if (keyProp != null && keyProp.stringValue == key)
                {
                    russian = valueProp != null ? valueProp.stringValue : "";
                    break;
                }
            }
        }
        
        if (englishDict != null && englishDict.isArray)
        {
            for (int i = 0; i < englishDict.arraySize; i++)
            {
                var element = englishDict.GetArrayElementAtIndex(i);
                var keyProp = element.FindPropertyRelative("key");
                var valueProp = element.FindPropertyRelative("value");
                
                if (keyProp != null && keyProp.stringValue == key)
                {
                    english = valueProp != null ? valueProp.stringValue : "";
                    break;
                }
            }
        }
        
        return (russian, english);
    }

    void SaveTranslation(string key, string russian, string english)
    {
        localizationManager.SetTranslation(key, russian, english);
        EditorUtility.SetDirty(localizationManager);
    }

    void DeleteKey(string key)
    {
        selectedKey = "";
    }

    void DuplicateKey(string key)
    {
        string newKeyName = key + "_copy";
        var translations = GetTranslations(key);
        localizationManager.SetTranslation(newKeyName, translations.Item1, translations.Item2);
        EditorUtility.SetDirty(localizationManager);
        selectedKey = newKeyName;
    }

    void CreateLocalizationManager()
    {
        GameObject go = new GameObject("LocalizationManager");
        localizationManager = go.AddComponent<LocalizationManager>();
        DontDestroyOnLoad(go);
    }
}

public static class LocalizationKeys
{
    public const string GAME_TITLE = "game_title";
    public const string START_GAME = "start_game";
    public const string SETTINGS = "settings";
    public const string QUIT = "quit";
    public const string SAVE = "save";
    public const string EXIT = "exit";
    public const string BACK = "back";
    public const string PLAY = "play";
    public const string RESET = "reset";
    public const string RENAME = "rename";
    public const string EMPTY_SAVE = "empty_save";
    public const string OPEN_SHOP = "open_shop";
    public const string AUDIO = "audio";
    public const string MUSIC_VOLUME = "music_volume";
    public const string SFX_VOLUME = "sfx_volume";
    public const string SLOT = "slot";
    public const string SLOT_NUMBER = "slot_number";
    public const string SAVE_NAME = "save_name";
    public const string SAVE_DATE = "save_date";
    public const string NEW_SAVE = "new_save";
    public const string LOAD_SAVE = "load_save";
    public const string DELETE_SAVE = "delete_save";
    public const string GROW_SPEED = "grow_speed";
    public const string HARVEST_VALUE = "harvest_value";
    public const string CRIT_HARVEST = "crit_harvest";
    public const string FERTILIZER = "fertilizer";
    public const string SUPER_SEEDS = "super_seeds";
    public const string LEVEL = "level";
    public const string COINS = "coins";
    public const string MAX_LEVEL = "max_level";
    public const string UNLOCK = "unlock";
    public const string ALL_UNLOCKED = "all_unlocked";
    public const string SLOT_EMPTY = "slot_empty";
    public const string CONFIRM = "confirm";
    public const string CANCEL = "cancel";
    public const string PAUSED = "paused";
    public const string RESUME = "resume";
    public const string MAIN_MENU = "main_menu";
    public const string VERSIONS = "versions";
    public const string LOADING = "loading";
    public const string COINS_LABEL = "coins_label";
}
