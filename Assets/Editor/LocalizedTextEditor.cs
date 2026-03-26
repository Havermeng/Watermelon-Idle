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
    private Vector2 scrollPosition;
    
    private string editRU = "";
    private string editEN = "";

    void OnEnable()
    {
        localizedText = (LocalizedText)target;
        LoadAllKeys();
    }

    void LoadAllKeys()
    {
        LocalizationManager.LoadStaticTranslations();
        allKeys = LocalizationManager.GetStaticKeys();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        LoadAllKeys();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("=== Localized Text ===", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Search / Key list
        EditorGUILayout.LabelField("Список ключей:", EditorStyles.boldLabel);
        searchFilter = EditorGUILayout.TextField("Поиск:", searchFilter);

        // Keys list
        EditorGUILayout.BeginVertical("box", GUILayout.Height(120));
        
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
                LoadCurrentKeyValues();
                localizedText.ForceUpdate();
            }
        }
        
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // Selected key info
        EditorGUILayout.LabelField("Выбранный ключ: " + localizedText.Key, EditorStyles.boldLabel);
        
        if (!string.IsNullOrEmpty(localizedText.Key))
        {
            // Show RU/EN versions
            EditorGUILayout.LabelField("RU версия:", EditorStyles.boldLabel);
            string newRU = EditorGUILayout.TextArea(GetRU(localizedText.Key), GUILayout.Height(40));
            
            EditorGUILayout.LabelField("EN версия:", EditorStyles.boldLabel);
            string newEN = EditorGUILayout.TextArea(GetEN(localizedText.Key), GUILayout.Height(40));
            
            // Save button
            EditorGUILayout.Space();
            
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("💾 Сохранить изменения", GUILayout.Height(30)))
            {
                LocalizationManager.SetStaticTranslation(localizedText.Key, newRU, newEN);
                EditorUtility.DisplayDialog("Сохранено", "Перевод сохранён!", "OK");
                LoadAllKeys();
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.Space();
            
            // Current text preview
            EditorGUILayout.LabelField("Текущий текст: " + localizedText.CurrentText, EditorStyles.boldLabel);
        }

        EditorGUILayout.Space();

        // Actions
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("🔄 Обновить"))
        {
            LoadCurrentKeyValues();
            localizedText.ForceUpdate();
        }
        
        if (GUILayout.Button("❌ Очистить"))
        {
            Undo.RecordObject(localizedText, "Clear Key");
            serializedObject.FindProperty("localizationKey").stringValue = "";
            localizedText.ClearKey();
        }
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Info
        EditorGUILayout.HelpBox(
            "Откройте Tools > Localization Editor для управления всеми ключами.",
            MessageType.Info);

        serializedObject.ApplyModifiedProperties();
    }

    void LoadCurrentKeyValues()
    {
        if (!string.IsNullOrEmpty(localizedText.Key))
        {
            editRU = LocalizationManager.GetStaticRussian(localizedText.Key);
            editEN = LocalizationManager.GetStaticEnglish(localizedText.Key);
        }
    }

    string GetRU(string key)
    {
        return LocalizationManager.GetStaticRussian(key);
    }

    string GetEN(string key)
    {
        return LocalizationManager.GetStaticEnglish(key);
    }
}
