using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Перемещает фоновое изображение в Canvas и настраивает его для правильного отображения в WebGL
/// </summary>
public class BackgroundCanvasFixer : MonoBehaviour
{
    [Header("Настройки фона")]
    public string backgroundObjectName = "Background"; // Имя объекта фона
    public Sprite defaultBackgroundSprite; // Спрайт фона (если нужно назначить)

    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        FixBackground();
#endif
    }

    void FixBackground()
    {
        // Находим главный Canvas
        Canvas mainCanvas = GetComponent<Canvas>();
        if (mainCanvas == null)
        {
            mainCanvas = GetComponentInParent<Canvas>();
        }
        
        if (mainCanvas == null)
        {
            Debug.LogError("BackgroundCanvasFixer: Canvas не найден!");
            return;
        }

        // Ищем фоновый объект на сцене
        GameObject backgroundObj = GameObject.Find(backgroundObjectName);
        if (backgroundObj == null)
        {
            // Пытаемся найти по тегу
            backgroundObj = GameObject.FindGameObjectWithTag("Background");
            if (backgroundObj == null)
            {
                Debug.LogWarning($"BackgroundCanvasFixer: Объект '{backgroundObjectName}' не найден на сцене");
                return;
            }
        }

        // Проверяем, уже ли фон в Canvas
        if (backgroundObj.transform.parent == mainCanvas.transform)
        {
            Debug.Log("BackgroundCanvasFixer: Фон уже находится в Canvas");
            // Все равно настраиваем его как Image
            SetupBackgroundAsImage(backgroundObj);
            return;
        }

        // Создаем новый объект фона внутри Canvas
        GameObject newBackground = new GameObject("Background_Fixed");
        newBackground.transform.SetParent(mainCanvas.transform, false);
        
        // Добавляем компонент Image
        Image backgroundImage = newBackground.AddComponent<Image>();
        
        // Копируем спрайт из старого объекта если есть SpriteRenderer
        SpriteRenderer oldSpriteRenderer = backgroundObj.GetComponent<SpriteRenderer>();
        if (oldSpriteRenderer != null && oldSpriteRenderer.sprite != null)
        {
            backgroundImage.sprite = oldSpriteRenderer.sprite;
        }
        else if (defaultBackgroundSprite != null)
        {
            backgroundImage.sprite = defaultBackgroundSprite;
        }
        
        // Настраиваем RectTransform для растягивания на весь экран
        RectTransform rt = newBackground.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        
        // Устанавливаем сортировку (должен быть под всеми UI элементами)
        CanvasRenderer canvasRenderer = newBackground.GetComponent<CanvasRenderer>();
        if (canvasRenderer != null)
        {
            // Устанавливаем порядок сортировки (меньше = ниже)
            // Canvas по умолчанию имеет sortingOrder = 0
            // Фон должен быть под другими элементами
        }
        
        // Делаем старый фон невидимым (или удаляем)
        backgroundObj.SetActive(false);
        SpriteRenderer oldRenderer = backgroundObj.GetComponent<SpriteRenderer>();
        if (oldRenderer != null)
        {
            oldRenderer.enabled = false;
        }
        
        Debug.Log($"BackgroundCanvasFixer: Фон перемещен в Canvas и настроен как Image");
    }

    void SetupBackgroundAsImage(GameObject backgroundObj)
    {
        // Добавляем Image компонент если его нет
        Image img = backgroundObj.GetComponent<Image>();
        if (img == null)
        {
            img = backgroundObj.AddComponent<Image>();
            
            // Пытаемся получить спрайт из SpriteRenderer
            SpriteRenderer sr = backgroundObj.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                img.sprite = sr.sprite;
                sr.enabled = false; // Отключаем SpriteRenderer
            }
        }
        
        // Настраиваем RectTransform
        RectTransform rt = backgroundObj.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
        
        Debug.Log($"BackgroundCanvasFixer: Фон '{backgroundObj.name}' настроен как Image");
    }
}