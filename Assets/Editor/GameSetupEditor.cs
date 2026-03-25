using UnityEngine;
using UnityEditor;
using TMPro;

public class GameSetupEditor : EditorWindow
{
    [MenuItem("Tools/Watermelon Idle/Full Setup")]
    public static void ShowWindow()
    {
        GetWindow<GameSetupEditor>("Game Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("Watermelon Idle - Scene Setup", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("Setup MainMenu Scene", GUILayout.Height(40)))
        {
            SetupMainMenuScene();
        }

        if (GUILayout.Button("Setup ArbuzFerma Scene", GUILayout.Height(40)))
        {
            SetupArbuzFermaScene();
        }

        if (GUILayout.Button("Add GameReady to All Scenes", GUILayout.Height(40)))
        {
            AddGameReadyToScenes();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "MainMenu: Добавляет Canvas Fixer и LocalizedText компоненты\n" +
            "ArbuzFerma: Добавляет UI Fixer, HarvestTextPool, SceneLoader\n" +
            "GameReady: Добавляет вызов GameReadyBridge",
            MessageType.Info);
    }

    static void SetupMainMenuScene()
    {
        Debug.Log("=== Setting up MainMenu Scene ===");

        SetupCanvas("MainMenu");
        SetupLocalizationOnTexts();

        Debug.Log("MainMenu scene setup complete!");
    }

    static void SetupArbuzFermaScene()
    {
        Debug.Log("=== Setting up ArbuzFerma Scene ===");

        SetupCanvas("ArbuzFerma");
        SetupLocalizationOnTexts();
        SetupHarvestTextPool();
        SetupSceneLoader();
        SetupGameReadyBridge();

        Debug.Log("ArbuzFerma scene setup complete!");
    }

    static void SetupCanvas(string sceneName)
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>(true);

        if (canvases.Length == 0)
        {
            Debug.LogWarning($"No Canvas found in scene {sceneName}");
            return;
        }

        foreach (Canvas canvas in canvases)
        {
            if (canvas.gameObject.GetComponent<UILayoutFixer>() == null)
            {
                canvas.gameObject.AddComponent<UILayoutFixer>();
                Debug.Log($"Added UILayoutFixer to {canvas.name}");
            }
        }
    }

    static void SetupLocalizationOnTexts()
    {
        TMPro.TMP_Text[] texts = FindObjectsOfType<TMP_Text>(true);

        foreach (TMP_Text text in texts)
        {
            string currentText = text.text.Trim().ToUpper();

            string key = FindLocalizationKey(currentText);

            if (!string.IsNullOrEmpty(key) && text.GetComponent<LocalizedText>() == null)
            {
                LocalizedText lt = text.gameObject.AddComponent<LocalizedText>();
                lt.SetKey(key);
                Debug.Log($"Added LocalizedText ({key}) to {text.gameObject.name}");
            }
        }
    }

    static string FindLocalizationKey(string text)
    {
        text = text.Replace(" ", "_").Replace("\n", "_");

        if (text.Contains("WATERMELON_FARM")) return "game_title";
        if (text.Contains("НАЧАТЬ_ИГРУ") || text.Contains("START_GAME")) return "start_game";
        if (text.Contains("НАСТРОЙКИ") || text.Contains("SETTINGS")) return "settings";
        if (text.Contains("ВЫЙТИ") || text.Contains("QUIT")) return "quit";
        if (text.Contains("СОХРАНИТЬ") || text.Contains("SAVE")) return "save";
        if (text.Contains("ВЫХОД") || text.Contains("EXIT")) return "exit";
        if (text.Contains("НАЗАД") || text.Contains("BACK")) return "back";
        if (text.Contains("ИГРАТЬ") || text.Contains("PLAY")) return "play";
        if (text.Contains("СБРОС") || text.Contains("RESET")) return "reset";
        if (text.Contains("ПЕРЕИМЕНОВАТЬ") || text.Contains("RENAME")) return "rename";
        if (text.Contains("ПУСТО") || text.Contains("EMPTY")) return "slot_empty";
        if (text.Contains("МАГАЗИН") || text.Contains("SHOP")) return "open_shop";
        if (text.Contains("АУДИО") || text.Contains("AUDIO")) return "audio";
        if (text.Contains("ГРОМКОСТЬ_МУЗЫКИ") || text.Contains("MUSIC_VOLUME")) return "music_volume";
        if (text.Contains("ГРОМКОСТЬ_ЗВУКОВ") || text.Contains("SFX_VOLUME")) return "sfx_volume";
        if (text.Contains("СЛОТ") || text.Contains("SLOT")) return "slot";
        if (text.Contains("ВЕРСИИ") || text.Contains("VERSIONS")) return "versions";
        if (text.Contains("ПАУЗА") || text.Contains("PAUSED")) return "paused";
        if (text.Contains("ПРОДОЛЖИТЬ") || text.Contains("RESUME")) return "resume";
        if (text.Contains("ГЛАВНОЕ_МЕНЮ") || text.Contains("MAIN_MENU")) return "main_menu";
        if (text.Contains("ЗАГРУЗКА") || text.Contains("LOADING")) return "loading";
        if (text.Contains("ПОДТВЕРДИТЬ") || text.Contains("CONFIRM")) return "confirm";
        if (text.Contains("ОТМЕНА") || text.Contains("CANCEL")) return "cancel";
        if (text.Contains("УДАЛИТЬ") || text.Contains("DELETE")) return "delete_save";
        if (text.Contains("ЗАГРУЗИТЬ") || text.Contains("LOAD")) return "load_save";

        return null;
    }

    static void SetupHarvestTextPool()
    {
        HarvestTextPool[] pools = FindObjectsOfType<HarvestTextPool>(true);

        if (pools.Length > 0)
        {
            Debug.Log("HarvestTextPool already exists");
            return;
        }

        GameObject poolObj = new GameObject("HarvestTextPool");
        poolObj.AddComponent<HarvestTextPool>();

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            poolObj.transform.SetParent(canvas.transform, false);
        }

        Debug.Log("Created HarvestTextPool");
    }

    static void SetupSceneLoader()
    {
        SceneLoader[] loaders = FindObjectsOfType<SceneLoader>(true);

        if (loaders.Length > 0)
        {
            Debug.Log("SceneLoader already exists");
            return;
        }

        GameObject loaderObj = new GameObject("SceneLoader");
        SceneLoader loader = loaderObj.AddComponent<SceneLoader>();

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            loaderObj.transform.SetParent(canvas.transform, false);
        }

        GameObject loadingScreen = new GameObject("LoadingScreen");
        loadingScreen.transform.SetParent(loaderObj.transform, false);
        loadingScreen.AddComponent<UnityEngine.RectTransform>();
        UnityEngine.UI.Image bg = loadingScreen.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0, 0, 0, 0.8f);
        loadingScreen.SetActive(false);

        RectTransform rt = loadingScreen.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        GameObject sliderObj = new GameObject("ProgressSlider");
        sliderObj.transform.SetParent(loadingScreen.transform, false);
        UnityEngine.UI.Slider slider = sliderObj.AddComponent<UnityEngine.UI.Slider>();
        slider.wholeNumbers = false;
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 0;

        RectTransform sliderRT = sliderObj.AddComponent<RectTransform>();
        sliderRT.anchorMin = new Vector2(0.2f, 0.4f);
        sliderRT.anchorMax = new Vector2(0.8f, 0.6f);
        sliderRT.sizeDelta = Vector2.zero;

        GameObject textObj = new GameObject("ProgressText");
        textObj.transform.SetParent(loadingScreen.transform, false);
        TMPro.TMP_Text tmp = textObj.AddComponent<TMPro.TMP_Text>();
        tmp.text = "0%";
        tmp.alignment = TMPro.TextAlignmentOptions.Center;

        RectTransform textRT = textObj.AddComponent<RectTransform>();
        textRT.anchorMin = new Vector2(0.3f, 0.7f);
        textRT.anchorMax = new Vector2(0.7f, 0.9f);
        textRT.sizeDelta = Vector2.zero;

        SerializedObject so = new SerializedObject(loader);
        so.FindProperty("loadingScreen").objectReferenceValue = loadingScreen;
        so.FindProperty("progressSlider").objectReferenceValue = slider;
        so.ApplyModifiedProperties();

        Debug.Log("Created SceneLoader with LoadingScreen");
    }

    static void AddGameReadyToScenes()
    {
        SetupGameReadyBridge();
        Debug.Log("GameReady integration check complete!");
    }

    static void SetupGameReadyBridge()
    {
        GameReadyBridge[] bridges = FindObjectsOfType<GameReadyBridge>(true);

        if (bridges.Length > 0)
        {
            Debug.Log("GameReadyBridge already exists");
            return;
        }

        GameObject bridgeObj = new GameObject("GameReadyBridge");
        bridgeObj.AddComponent<GameReadyBridge>();
        DontDestroyOnLoad(bridgeObj);

        Debug.Log("Created GameReadyBridge");
    }
}
