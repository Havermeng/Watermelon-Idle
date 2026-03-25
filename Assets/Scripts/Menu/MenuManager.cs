using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    
    public RectTransform mainMenuPanel;
    public RectTransform saveSlotsPanel;
    public RectTransform settingsPanel;
    public float panSpeed = 800f;

    // Ссылки на текстовые элементы UI
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI startGameButtonText;
    public TextMeshProUGUI settingsButtonText;
    public TextMeshProUGUI quitButtonText;
    public TextMeshProUGUI versionsButtonText;

    private bool isPanning = false;
    private float screenWidth = 1920f;
    private RectTransform mainMenuRectTransform;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Создаем LocalizationManager если его нет
        if (LocalizationManager.Instance == null)
        {
            GameObject locObj = new GameObject("LocalizationManager");
            locObj.AddComponent<LocalizationManager>();
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        // Настраиваем Canvas Scaler для WebGL
        Canvas mainCanvas = GetComponentInParent<Canvas>();
        if (mainCanvas != null)
        {
            if (mainCanvas.GetComponent<WebGLCanvasScaler>() == null)
            {
                mainCanvas.gameObject.AddComponent<WebGLCanvasScaler>();
            }
            
            // Добавляем фиксатор фона для WebGL
            BackgroundCanvasFixer backgroundFixer = mainCanvas.GetComponent<BackgroundCanvasFixer>();
            if (backgroundFixer == null)
            {
                backgroundFixer = mainCanvas.gameObject.AddComponent<BackgroundCanvasFixer>();
            }
        }
#endif
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Start()
    {
        // Динамически определяем ширину экрана на основе RectTransform
        if (mainMenuPanel != null)
        {
            mainMenuRectTransform = mainMenuPanel;
            screenWidth = mainMenuRectTransform.rect.width;
        }
        else
        {
            // Fallback: используем разрешение экрана
            screenWidth = Screen.width;
        }
        
        mainMenuPanel.anchoredPosition = Vector2.zero;
        saveSlotsPanel.anchoredPosition = new Vector2(screenWidth, 0f);
        settingsPanel.anchoredPosition = new Vector2(-screenWidth, 0f);
        
        // Обновляем локализованный текст
        UpdateLocalizedText();
        
        // Подписываемся на изменение языка
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateLocalizedText;
        }

        Debug.Log("MenuManager started - localization is manual");
    }

    void UpdateLocalizedText()
    {
        if (LocalizationManager.Instance == null) return;
        
        if (titleText != null)
            titleText.text = LocalizationManager.Instance.Get("game_title");
            
        if (startGameButtonText != null)
            startGameButtonText.text = LocalizationManager.Instance.Get("start_game");
            
        if (settingsButtonText != null)
            settingsButtonText.text = LocalizationManager.Instance.Get("settings");
            
        if (quitButtonText != null)
            quitButtonText.text = LocalizationManager.Instance.Get("quit");
            
        if (versionsButtonText != null)
            versionsButtonText.text = LocalizationManager.Instance.Get("versions", "📋 Версии");
    }

    public void OpenSaveSlots()
    {
        if (!isPanning) StartCoroutine(SlideTo(saveSlotsPanel));
    }

    public void OpenSettings()
    {
        if (!isPanning) StartCoroutine(SlideTo(settingsPanel));
    }

    public void BackToMainMenu()
    {
        if (!isPanning) StartCoroutine(SlideBackAndLoad());
    }

    IEnumerator SlideBackAndLoad()
    {
        yield return StartCoroutine(SlideBack());
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator SlideTo(RectTransform targetPanel)
    {
        isPanning = true;

        // Куда едет главное меню
        float direction = (targetPanel == saveSlotsPanel) ? -screenWidth : screenWidth;
        Vector2 mainTarget = new Vector2(direction, 0f);
        Vector2 panelTarget = Vector2.zero;

        while (Vector2.Distance(mainMenuPanel.anchoredPosition, mainTarget) > 1f)
        {
            mainMenuPanel.anchoredPosition = Vector2.MoveTowards(
                mainMenuPanel.anchoredPosition, mainTarget, panSpeed * Time.deltaTime);
            targetPanel.anchoredPosition = Vector2.MoveTowards(
                targetPanel.anchoredPosition, panelTarget, panSpeed * Time.deltaTime);
            yield return null;
        }

        mainMenuPanel.anchoredPosition = mainTarget;
        targetPanel.anchoredPosition = panelTarget;
        isPanning = false;
    }

    IEnumerator SlideBack()
    {
        isPanning = true;

        // Определяем какая панель сейчас активна
        RectTransform activePanel = null;
        float returnPos = 0f;

        if (Mathf.Abs(saveSlotsPanel.anchoredPosition.x) < screenWidth - 1f)
        {
            activePanel = saveSlotsPanel;
            returnPos = screenWidth;
        }
        else if (Mathf.Abs(settingsPanel.anchoredPosition.x) < screenWidth - 1f)
        {
            activePanel = settingsPanel;
            returnPos = -screenWidth;
        }

        if (activePanel == null) { isPanning = false; yield break; }

        Vector2 mainTarget = Vector2.zero;
        Vector2 panelTarget = new Vector2(returnPos, 0f);

        while (Vector2.Distance(mainMenuPanel.anchoredPosition, mainTarget) > 1f)
        {
            mainMenuPanel.anchoredPosition = Vector2.MoveTowards(
                mainMenuPanel.anchoredPosition, mainTarget, panSpeed * Time.deltaTime);
            activePanel.anchoredPosition = Vector2.MoveTowards(
                activePanel.anchoredPosition, panelTarget, panSpeed * Time.deltaTime);
            yield return null;
        }

        mainMenuPanel.anchoredPosition = mainTarget;
        activePanel.anchoredPosition = panelTarget;
        isPanning = false;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Выход из игры");
    }

    /// <summary>
    /// Открывает панель версий
    /// </summary>
    public void OpenVersions()
    {
        if (VersionUIManager.Instance != null)
        {
            VersionUIManager.Instance.ShowVersionPanel();
        }
        else
        {
            Debug.LogWarning("OpenVersions: VersionUIManager.Instance is null");
        }
    }
}
