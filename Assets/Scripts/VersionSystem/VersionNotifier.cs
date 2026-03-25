using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Уведомляет игрока о новой версии при запуске игры.
/// Показывает анимированное уведомление с кнопкой "Посмотреть изменения".
/// </summary>
public class VersionNotifier : MonoBehaviour
{
    public static VersionNotifier Instance;

    [Header("Notification UI")]
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private Transform notificationParent;
    [SerializeField] private float notificationDuration = 8f;

    [Header("Auto-show on Start")]
    [SerializeField] private bool showOnStart = true;

    private GameObject currentNotification;
    private bool hasBeenShown = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (showOnStart && !hasBeenShown)
        {
            CheckAndShowNotification();
        }
    }

    /// <summary>
    /// Проверяет, есть ли новая версия, и показывает уведомление если нужно
    /// </summary>
    public void CheckAndShowNotification()
    {
        if (VersionManager.Instance == null)
        {
            Debug.LogWarning("VersionNotifier: VersionManager not found, skipping notification check");
            return;
        }

        if (VersionManager.Instance.IsNewVersion())
        {
            Debug.Log("VersionNotifier: New version detected! Showing notification.");
            ShowUpdateNotification();
        }
        else
        {
            Debug.Log("VersionNotifier: No new version detected.");
        }
    }

    /// <summary>
    /// Показывает уведомление об обновлении
    /// </summary>
    public void ShowUpdateNotification()
    {
        if (hasBeenShown)
        {
            Debug.Log("VersionNotifier: Notification already shown this session.");
            return;
        }

        if (currentNotification != null)
        {
            Destroy(currentNotification);
        }

        // Создаем уведомление
        if (notificationPrefab != null)
        {
            currentNotification = Instantiate(notificationPrefab, notificationParent);
        }
        else
        {
            currentNotification = CreateDefaultNotification();
        }

        // Настраиваем текст
        TextMeshProUGUI titleText = currentNotification.transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI messageText = currentNotification.transform.Find("MessageText")?.GetComponent<TextMeshProUGUI>();
        Button viewButton = currentNotification.transform.Find("ViewButton")?.GetComponent<Button>();
        Button closeButton = currentNotification.transform.Find("CloseButton")?.GetComponent<Button>();

        if (titleText != null)
        {
            titleText.text = "🎉 Обновление!";
            if (LocalizationManager.Instance != null)
            {
                titleText.text = LocalizationManager.Instance.Get("update_available", "🎉 Обновление!");
            }
        }

        if (messageText != null)
        {
            string currentVersion = VersionManager.Instance.GetCurrentVersion();
            messageText.text = $"Доступна новая версия {currentVersion}\nПосмотрите список изменений.";
            
            if (LocalizationManager.Instance != null)
            {
                messageText.text = LocalizationManager.Instance.Get("update_message", 
                    $"Доступна новая версия {currentVersion}\nПосмотрите список изменений.");
            }
        }

        if (viewButton != null)
        {
            string buttonText = "📋 Посмотреть";
            if (LocalizationManager.Instance != null)
            {
                buttonText = LocalizationManager.Instance.Get("view_changes", "📋 Посмотреть");
            }
            
            TextMeshProUGUI btnText = viewButton.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null) btnText.text = buttonText;

            viewButton.onClick.AddListener(() =>
            {
                HideNotification();
                if (VersionUIManager.Instance != null)
                {
                    VersionUIManager.Instance.ShowVersionPanel();
                }
            });
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideNotification);
        }

        hasBeenShown = true;

        // Автоматическое скрытие через время
        StartCoroutine(AutoHideNotification(notificationDuration));
    }

    /// <summary>
    /// Создает уведомление по умолчанию если нет префаба
    /// </summary>
    GameObject CreateDefaultNotification()
    {
        GameObject notification = new GameObject("VersionNotification");
        
        Transform parent = notificationParent;
        if (parent == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                parent = canvas.transform;
            }
            else
            {
                Debug.LogError("VersionNotifier: No Canvas found in scene! Cannot create notification.");
                return null;
            }
        }
        
        notification.transform.SetParent(parent, false);

        RectTransform rt = notification.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = new Vector2(400, 120);
        rt.anchoredPosition = new Vector2(0, -20);

        // Фон
        Image bg = notification.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.35f, 0.65f, 0.95f);

        // Добавляем тень
        GameObject shadow = new GameObject("Shadow");
        shadow.transform.SetParent(notification.transform, false);
        Image shadowImg = shadow.AddComponent<Image>();
        shadowImg.color = new Color(0, 0, 0, 0.4f);
        RectTransform shadowRT = shadowImg.GetComponent<RectTransform>();
        shadowRT.anchorMin = Vector2.zero;
        shadowRT.anchorMax = Vector2.one;
        shadowRT.offsetMin = new Vector2(4, -4);
        shadowRT.offsetMax = new Vector2(4, -4);

        // Заголовок
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(notification.transform, false);
        TextMeshProUGUI titleTMP = titleObj.AddComponent<TextMeshProUGUI>();
        titleTMP.fontSize = 28;
        titleTMP.color = Color.white;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.fontStyle = FontStyles.Bold;
        RectTransform titleRT = titleTMP.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0, 0.6f);
        titleRT.anchorMax = new Vector2(1, 1);
        titleRT.offsetMin = new Vector2(20, 0);
        titleRT.offsetMax = new Vector2(-20, -10);

        // Сообщение
        GameObject msgObj = new GameObject("MessageText");
        msgObj.transform.SetParent(notification.transform, false);
        TextMeshProUGUI msgTMP = msgObj.AddComponent<TextMeshProUGUI>();
        msgTMP.fontSize = 18;
        msgTMP.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        msgTMP.alignment = TextAlignmentOptions.Center;
        msgTMP.enableWordWrapping = true;
        RectTransform msgRT = msgTMP.GetComponent<RectTransform>();
        msgRT.anchorMin = new Vector2(0, 0.2f);
        msgRT.anchorMax = new Vector2(1, 0.6f);
        msgRT.offsetMin = new Vector2(20, 0);
        msgRT.offsetMax = new Vector2(-20, 0);

        // Кнопка "Посмотреть"
        GameObject btnObj = new GameObject("ViewButton");
        btnObj.transform.SetParent(notification.transform, false);
        Button btn = btnObj.AddComponent<Button>();
        RectTransform btnRT = btn.GetComponent<RectTransform>();
        btnRT.anchorMin = new Vector2(0.05f, 0);
        btnRT.anchorMax = new Vector2(0.45f, 0.15f);
        btnRT.offsetMin = Vector2.zero;
        btnRT.offsetMax = Vector2.zero;

        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.6f, 0.3f, 0.9f);
        btn.targetGraphic = btnImg;

        GameObject btnTextObj = new GameObject("ButtonText");
        btnTextObj.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI btnText = btnTextObj.AddComponent<TextMeshProUGUI>();
        btnText.text = "📋 Посмотреть";
        btnText.fontSize = 20;
        btnText.color = Color.white;
        btnText.alignment = TextAlignmentOptions.Center;
        RectTransform btnTextRT = btnText.GetComponent<RectTransform>();
        btnTextRT.anchorMin = Vector2.zero;
        btnTextRT.anchorMax = Vector2.one;
        btnTextRT.offsetMin = Vector2.zero;
        btnTextRT.offsetMax = Vector2.zero;

        // Кнопка закрытия
        GameObject closeBtnObj = new GameObject("CloseButton");
        closeBtnObj.transform.SetParent(notification.transform, false);
        Button closeBtn = closeBtnObj.AddComponent<Button>();
        RectTransform closeBtnRT = closeBtn.GetComponent<RectTransform>();
        closeBtnRT.anchorMin = new Vector2(0.55f, 0);
        closeBtnRT.anchorMax = new Vector2(0.95f, 0.15f);
        closeBtnRT.offsetMin = Vector2.zero;
        closeBtnRT.offsetMax = Vector2.zero;

        Image closeBtnImg = closeBtnObj.AddComponent<Image>();
        closeBtnImg.color = new Color(0.8f, 0.2f, 0.2f, 0.9f);
        closeBtn.targetGraphic = closeBtnImg;

        GameObject closeBtnTextObj = new GameObject("CloseText");
        closeBtnTextObj.transform.SetParent(closeBtnObj.transform, false);
        TextMeshProUGUI closeBtnText = closeBtnTextObj.AddComponent<TextMeshProUGUI>();
        closeBtnText.text = "✕ Позже";
        closeBtnText.fontSize = 20;
        closeBtnText.color = Color.white;
        closeBtnText.alignment = TextAlignmentOptions.Center;
        RectTransform closeBtnTextRT = closeBtnText.GetComponent<RectTransform>();
        closeBtnTextRT.anchorMin = Vector2.zero;
        closeBtnTextRT.anchorMax = Vector2.one;
        closeBtnTextRT.offsetMin = Vector2.zero;
        closeBtnTextRT.offsetMax = Vector2.zero;

        closeBtn.onClick.AddListener(HideNotification);

        // Анимация появления
        notification.transform.localScale = Vector3.zero;
        notification.SetActive(true);
        StartCoroutine(ScaleInAnimation(notification));

        return notification;
    }

    /// <summary>
    /// Скрывает уведомление
    /// </summary>
    public void HideNotification()
    {
        if (currentNotification != null)
        {
            StopAllCoroutines();
            StartCoroutine(ScaleOutAnimation(currentNotification, () =>
            {
                Destroy(currentNotification);
                currentNotification = null;
            }));
        }
    }

    /// <summary>
    /// Автоматическое скрытие через время
    /// </summary>
    IEnumerator AutoHideNotification(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideNotification();
    }

    /// <summary>
    /// Анимация появления
    /// </summary>
    IEnumerator ScaleInAnimation(GameObject obj)
    {
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float scale = Mathf.SmoothStep(0f, 1f, t);
            obj.transform.localScale = new Vector3(scale, scale, scale);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Анимация исчезновения
    /// </summary>
    IEnumerator ScaleOutAnimation(GameObject obj, System.Action onComplete)
    {
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float scale = Mathf.SmoothStep(1f, 0f, t);
            obj.transform.localScale = new Vector3(scale, scale, scale);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (onComplete != null) onComplete();
    }

    /// <summary>
    /// Сбрасывает флаг показа (для тестирования)
    /// </summary>
    public void ResetNotificationFlag()
    {
        hasBeenShown = false;
    }
}