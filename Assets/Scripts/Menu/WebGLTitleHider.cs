using UnityEngine;

/// <summary>
/// Скрывает объект в браузерной версии (WebGL) при необходимости
/// </summary>
public class WebGLTitleHider : MonoBehaviour
{
    /// <summary>
    /// Скрывать объект в WebGL сборке? По умолчанию false (не скрывать)
    /// </summary>
    public bool hideInWebGL = false;

    private void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (hideInWebGL)
        {
            // Скрываем объект только в WebGL сборке, если включено
            gameObject.SetActive(false);
        }
#endif
    }
}
