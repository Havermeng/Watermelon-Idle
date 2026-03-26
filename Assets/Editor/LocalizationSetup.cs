using UnityEngine;
using UnityEditor;
using TMPro;

public class LocalizationSetup : EditorWindow
{
    [MenuItem("Tools/Localization/Setup All Text")]
    public static void SetupAllText()
    {
        SetupLocalizedTexts();
        SetupLanguageButtons();
        Debug.Log("Localization setup complete!");
    }

    [MenuItem("Tools/Localization/Setup Text in Scene")]
    public static void SetupSceneText()
    {
        SetupLocalizedTexts();
        Debug.Log("Scene text setup complete!");
    }

    [MenuItem("Tools/Localization/Connect Language Buttons")]
    public static void SetupButtons()
    {
        SetupLanguageButtons();
        Debug.Log("Language buttons connected!");
    }

    static void SetupLocalizedTexts()
    {
        TMP_Text[] texts = FindObjectsOfType<TMP_Text>(true);
        int count = 0;

        foreach (TMP_Text text in texts)
        {
            if (text.GetComponent<LocalizedText>() != null)
                continue;

            string key = FindKeyForText(text.text);
            if (!string.IsNullOrEmpty(key))
            {
                LocalizedText lt = text.gameObject.AddComponent<LocalizedText>();
                lt.SetKey(key);
                count++;
                Debug.Log($"Added: {key} to {text.gameObject.name}");
            }
        }

        Debug.Log($"Total texts localized: {count}");
    }

    static void SetupLanguageButtons()
    {
        SettingsUI[] settings = FindObjectsOfType<SettingsUI>(true);

        if (settings.Length == 0)
        {
            Debug.LogWarning("SettingsUI not found in scene!");
            return;
        }

        foreach (SettingsUI ui in settings)
        {
            Button[] allButtons = FindObjectsOfType<Button>(true);

            foreach (Button btn in allButtons)
            {
                string name = btn.gameObject.name.ToLower();

                if (name.Contains("ru") && ui.ruButton == null)
                {
                    Selection.activeGameObject = btn.gameObject;
                    Debug.Log($"Select RU button: {btn.gameObject.name}");
                }
                else if (name.Contains("en") && ui.enButton == null)
                {
                    Selection.activeGameObject = btn.gameObject;
                    Debug.Log($"Select EN button: {btn.gameObject.name}");
                }
            }

            Debug.Log($"SettingsUI found. Assign ruButton and enButton in Inspector.");
        }
    }

    static string FindKeyForText(string text)
    {
        if (string.IsNullOrEmpty(text)) return null;

        text = text.Trim().ToUpper().Replace(" ", "_").Replace("\n", "_");

        if (text.Contains("WATERMELON") || text.Contains("АРБУЗ"))
            return "game_title";

        string[,] mappings = new string[,]
        {
            { "НАЧАТЬ", "ИГРУ", "start_game" },
            { "START", "GAME", "start_game" },
            { "НАСТРОЙКИ", "SETTINGS", "settings" },
            { "ВЫЙТИ", "QUIT", "quit" },
            { "СОХРАНИТЬ", "SAVE", "save" },
            { "ВЫХОД", "EXIT", "exit" },
            { "НАЗАД", "BACK", "back" },
            { "ИГРАТЬ", "PLAY", "play" },
            { "СБРОС", "RESET", "reset" },
            { "МАГАЗИН", "SHOP", "open_shop" },
            { "ВЕРСИИ", "VERSIONS", "versions" },
            { "ПАУЗА", "PAUSED", "paused" },
            { "ПРОДОЛЖИТЬ", "RESUME", "resume" },
            { "ГЛАВНОЕ", "МЕНЮ", "main_menu" },
            { "MAIN", "MENU", "main_menu" },
            { "ПОДТВЕРДИТЬ", "CONFIRM", "confirm" },
            { "ОТМЕНА", "CANCEL", "cancel" },
            { "УДАЛИТЬ", "DELETE", "delete_save" },
            { "ЗАГРУЗИТЬ", "LOAD", "load_save" },
            { "ПЕРЕИМЕНОВАТЬ", "RENAME", "rename" },
            { "АУДИО", "AUDIO", "audio" },
            { "МУЗЫКИ", "MUSIC", "music_volume" },
            { "ЗВУКОВ", "SFX", "sfx_volume" },
            { "СКОРОСТЬ", "РОСТА", "grow_speed" },
            { "GROWTH", "SPEED", "grow_speed" },
            { "СТОИМОСТЬ", "УРОЖАЯ", "harvest_value" },
            { "HARVEST", "VALUE", "harvest_value" },
            { "КРИТ", "CRIT", "crit_harvest" },
            { "УДОБРЕНИЕ", "FERTILIZER", "fertilizer" },
            { "СУПЕР", "SEМЕНА", "super_seeds" },
            { "SUPER", "SEEDS", "super_seeds" },
            { "МОНЕТЫ", "COINS", "coins" },
        };

        for (int i = 0; i < mappings.GetLength(0); i++)
        {
            string ru = mappings[i, 0];
            string en = mappings[i, 1];
            string key = mappings[i, 2];

            if (text.Contains(ru) || text.Contains(en))
                return key;
        }

        if (text.Contains("ПУСТО") || text.Contains("EMPTY"))
            return "slot_empty";
        if (text.Contains("СЛОТ") || text.Contains("SLOT"))
            return "slot";

        return null;
    }
}
