using UnityEngine;
using UnityEditor;
using TMPro;
using System.Collections.Generic;

[CustomEditor(typeof(LocalizedText))]
public class LocalizedTextEditor : Editor
{
    private LocalizedText localizedText;
    private List<string> allKeys = new List<string>();
    
    private string searchFilter = "";
    
    private string newKeyName = "";
    private string newKeyRU = "";
    private string newKeyEN = "";
    
    private Vector2 scrollPosition;

    void OnEnable()
    {
        localizedText = (LocalizedText)target;
        LoadKeys();
    }

    void LoadKeys()
    {
        LocalizationManager.LoadStaticTranslations();
        allKeys = LocalizationManager.GetStaticKeys();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("=== Localized Text ===", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Search
        EditorGUILayout.LabelField("Список ключей:", EditorStyles.boldLabel);
        searchFilter = EditorGUILayout.TextField("Поиск:", searchFilter);

        // Keys list
        EditorGUILayout.BeginVertical("box", GUILayout.Height(100));
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        var filteredKeys = allKeys.FindAll(k => 
            string.IsNullOrEmpty(searchFilter) || 
            k.ToLower().Contains(searchFilter.ToLower()));
        
        foreach (var key in filteredKeys)
        {
            bool isSelected = (key == localizedText.Key);
            GUI.backgroundColor = isSelected ? Color.yellow : Color.white;
            
            if (GUILayout.Button(key, isSelected ? "Button" : "Label", GUILayout.Height(20)))
            {
                Undo.RecordObject(localizedText, "Change Key");
                serializedObject.FindProperty("localizationKey").stringValue = key;
                localizedText.ForceUpdate();
            }
        }
        
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // Selected key
        EditorGUILayout.LabelField("Выбранный ключ: " + localizedText.Key, EditorStyles.boldLabel);

        if (!string.IsNullOrEmpty(localizedText.Key))
        {
            string ru = LocalizationManager.GetStaticRussian(localizedText.Key);
            string en = LocalizationManager.GetStaticEnglish(localizedText.Key);
            
            EditorGUILayout.LabelField("RU версия: " + ru);
            EditorGUILayout.LabelField("EN версия: " + en);
        }

        EditorGUILayout.Space();

        // Edit section
        GUI.backgroundColor = new Color(0.9f, 1f, 0.9f);
        EditorGUILayout.BeginVertical("box");
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.LabelField("Редактор ключей:", EditorStyles.boldLabel);
        
        newKeyName = EditorGUILayout.TextField("Название:", localizedText.Key);
        
        EditorGUILayout.LabelField("RU:");
        newKeyRU = EditorGUILayout.TextArea(string.IsNullOrEmpty(localizedText.Key) ? "" : 
            LocalizationManager.GetStaticRussian(localizedText.Key), GUILayout.Height(30));
        
        EditorGUILayout.LabelField("EN:");
        newKeyEN = EditorGUILayout.TextArea(string.IsNullOrEmpty(localizedText.Key) ? "" : 
            LocalizationManager.GetStaticEnglish(localizedText.Key), GUILayout.Height(30));
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("💾 изменить"))
        {
            if (!string.IsNullOrEmpty(localizedText.Key))
            {
                LocalizationManager.SetStaticTranslation(localizedText.Key, newKeyRU, newKeyEN);
                LoadKeys();
                EditorUtility.DisplayDialog("Сохранено", "Ключ изменён!", "OK");
            }
        }
        GUI.backgroundColor = Color.white;
        
        GUI.backgroundColor = new Color(0.5f, 1f, 0.5f);
        if (GUILayout.Button("➕ добавить"))
        {
            string keyToAdd = !string.IsNullOrEmpty(newKeyName) ? newKeyName : "new_key_" + Random.Range(1000, 9999);
            string ruToAdd = !string.IsNullOrEmpty(newKeyRU) ? newKeyRU : keyToAdd;
            string enToAdd = !string.IsNullOrEmpty(newKeyEN) ? newKeyEN : keyToAdd;
            
            LocalizationManager.SetStaticTranslation(keyToAdd, ruToAdd, enToAdd);
            Undo.RecordObject(localizedText, "Set Key");
            serializedObject.FindProperty("localizationKey").stringValue = keyToAdd;
            localizedText.ForceUpdate();
            LoadKeys();
            EditorUtility.DisplayDialog("Добавлено", $"Ключ '{keyToAdd}' добавлен!", "OK");
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // Actions
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("🔄 обновить"))
        {
            localizedText.ForceUpdate();
        }
        
        if (GUILayout.Button("❌ очистить"))
        {
            Undo.RecordObject(localizedText, "Clear Key");
            serializedObject.FindProperty("localizationKey").stringValue = "";
            localizedText.ClearKey();
        }
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Current text
        if (!string.IsNullOrEmpty(localizedText.Key))
        {
            EditorGUILayout.LabelField("Текущий текст: " + localizedText.CurrentText, EditorStyles.boldLabel);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
