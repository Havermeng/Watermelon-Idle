using UnityEngine;
using System.Collections.Generic;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    [SerializeField] private string defaultLanguage = "ru";

    private string currentLanguage;
    private Dictionary<string, string> russianTranslations;
    private Dictionary<string, string> englishTranslations;

    public event System.Action OnLanguageChanged;

    private static Dictionary<string, string> staticRussian = new Dictionary<string, string>();
    private static Dictionary<string, string> staticEnglish = new Dictionary<string, string>();
    private static bool staticInitialized = false;

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

        LoadAllTranslations();
        InitializeLanguage();
    }

    void InitializeLanguage()
    {
        string savedLang = PlayerPrefs.GetString("Language", "");
        
        if (!string.IsNullOrEmpty(savedLang))
        {
            SetLanguage(savedLang, save: false);
            return;
        }

        string systemLang = Application.systemLanguage.ToString().ToLower();
        if (systemLang.Contains("russian"))
        {
            SetLanguage("ru", save: true);
        }
        else
        {
            SetLanguage("en", save: true);
        }
    }

    public void SetLanguage(string languageCode, bool save = true)
    {
        currentLanguage = languageCode.ToLower();
        
        if (save)
        {
            PlayerPrefs.SetString("Language", currentLanguage);
        }
        
        OnLanguageChanged?.Invoke();
    }

    public void ToggleLanguage()
    {
        string newLang = currentLanguage == "ru" ? "en" : "ru";
        SetLanguage(newLang);
    }

    public string Get(string key)
    {
        return Get(key, key);
    }

    public string Get(string key, string defaultValue)
    {
        if (string.IsNullOrEmpty(key))
            return defaultValue;

        Dictionary<string, string> dict = currentLanguage == "ru" ? russianTranslations : englishTranslations;
        if (dict != null && dict.ContainsKey(key))
        {
            return dict[key];
        }

        return defaultValue;
    }

    public string GetFormatted(string key, params object[] args)
    {
        string format = Get(key);
        try
        {
            return string.Format(format, args);
        }
        catch
        {
            return format;
        }
    }

    public string GetCurrentLanguage() => currentLanguage;

    public void SetTranslation(string key, string russian, string english)
    {
        if (russianTranslations == null) russianTranslations = new Dictionary<string, string>();
        if (englishTranslations == null) englishTranslations = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(russian))
            russianTranslations[key] = russian;
        if (!string.IsNullOrEmpty(english))
            englishTranslations[key] = english;

        staticRussian[key] = russian;
        staticEnglish[key] = english;
    }

    public bool HasKey(string key)
    {
        Dictionary<string, string> dict = currentLanguage == "ru" ? russianTranslations : englishTranslations;
        return dict != null && dict.ContainsKey(key);
    }

    public List<string> GetAllKeys()
    {
        var keys = new List<string>();
        
        if (!staticInitialized)
        {
            LoadAllTranslations();
        }
        
        keys.AddRange(staticRussian.Keys);
        keys.Sort();
        return keys;
    }

    public string GetRussian(string key)
    {
        if (!staticInitialized)
        {
            LoadAllTranslations();
        }
        
        if (staticRussian.ContainsKey(key))
            return staticRussian[key];
        return key;
    }

    public string GetEnglish(string key)
    {
        if (!staticInitialized)
        {
            LoadAllTranslations();
        }
        
        if (staticEnglish.ContainsKey(key))
            return staticEnglish[key];
        return key;
    }

    public void RemoveKey(string key)
    {
        if (russianTranslations != null)
            russianTranslations.Remove(key);
        if (englishTranslations != null)
            englishTranslations.Remove(key);
        if (staticRussian.ContainsKey(key))
            staticRussian.Remove(key);
        if (staticEnglish.ContainsKey(key))
            staticEnglish.Remove(key);
    }

    // Static methods for Editor access
    public static void LoadStaticTranslations()
    {
        if (!staticInitialized)
        {
            LoadAllTranslationsStatic();
        }
    }

    private static void LoadAllTranslationsStatic()
    {
        if (staticInitialized && staticRussian.Count > 0) return;

        staticRussian = new Dictionary<string, string>
        {
            { "game_title", "WATERMELON FARM" },
            { "start_game", "НАЧАТЬ ИГРУ" },
            { "settings", "НАСТРОЙКИ" },
            { "quit", "ВЫЙТИ" },
            { "save", "СОХРАНИТЬ" },
            { "exit", "ВЫХОД" },
            { "back", "НАЗАД" },
            { "play", "ИГРАТЬ" },
            { "reset", "СБРОС" },
            { "rename", "ПЕРЕИМЕНОВАТЬ" },
            { "empty_save", "ПУСТОЕ СОХРАНЕНИЕ" },
            { "open_shop", "МАГАЗИН" },
            { "audio", "АУДИО" },
            { "music_volume", "ГРОМКОСТЬ МУЗЫКИ" },
            { "sfx_volume", "ГРОМКОСТЬ ЗВУКОВ" },
            { "slot", "СЛОТ" },
            { "slot_number", "Слот {0}" },
            { "save_name", "Имя сохранения" },
            { "save_date", "Дата" },
            { "new_save", "НОВОЕ СОХРАНЕНИЕ" },
            { "load_save", "ЗАГРУЗИТЬ" },
            { "delete_save", "УДАЛИТЬ" },
            { "grow_speed", "СКОРОСТЬ РОСТА" },
            { "harvest_value", "СТОИМОСТЬ УРОЖАЯ" },
            { "crit_harvest", "КРИТ. УРОЖАЙ" },
            { "fertilizer", "УДОБРЕНИЕ" },
            { "super_seeds", "СУПЕР СЕМЕНА" },
            { "level", "УРОВЕНЬ" },
            { "coins", "МОНЕТЫ" },
            { "max_level", "МАКС. УРОВЕНЬ" },
            { "unlock", "РАЗБЛОКИРОВАТЬ {0}" },
            { "all_unlocked", "ВСЕ РАЗБЛОКИРОВАНО" },
            { "slot_empty", "Пусто" },
            { "confirm", "ПОДТВЕРДИТЬ" },
            { "cancel", "ОТМЕНА" },
            { "paused", "ПАУЗА" },
            { "resume", "ПРОДОЛЖИТЬ" },
            { "main_menu", "ГЛАВНОЕ МЕНЮ" },
            { "versions", "ВЕРСИИ" },
            { "loading", "ЗАГРУЗКА..." },
            { "coins_label", "Монеты" }
        };

        staticEnglish = new Dictionary<string, string>
        {
            { "game_title", "WATERMELON FARM" },
            { "start_game", "START GAME" },
            { "settings", "SETTINGS" },
            { "quit", "QUIT" },
            { "save", "SAVE" },
            { "exit", "EXIT" },
            { "back", "BACK" },
            { "play", "PLAY" },
            { "reset", "RESET" },
            { "rename", "RENAME" },
            { "empty_save", "EMPTY SAVE" },
            { "open_shop", "SHOP" },
            { "audio", "AUDIO" },
            { "music_volume", "MUSIC VOLUME" },
            { "sfx_volume", "SFX VOLUME" },
            { "slot", "SLOT" },
            { "slot_number", "Slot {0}" },
            { "save_name", "Save name" },
            { "save_date", "Date" },
            { "new_save", "NEW SAVE" },
            { "load_save", "LOAD" },
            { "delete_save", "DELETE" },
            { "grow_speed", "GROWTH SPEED" },
            { "harvest_value", "HARVEST VALUE" },
            { "crit_harvest", "CRIT HARVEST" },
            { "fertilizer", "FERTILIZER" },
            { "super_seeds", "SUPER SEEDS" },
            { "level", "LEVEL" },
            { "coins", "COINS" },
            { "max_level", "MAX LEVEL" },
            { "unlock", "UNLOCK {0}" },
            { "all_unlocked", "ALL UNLOCKED" },
            { "slot_empty", "Empty" },
            { "confirm", "CONFIRM" },
            { "cancel", "CANCEL" },
            { "paused", "PAUSED" },
            { "resume", "RESUME" },
            { "main_menu", "MAIN MENU" },
            { "versions", "VERSIONS" },
            { "loading", "LOADING..." },
            { "coins_label", "Coins" }
        };

        staticInitialized = true;
    }

    public static List<string> GetStaticKeys()
    {
        LoadAllTranslationsStatic();
        var keys = new List<string>(staticRussian.Keys);
        keys.Sort();
        return keys;
    }

    public static string GetStaticRussian(string key)
    {
        LoadAllTranslationsStatic();
        if (staticRussian.ContainsKey(key))
            return staticRussian[key];
        return key;
    }

    public static string GetStaticEnglish(string key)
    {
        LoadAllTranslationsStatic();
        if (staticEnglish.ContainsKey(key))
            return staticEnglish[key];
        return key;
    }

    public static void SetStaticTranslation(string key, string russian, string english)
    {
        LoadAllTranslationsStatic();
        staticRussian[key] = russian;
        staticEnglish[key] = english;
    }

    void LoadAllTranslations()
    {
        if (staticInitialized && staticRussian.Count > 0)
        {
            russianTranslations = new Dictionary<string, string>(staticRussian);
            englishTranslations = new Dictionary<string, string>(staticEnglish);
            return;
        }

        LoadAllTranslationsStatic();

        russianTranslations = new Dictionary<string, string>(staticRussian);
        englishTranslations = new Dictionary<string, string>(staticEnglish);
    }
}
