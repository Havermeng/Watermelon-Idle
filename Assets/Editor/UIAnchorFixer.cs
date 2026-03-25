using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class UIAnchorFixer : EditorWindow
{
    [MenuItem("Tools/UI Anchor Fixer")]
    public static void ShowWindow()
    {
        GetWindow<UIAnchorFixer>("UI Anchor Fixer");
    }

    void OnGUI()
    {
        GUILayout.Label("Настройка якорей UI", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("Выберите UI элемент в сцене и нажмите соответствующую кнопку:", EditorStyles.wordWrappedLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Левый нижний угол (Back/Exit)", GUILayout.Height(30)))
        {
            SetAnchor(new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(20, 20));
        }

        if (GUILayout.Button("Правый верхний угол", GUILayout.Height(30)))
        {
            SetAnchor(new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-20, -20));
        }

        if (GUILayout.Button("Верхний левый угол (Coins/Shop)", GUILayout.Height(30)))
        {
            SetAnchor(new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -20));
        }

        if (GUILayout.Button("Центральная панель (Shop/Settings)", GUILayout.Height(30)))
        {
            SetAnchor(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero);
        }

        GUILayout.Space(10);
        GUILayout.Label("Примечание: Убедитесь, что выбран RectTransform", EditorStyles.miniLabel);
    }

    void SetAnchor(Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position)
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Ошибка", "Сначала выберите UI элемент в сцене!", "OK");
            return;
        }

        RectTransform rt = selected.GetComponent<RectTransform>();
        if (rt == null)
        {
            EditorUtility.DisplayDialog("Ошибка", "У выбранного объекта нет RectTransform!", "OK");
            return;
        }

        Undo.RecordObject(rt, "Set Anchor Preset");

        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = position;

        EditorUtility.SetDirty(rt);
        Debug.Log($"Настроен RectTransform для '{selected.name}':\n" +
                 $"Anchor Min: {anchorMin}\n" +
                 $"Anchor Max: {anchorMax}\n" +
                 $"Pivot: {pivot}\n" +
                 $"Position: {position}");
    }
}