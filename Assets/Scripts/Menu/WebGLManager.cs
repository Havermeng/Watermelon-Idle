using UnityEngine;
using UnityEngine.UI;

public class WebGLManager : MonoBehaviour
{
    public static WebGLManager Instance { get; private set; }

    [Header("Canvas Settings")]
    [SerializeField] private float referenceResolutionWidth = 1920f;
    [SerializeField] private float referenceResolutionHeight = 1080f;
    
    [Header("Background Settings")]
    [SerializeField] private string backgroundObjectName = "Background";

    [Header("Button Bindings")]
    [SerializeField] private string startGameButtonName = "StartGameButton";
    [SerializeField] private string settingsButtonName = "SettingsButton";
    [SerializeField] private string quitButtonName = "QuitButton";

    [Header("Title Hider")]
    [SerializeField] private bool hideTitleInWebGL = false;

    private Canvas mainCanvas;

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
            return;
        }
    }

    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SetupCanvas();
        SetupBackground();
        BindButtons();
        
        if (hideTitleInWebGL)
        {
            HideTitle();
        }
#endif
    }

    void SetupCanvas()
    {
        mainCanvas = GetComponent<Canvas>();
        if (mainCanvas == null)
        {
            mainCanvas = GetComponentInParent<Canvas>();
        }
        
        if (mainCanvas == null) return;

        CanvasScaler scaler = mainCanvas.GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = mainCanvas.gameObject.AddComponent<CanvasScaler>();
        }
        
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(referenceResolutionWidth, referenceResolutionHeight);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        scaler.scaleFactor = 1f;
        scaler.dynamicPixelsPerUnit = 1f;

        FixAspectRatio();
        OptimizeUI();
    }

    void FixAspectRatio()
    {
        float targetAspect = 16f / 9f;
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Camera camera = Camera.main;
        if (camera == null) return;

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
    }

    void OptimizeUI()
    {
        if (mainCanvas == null) return;
        
        CanvasScaler scaler = mainCanvas.GetComponent<CanvasScaler>();
        if (scaler != null && scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
        {
            if (scaler.matchWidthOrHeight > 0.5f)
            {
                scaler.matchWidthOrHeight = 0.6f;
            }
        }

        RectTransform canvasRect = mainCanvas.GetComponent<RectTransform>();
        if (canvasRect != null)
        {
            Rect safeArea = Screen.safeArea;
            if (safeArea.width > 0 && safeArea.height > 0)
            {
                Vector2 anchorMin = new Vector2(safeArea.x / Screen.width, safeArea.y / Screen.height);
                Vector2 anchorMax = new Vector2((safeArea.x + safeArea.width) / Screen.width, 
                                              (safeArea.y + safeArea.height) / Screen.height);
                canvasRect.anchorMin = anchorMin;
                canvasRect.anchorMax = anchorMax;
            }
        }

        RemoveExtraPadding();
    }

    void RemoveExtraPadding()
    {
        if (mainCanvas == null) return;
        
        RectTransform[] allRects = mainCanvas.GetComponentsInChildren<RectTransform>(true);
        RectTransform canvasRect = mainCanvas.GetComponent<RectTransform>();
        
        foreach (RectTransform rt in allRects)
        {
            if (rt == canvasRect || rt.name == "UIContainer") continue;
            
            if (rt.parent != null && rt.parent.name == "UIContainer")
            {
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
        }
    }

    void SetupBackground()
    {
        if (mainCanvas == null) return;

        GameObject backgroundObj = GameObject.Find(backgroundObjectName);
        if (backgroundObj == null)
        {
            backgroundObj = GameObject.FindGameObjectWithTag("Background");
        }
        
        if (backgroundObj == null) return;

        if (backgroundObj.transform.parent == mainCanvas.transform) return;

        GameObject newBackground = new GameObject("Background_Fixed");
        newBackground.transform.SetParent(mainCanvas.transform, false);
        
        Image backgroundImage = newBackground.AddComponent<Image>();
        
        SpriteRenderer oldSpriteRenderer = backgroundObj.GetComponent<SpriteRenderer>();
        if (oldSpriteRenderer != null && oldSpriteRenderer.sprite != null)
        {
            backgroundImage.sprite = oldSpriteRenderer.sprite;
        }
        
        RectTransform rt = newBackground.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        
        backgroundObj.SetActive(false);
    }

    void BindButtons()
    {
        if (MenuManager.Instance == null) return;

        Button startBtn = FindButtonByName(startGameButtonName);
        if (startBtn != null)
        {
            startBtn.onClick.RemoveAllListeners();
            startBtn.onClick.AddListener(() => MenuManager.Instance.OpenSaveSlots());
        }

        Button settingsBtn = FindButtonByName(settingsButtonName);
        if (settingsBtn != null)
        {
            settingsBtn.onClick.RemoveAllListeners();
            settingsBtn.onClick.AddListener(() => MenuManager.Instance.OpenSettings());
        }

        Button quitBtn = FindButtonByName(quitButtonName);
        if (quitBtn != null)
        {
            quitBtn.onClick.RemoveAllListeners();
            quitBtn.onClick.AddListener(() => MenuManager.Instance.QuitGame());
        }
    }

    Button FindButtonByName(string buttonName)
    {
        GameObject container = GameObject.Find("UIContainer");
        if (container != null)
        {
            Transform btnTransform = container.transform.Find(buttonName);
            if (btnTransform != null)
            {
                Button btn = btnTransform.GetComponent<Button>();
                if (btn != null) return btn;
            }
        }

        Button[] allButtons = FindObjectsOfType<Button>(true);
        foreach (Button btn in allButtons)
        {
            if (btn.gameObject.name == buttonName)
            {
                return btn;
            }
        }
        return null;
    }

    void HideTitle()
    {
        GameObject title = GameObject.Find("Title");
        if (title != null)
        {
            title.SetActive(false);
        }
    }

    public void ForceRedraw()
    {
        if (mainCanvas != null)
        {
            Canvas.ForceUpdateCanvases();
        }
    }
}
