using UnityEngine;
using UnityEditor;
using TMPro;
using System.Collections.Generic;

[CustomEditor(typeof(LocalizedText))]
public class LocalizedTextEditor : Editor
{
    private LocalizedText localizedText;
    private LocalizationManager locManager;
    private List<string> allKeys = new List<string>();
    
    private string searchFilter = "";
    private Vector2 scrollPosition;
    
    private string editRU = "";
    private string editEN = "";
    private bool isEditing = false;

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
            LoadCurrentKeyValues();
        }
    }

    void LoadCurrentKeyValues()
    {
        if (locManager != null && !string.IsNullOrEmpty(localizedText.Key))
        {
            editRU = locManager.GetRussian(localizedText.Key);
            editEN = locManager.GetEnglish(localizedText.Key);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        FindLocalizationManager();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("=== Localized Text ===", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Search / Key list
        EditorGUILayout.LabelField("Список ключей:", EditorStyles.boldLabel);
        searchFilter = EditorGUILayout.TextField("Поиск:", searchFilter);
        
        if (locManager == null)
        {
            EditorGUILayout.HelpBox("LocalizationManager не найден!", MessageType.Warning);
            
            if (GUILayout.Button("Создать LocalizationManager"))
            {
                GameObject go = new GameObject("LocalizationManager");
                locManager = go.AddComponent<LocalizationManager>();
                DontDestroyOnLoad(go);
                allKeys = locManager.GetAllKeys();
            }
            
            serializedObject.ApplyModifiedProperties();
            return;
        }

        // Keys list
        EditorGUILayout.BeginVertical("box", GUILayout.Height(150));
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        var filteredKeys = allKeys.FindAll(k => 
            string.IsNullOrEmpty(searchFilter) || 
            k.ToLower().Contains(searchFilter.ToLower()));
        
        foreach (var key in filteredKeys)
        {
            bool isSelected = (key == localizedText.Key);
            
            GUI.backgroundColor = isSelected ? Color.yellow : Color.white;
            
            if (GUILayout.Button(key, isSelected ? "Button" : "Label"))
            {
                Undo.RecordObject(localizedText, "Change Key");
                serializedObject.FindProperty("localizationKey").stringValue = key;
                LoadCurrentKeyValues();
                localizedText.ForceUpdate();
                isEditing = false;
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
            GUI.backgroundColor = new Color(0.9f, 0.95f, 1f);
            EditorGUILayout.LabelField("RU версия:", EditorStyles.boldLabel);
            GUI.backgroundColor = Color.white;
            
            GUI.backgroundColor = new Color(0.9f, 0.95f, 1f);
            string newRU = EditorGUILayout.TextArea(editRU, GUILayout.Height(40));
            GUI.backgroundColor = Color.white;
            
            GUI.backgroundColor = new Color(0.9f, 1f, 0.9f);
            EditorGUILayout.LabelField("EN версия:", EditorStyles.boldLabel);
            GUI.backgroundColor = Color.white;
            
            GUI.backgroundColor = new Color(0.9f, 1f, 0.9f);
            string newEN = EditorGUILayout.TextArea(editEN, GUILayout.Height(40));
            GUI.backgroundColor = Color.white;
            
            // Save button
            EditorGUILayout.Space();
            
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("💾 Сохранить изменения", GUILayout.Height(35)))
            {
                bool changed = false;
                
                if (editRU != newRU)
                {
                    editRU = newRU;
                    changed = true;
                }
                
                if (editEN != newEN)
                {
                    editEN = newEN;
                    changed = true;
                }
                
                if (changed)
                {
                    locManager.SetTranslation(localizedText.Key, editRU, editEN);
                    EditorUtility.SetDirty(locManager);
                    localizedText.ForceUpdate();
                    EditorUtility.DisplayDialog("Сохранено", "Перевод сохранён!", "OK");
                }
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
            editRU = "";
            editEN = "";
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
}
