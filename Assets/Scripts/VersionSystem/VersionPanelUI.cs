using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// UI панель для отображения истории версий (Patch Notes).
/// Управляет отображением списка версий и анимациями.
/// </summary>
public class VersionPanelUI : MonoBehaviour
{
    public static VersionPanelUI Instance { get; private set; }

    [Header("UI References")]
    public TextMeshProUGUI versionText;
    public RectTransform panelRectTransform;
    public CanvasGroup canvasGroup;

    [Header("Animation Settings")]
    [SerializeField] private float openDuration = 0.3f;
    [SerializeField] private float closeDuration = 0.2f;
    [SerializeField] private AnimationCurve openEase = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve closeEase = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private bool isOpen = false;
    private Coroutine animationCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        // Находим компоненты если не назначены
        if (panelRectTransform == null)
            panelRectTransform = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Скрываем панель изначально
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        Refresh();
    }

    void Start()
    {
        Refresh();
        
        // Подписываемся на изменение версии
        if (VersionManager.Instance != null)
        {
            VersionManager.Instance.OnVersionChanged += Refresh;
        }
    }

    void OnDestroy()
    {
        if (VersionManager.Instance != null)
        {
            VersionManager.Instance.OnVersionChanged -= Refresh;
        }
    }

    /// <summary>
    /// Обновляет отображаемый текст версий
    /// </summary>
    public void Refresh()
    {
        if (VersionManager.Instance == null)
        {
            Debug.LogWarning("VersionPanelUI: VersionManager.Instance is null");
            if (versionText != null)
                versionText.text = "Ошибка: VersionManager не найден";
            return;
        }

        var versions = VersionManager.Instance.GetVersions();
        string currentVersion = VersionManager.Instance.GetCurrentVersion();

        // Сортируем версии по убыванию (новые сначала)
        versions.Sort((a, b) => VersionManager.Instance.CompareVersions(b.version, a.version));

        // Формируем текст
        string result = $"<size=32><b>📋 История версий</b></size>\n\n";
        result += $"<size=24><color=#88ccff>Текущая версия: v{currentVersion}</color></size>\n\n";

        for (int i = 0; i < versions.Count; i++)
        {
            var v = versions[i];
            bool isCurrent = v.version == currentVersion;
            
            string versionHeader = $"<size=28><b>v{v.version}</b></size>";
            if (isCurrent)
            {
                versionHeader += " <size=20><color=#88ff88>✓ Текущая</color></size>";
            }

            result += $"{versionHeader}\n{v.changes}\n\n";
        }

        if (versionText != null)
        {
            versionText.text = result;
        }
    }

    /// <summary>
    /// Открывает панель с анимацией
    /// </summary>
    public void OpenPanel()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        // Обновляем текст перед открытием
        Refresh();
        
        animationCoroutine = StartCoroutine(OpenAnimation());
        isOpen = true;
    }

    /// <summary>
    /// Закрывает панель с анимацией
    /// </summary>
    public void ClosePanel()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(CloseAnimation());
        isOpen = false;
    }

    /// <summary>
    /// Анимация открытия
    /// </summary>
    IEnumerator OpenAnimation()
    {
        gameObject.SetActive(true);

        float elapsed = 0f;
        Vector2 startScale = new Vector2(0.8f, 0.8f);
        Vector2 targetScale = Vector2.one;
        float startAlpha = 0f;
        float targetAlpha = 1f;

        while (elapsed < openDuration)
        {
            float t = openEase.Evaluate(elapsed / openDuration);
            
            // Масштаб
            Vector2 currentScale = Vector2.Lerp(startScale, targetScale, t);
            panelRectTransform.localScale = new Vector3(currentScale.x, currentScale.y, 1f);

            // Альфа
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        panelRectTransform.localScale = Vector3.one;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = targetAlpha;
        }
    }

    /// <summary>
    /// Анимация закрытия
    /// </summary>
    IEnumerator CloseAnimation()
    {
        float elapsed = 0f;
        Vector2 startScale = panelRectTransform.localScale;
        Vector2 targetScale = new Vector2(0.8f, 0.8f);
        float startAlpha = canvasGroup != null ? canvasGroup.alpha : 1f;
        float targetAlpha = 0f;

        while (elapsed < closeDuration)
        {
            float t = closeEase.Evaluate(elapsed / closeDuration);
            
            // Масштаб
            Vector2 currentScale = Vector2.Lerp(startScale, targetScale, t);
            panelRectTransform.localScale = new Vector3(currentScale.x, currentScale.y, 1f);

            // Альфа
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Переключатель панели (открыть/закрыть)
    /// </summary>
    public void TogglePanel()
    {
        if (isOpen)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }

    /// <summary>
    /// Принудительно закрывает панель без анимации
    /// </summary>
    public void ForceClose()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        gameObject.SetActive(false);
        isOpen = false;
    }
}