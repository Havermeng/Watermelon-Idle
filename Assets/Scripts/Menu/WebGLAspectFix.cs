using UnityEngine;

/// <summary>
/// Исправляет соотношение сторон камеры в WebGL для устранения чёрных полос
/// </summary>
public class WebGLAspectFix : MonoBehaviour
{
    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        float targetAspect = 16f / 9f;
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Camera camera = GetComponent<Camera>();
        if (camera == null)
        {
            camera = Camera.main;
        }

        if (camera == null)
        {
            Debug.LogWarning("WebGLAspectFix: Камера не найдена!");
            return;
        }

        if (scaleHeight < 1.0f)
        {
            Rect rect = camera.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            camera.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = camera.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            camera.rect = rect;
        }

        Debug.Log($"WebGLAspectFix: Соотношение сторон исправлено ({Screen.width}x{Screen.height})");
#endif
    }
}
