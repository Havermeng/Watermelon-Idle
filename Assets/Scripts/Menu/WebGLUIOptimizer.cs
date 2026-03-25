using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Оптимизирует отображение UI в WebGL для устранения пустых областей
/// </summary>
public class WebGLUIOptimizer : MonoBehaviour
{
    [Header("Настройки оптимизации")]
    public bool optimizeCanvasScale = true;
    public bool adjustSafeArea = true;
    public float safeAreaPadding = 20f;

    private Canvas mainCanvas;
    private RectTransform canvasRect;

    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        OptimizeUI();
#endif
    }

    void OptimizeUI()
    {
        // Находим главный Canvas
        mainCanvas = GetComponent<Canvas>();
        if (mainCanvas == null)
        {
            mainCanvas = GetComponentInParent<Canvas>();
        }
        
        if (mainCanvas == null)
        {
            Debug.LogError("WebGLUIOptimizer: Canvas не найден!");
            return;
        }

        canvasRect = mainCanvas.GetComponent<RectTransform>();
        if (canvasRect == null)
        {
            Debug.LogError("WebGLUIOptimizer: RectTransform Canvas не найден!");
            return;
        }

        // Оптимизируем масштабирование Canvas
        if (optimizeCanvasScale)
        {
            OptimizeCanvasScale();
        }

        // Настраиваем безопасную область
        if (adjustSafeArea)
        {
            AdjustSafeArea();
        }

        // Убираем лишние отступы
        RemoveExtraPadding();
    }

    void OptimizeCanvasScale()
    {
        // Проверяем, есть ли CanvasScaler
        CanvasScaler canvasScaler = mainCanvas.GetComponent<CanvasScaler>();
        if (canvasScaler != null)
        {
            // Оптимизируем настройки для WebGL
            if (canvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                // Устанавливаем оптимальное значение matchWidthOrHeight
                // 0.6f - немного偏向 высоте, чтобы избежать пустых боковых областей
                if (canvasScaler.matchWidthOrHeight > 0.5f)
                {
                    canvasScaler.matchWidthOrHeight = 0.6f;
                    Debug.Log("WebGLUIOptimizer: matchWidthOrHeight оптимизирован до 0.6f");
                }
            }
        }
    }

    void AdjustSafeArea()
    {
        // Получаем безопасную область для WebGL
        Rect safeArea = Screen.safeArea;
        
        if (safeArea.width > 0 && safeArea.height > 0)
        {
            // Конвертируем безопасную область в координаты Canvas
            Vector2 anchorMin = new Vector2(safeArea.x / Screen.width, safeArea.y / Screen.height);
            Vector2 anchorMax = new Vector2((safeArea.x + safeArea.width) / Screen.width, 
                                          (safeArea.y + safeArea.height) / Screen.height);
            
            // Применяем безопасную область к Canvas
            canvasRect.anchorMin = anchorMin;
            canvasRect.anchorMax = anchorMax;
            
            Debug.Log($"WebGLUIOptimizer: Безопасная область применена: {anchorMin} - {anchorMax}");
        }
    }

    void RemoveExtraPadding()
    {
        // Находим все UI элементы и убираем лишние отступы
        RectTransform[] allRects = mainCanvas.GetComponentsInChildren<RectTransform>(true);
        
        foreach (RectTransform rt in allRects)
        {
            // Пропускаем сам Canvas и UIContainer
            if (rt == canvasRect || rt.name == "UIContainer")
                continue;
                
            // Убираем отступы для элементов в UIContainer
            if (rt.parent != null && rt.parent.name == "UIContainer")
            {
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                
                // Центрируем элементы, если они не центрированы
                if (rt.anchorMin.x == 0.5f && rt.anchorMax.x == 0.5f)
                {
                    rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y);
                }
            }
        }
        
        Debug.Log("WebGLUIOptimizer: Лишние отступы убраны");
    }

    // Дополнительный метод для принудительной перерисовки UI
    public void ForceRedraw()
    {
        if (mainCanvas != null)
        {
            Canvas.ForceUpdateCanvases();
            Debug.Log("WebGLUIOptimizer: UI принудительно перерисован");
        }
    }
}