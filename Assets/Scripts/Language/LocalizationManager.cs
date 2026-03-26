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

        russianTranslations = new Dictionary<string, string>();
        englishTranslations = new Dictionary<string, string>();
        LoadTranslations();
    }

    void Start()
    {
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
    }

    public bool HasKey(string key)
    {
        Dictionary<string, string> dict = currentLanguage == "ru" ? russianTranslations : englishTranslations;
        return dict != null && dict.ContainsKey(key);
    }

    public List<string> GetAllKeys()
    {
        var keys = new List<string>();
        if (russianTranslations != null)
        {
            keys.AddRange(russianTranslations.Keys);
        }
        keys.Sort();
        return keys;
    }

    public string GetRussian(string key)
    {
        if (russianTranslations != null && russianTranslations.ContainsKey(key))
            return russianTranslations[key];
        return "";
    }

    public string GetEnglish(string key)
    {
        if (englishTranslations != null && englishTranslations.ContainsKey(key))
            return englishTranslations[key];
        return "";
    }

    public void RemoveKey(string key)
    {
        if (russianTranslations != null)
            russianTranslations.Remove(key);
        if (englishTranslations != null)
            englishTranslations.Remove(key);
    }

    void LoadTranslations()
    {
        russianTranslations = new Dictionary<string, string>
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
            { "tooltip_grow_speed_max", "Достигнут максимальный уровень" },
            { "tooltip_grow_speed", "Уровень {0}/{1}\nЭффект: +{2}%\nСледующий: +{3}%" },
            { "tooltip_harvest_value_max", "Достигнут максимальный уровень" },
            { "tooltip_harvest_value", "Уровень {0}/{1}\nЭффект: +{2}%\nСледующий: +{3}%" },
            { "tooltip_crit_chance_max", "Достигнут максимальный уровень" },
            { "tooltip_crit_chance", "Уровень {0}/{1}\nЭффект: +{2}%\nСледующий: +{3}%" },
            { "tooltip_fertilizer_max", "Достигнут максимальный уровень" },
            { "tooltip_fertilizer", "Уровень {0}/{1}\nЭффект: +{2}%\nСледующий: +{3}%" },
            { "tooltip_super_seed_max", "Достигнут максимальный уровень" },
            { "tooltip_super_seed", "Уровень {0}/{1}\nЭффект: +{2}%\nСледующий: +{3}%" },
            { "tooltip_all_unlocked", "Все арбузы разблокированы!" },
            { "tooltip_watermelon_unlock", "Разблокировать {0}\nСтоимость: {1} монет\nХар-ка: {2}" },
            { "tooltip_max_upgrade", "Достигнут максимальный уровень" },
            { "tooltip_not_enough_coins", "Недостаточно монет" },
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

        englishTranslations = new Dictionary<string, string>
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
            { "tooltip_grow_speed_max", "Max level reached" },
            { "tooltip_grow_speed", "Level {0}/{1}\nEffect: +{2}%\nNext: +{3}%" },
            { "tooltip_harvest_value_max", "Max level reached" },
            { "tooltip_harvest_value", "Level {0}/{1}\nEffect: +{2}%\nNext: +{3}%" },
            { "tooltip_crit_chance_max", "Max level reached" },
            { "tooltip_crit_chance", "Level {0}/{1}\nEffect: +{2}%\nNext: +{3}%" },
            { "tooltip_fertilizer_max", "Max level reached" },
            { "tooltip_fertilizer", "Level {0}/{1}\nEffect: +{2}%\nNext: +{3}%" },
            { "tooltip_super_seed_max", "Max level reached" },
            { "tooltip_super_seed", "Level {0}/{1}\nEffect: +{2}%\nNext: +{3}%" },
            { "tooltip_all_unlocked", "All watermelons unlocked!" },
            { "tooltip_watermelon_unlock", "Unlock {0}\nCost: {1} coins\nHarvest: {2}" },
            { "tooltip_max_upgrade", "Max level reached" },
            { "tooltip_not_enough_coins", "Not enough coins" },
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
    }
}
