using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Настраивает Canvas Scaler для корректного отображения в WebGL
/// </summary>
public class WebGLCanvasScaler : MonoBehaviour
{
    [Header("Настройки масштабирования")]
    public float referenceResolutionWidth = 1920f;
    public float referenceResolutionHeight = 1080f;
    [Range(0f, 1f)] public float matchWidthOrHeight = 0.5f;

    private CanvasScaler canvasScaler;

    void Awake()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SetupCanvasScaler();
#endif
    }

    void SetupCanvasScaler()
    {
        canvasScaler = GetComponent<CanvasScaler>();
        if (canvasScaler == null)
        {
            canvasScaler = gameObject.AddComponent<CanvasScaler>();
        }

        // Оптимальные настройки для WebGL
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(referenceResolutionWidth, referenceResolutionHeight);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        // 0.5f - баланс между шириной и высотой, чтобы избежать пустых областей
        canvasScaler.matchWidthOrHeight = 0.5f;
        
        // Дополнительные настройки для лучшего отображения
        canvasScaler.scaleFactor = 1f;
        canvasScaler.dynamicPixelsPerUnit = 1f;

        Debug.Log($"WebGLCanvasScaler: настроен Canvas Scaler на {referenceResolutionWidth}x{referenceResolutionHeight}, match=0.5f");
    }
}