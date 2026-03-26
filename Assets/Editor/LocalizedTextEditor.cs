using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(LocalizedText))]
public class LocalizedTextEditor : Editor
{
    private LocalizedText localizedText;
    private LocalizationManager locManager;
    private List<string> allKeys = new List<string>();
    private string searchFilter = "";
    
    void OnEnable()
    {
        localizedText = (LocalizedText)target;
        FindLocalizationManager();
    }

    void FindLocalizationManager()
    {
        LocalizationManager[] managers = FindObjectsOfType<LocalizationManager>(true);
        if (managers.Length > 0)
        {
            locManager = managers[0];
            allKeys = locManager.GetAllKeys();
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        FindLocalizationManager();

        if (locManager == null)
        {
            EditorGUILayout.HelpBox("LocalizationManager not found in scene!", MessageType.Warning);
            
            if (GUILayout.Button("Find Manager"))
            {
                FindLocalizationManager();
            }
            
            serializedObject.ApplyModifiedProperties();
            return;
        }

        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("=== Localized Text ===", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        // Key selection
        EditorGUILayout.LabelField("Localization Key:", EditorStyles.boldLabel);
        
        searchFilter = EditorGUILayout.TextField("Search:", searchFilter);
        
        EditorGUILayout.BeginVertical("box");
        
        // Filter keys
        var filteredKeys = allKeys.FindAll(k => 
            string.IsNullOrEmpty(searchFilter) || 
            k.ToLower().Contains(searchFilter.ToLower()));
        
        // Current key display
        EditorGUILayout.LabelField("Current: " + localizedText.Key, EditorStyles.boldLabel);
        
        if (!string.IsNullOrEmpty(localizedText.Key))
        {
            EditorGUILayout.LabelField("RU: " + locManager.GetRussian(localizedText.Key), EditorStyles.miniLabel);
            EditorGUILayout.LabelField("EN: " + locManager.GetEnglish(localizedText.Key), EditorStyles.miniLabel);
        }
        
        EditorGUILayout.Space();
        
        // Key dropdown
        string[] displayKeys = new string[filteredKeys.Count + 1];
        displayKeys[0] = "-- Select Key --";
        for (int i = 0; i < filteredKeys.Count; i++)
        {
            displayKeys[i + 1] = filteredKeys[i];
        }
        
        int currentIndex = filteredKeys.IndexOf(localizedText.Key) + 1;
        int newIndex = EditorGUILayout.Popup("Select Key:", currentIndex, displayKeys);
        
        if (newIndex != currentIndex && newIndex > 0)
        {
            Undo.RecordObject(localizedText, "Change Localization Key");
            serializedObject.FindProperty("localizationKey").stringValue = displayKeys[newIndex];
            localizedText.ForceUpdate();
        }
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
        
        // Manual key input
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("Set Key Manually"))
        {
            // Opens a popup for manual input
            ShowKeyInputPopup();
        }
        GUI.backgroundColor = Color.white;
        
        if (GUILayout.Button("Clear"))
        {
            Undo.RecordObject(localizedText, "Clear Localization Key");
            serializedObject.FindProperty("localizationKey").stringValue = "";
            localizedText.ClearKey();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Preview
        EditorGUILayout.LabelField("Preview:", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginVertical("box");
        
        string ruText = locManager.GetRussian(localizedText.Key);
        string enText = locManager.GetEnglish(localizedText.Key);
        
        EditorGUILayout.LabelField("RU: " + (string.IsNullOrEmpty(ruText) ? "(empty)" : ruText));
        EditorGUILayout.LabelField("EN: " + (string.IsNullOrEmpty(enText) ? "(empty)" : enText));
        
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Current Text: " + localizedText.CurrentText, EditorStyles.boldLabel);
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
        
        // Actions
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("🔄 Update Now"))
        {
            localizedText.ForceUpdate();
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Reference to key list
        EditorGUILayout.HelpBox(
            "Open Tools > Localization Editor to manage all translation keys.",
            MessageType.Info);
        
        serializedObject.ApplyModifiedProperties();
    }

    void ShowKeyInputPopup()
    {
        string input = EditorInputDialog.Show("Enter Key", "Enter localization key:", localizedText.Key);
        if (!string.IsNullOrEmpty(input))
        {
            Undo.RecordObject(localizedText, "Set Localization Key");
            serializedObject.FindProperty("localizationKey").stringValue = input;
            localizedText.ForceUpdate();
        }
    }
}

public class EditorInputDialog
{
    public static string Show(string title, string message, string defaultValue = "")
    {
        EditorUtility.DisplayDialog(title, message, "OK");
        return defaultValue;
    }
}
