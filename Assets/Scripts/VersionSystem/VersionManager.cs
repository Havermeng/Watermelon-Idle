using UnityEngine;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Менеджер версий игры. Управление версиями через скрипт.
/// Текст изменений загружается из текстового документа (TextAsset).
/// </summary>
public class VersionManager : MonoBehaviour
{
    public static VersionManager Instance;

    [Header("Version Settings")]
    [SerializeField] private string currentVersion = "0.1.0";
    
    [Header("Versions Source")]
    [SerializeField] private TextAsset versionsTextFile; // Текстовый файл с версиями
    [SerializeField] private bool loadFromFileOnStart = true;

    [Header("UI Display (Optional)")]
    [SerializeField] private TextMeshProUGUI currentVersionText;

    private List<GameVersion> versions = new List<GameVersion>();
    private const string PLAYER_PREFS_VERSION_KEY = "LastGameVersion";

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
        // Загружаем версии из файла если нужно
        if (loadFromFileOnStart && versionsTextFile != null)
        {
            LoadFromTextFile(versionsTextFile.text);
        }
        else
        {
            // Добавляем базовую версию если список пуст
            if (versions.Count == 0)
            {
                versions.Add(new GameVersion
                {
                    version = currentVersion,
                    changes = "- Текущая версия"
                });
            }
        }

        // Сортируем версии по убыванию (новые сначала)
        if (versions.Count > 0)
        {
            versions.Sort((a, b) => CompareVersions(b.version, a.version));
        }

        // Проверяем, есть ли текущая версия в списке
        if (!VersionExists(currentVersion))
        {
            Debug.LogWarning($"VersionManager: Current version '{currentVersion}' not found in versions list. Adding it automatically.");
            AddVersion(currentVersion, "- Текущая версия (добавлена автоматически)");
        }

        // Обновляем отображение текущей версии если есть TextMeshPro
        UpdateCurrentVersionText();

        // Проверяем обновление
        CheckVersionUpdate();
    }

    /// <summary>
    /// Загружает версии из текстового файла
    /// Формат: 
    /// версия: 0.1.0
    /// изменения: - Первая версия\n- Базовая функциональность
    /// ---
    /// версия: 0.1.1
    /// изменения: - Исправление багов\n- Новые возможности
    /// </summary>
    public void LoadFromTextFile(string text)
    {
        versions.Clear();
        
        string[] lines = text.Split('\n');
        GameVersion currentVersionData = null;
        
        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            
            // Пропускаем пустые строки и разделители
            if (string.IsNullOrEmpty(trimmed) || trimmed == "---")
                continue;
                
            // Проверяем, это строка с версией или изменениями
            if (trimmed.StartsWith("версия:", System.StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("version:", System.StringComparison.OrdinalIgnoreCase))
            {
                // Сохраняем предыдущую версию если есть
                if (currentVersionData != null)
                {
                    versions.Add(currentVersionData);
                }
                
                // Создаем новую версию
                currentVersionData = new GameVersion();
                string versionValue = trimmed.Substring(trimmed.IndexOf(':') + 1).Trim();
                currentVersionData.version = versionValue;
            }
            else if (trimmed.StartsWith("изменения:", System.StringComparison.OrdinalIgnoreCase) ||
                     trimmed.StartsWith("changes:", System.StringComparison.OrdinalIgnoreCase))
            {
                if (currentVersionData != null)
                {
                    string changesValue = trimmed.Substring(trimmed.IndexOf(':') + 1).Trim();
                    // Заменяем \n на реальные переносы строк
                    currentVersionData.changes = changesValue.Replace("\\n", "\n");
                }
            }
        }
        
        // Добавляем последнюю версию
        if (currentVersionData != null)
        {
            versions.Add(currentVersionData);
        }
        
        Debug.Log($"VersionManager: Loaded {versions.Count} versions from text file");
    }

    /// <summary>
    /// Добавляет новую версию в список
    /// </summary>
    public void AddVersion(string version, string changes)
    {
        if (string.IsNullOrEmpty(version))
        {
            Debug.LogWarning("VersionManager: Cannot add version with empty version number.");
            return;
        }
        
        GameVersion newVersion = new GameVersion
        {
            version = version,
            changes = changes ?? ""
        };
        
        versions.Add(newVersion);
        versions.Sort((a, b) => CompareVersions(b.version, a.version));
        
        Debug.Log($"VersionManager: Added version {version}");
        OnVersionChanged?.Invoke();
    }

    /// <summary>
    /// Удаляет версию из списка
    /// </summary>
    public bool RemoveVersion(string version)
    {
        if (string.IsNullOrEmpty(version))
            return false;
            
        for (int i = 0; i < versions.Count; i++)
        {
            if (versions[i].version == version)
            {
                versions.RemoveAt(i);
                Debug.Log($"VersionManager: Removed version {version}");
                OnVersionChanged?.Invoke();
                return true;
            }
        }
        
        Debug.LogWarning($"VersionManager: Version {version} not found for removal.");
        return false;
    }

    /// <summary>
    /// Очищает все версии
    /// </summary>
    public void ClearVersions()
    {
        versions.Clear();
        Debug.Log("VersionManager: Cleared all versions");
        OnVersionChanged?.Invoke();
    }

    /// <summary>
    /// Обновляет текст с текущей версией (если назначен)
    /// </summary>
    void UpdateCurrentVersionText()
    {
        if (currentVersionText != null)
        {
            currentVersionText.text = $"Версия: {currentVersion}";
        }
    }

    /// <summary>
    /// Проверяет и сохраняет версию при первом запуске
    /// </summary>
    void CheckVersionUpdate()
    {
        if (IsNewVersion())
        {
            Debug.Log($"VersionManager: New version detected! '{currentVersion}' (saved: '{PlayerPrefs.GetString(PLAYER_PREFS_VERSION_KEY, "")}')");
            MarkVersionAsSeen();
        }
    }

    /// <summary>
    /// Возвращает список всех версий
    /// </summary>
    public List<GameVersion> GetVersions()
    {
        return new List<GameVersion>(versions); // Возвращаем копию
    }

    /// <summary>
    /// Возвращает текущую версию игры
    /// </summary>
    public string GetCurrentVersion()
    {
        return currentVersion;
    }

    /// <summary>
    /// Устанавливает текущую версию игры (для изменения через скрипт)
    /// </summary>
    public void SetCurrentVersion(string newVersion)
    {
        if (string.IsNullOrEmpty(newVersion))
        {
            Debug.LogWarning("VersionManager: SetCurrentVersion called with empty or null version string.");
            return;
        }

        string oldVersion = currentVersion;
        currentVersion = newVersion;
        
        Debug.Log($"VersionManager: Current version changed from '{oldVersion}' to '{currentVersion}'");

        // Проверяем, есть ли новая версия в списке
        if (!VersionExists(currentVersion))
        {
            Debug.LogWarning($"VersionManager: Current version '{currentVersion}' not found in versions list. Adding it automatically.");
            AddVersion(currentVersion, "- Текущая версия (добавлена автоматически)");
        }

        // Обновляем отображение
        UpdateCurrentVersionText();

        // Уведомляем подписчиков
        OnVersionChanged?.Invoke();
    }

    /// <summary>
    /// Событие, вызываемое при изменении версии или списка версий
    /// </summary>
    public event System.Action OnVersionChanged;

    /// <summary>
    /// Проверяет, является ли текущая версия новой (отличается от сохраненной)
    /// </summary>
    public bool IsNewVersion()
    {
        string savedVersion = PlayerPrefs.GetString(PLAYER_PREFS_VERSION_KEY, "");
        return savedVersion != currentVersion;
    }

    /// <summary>
    /// Сохраняет текущую версию как просмотренную
    /// </summary>
    public void MarkVersionAsSeen()
    {
        PlayerPrefs.SetString(PLAYER_PREFS_VERSION_KEY, currentVersion);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Проверяет существование версии в списке
    /// </summary>
    bool VersionExists(string version)
    {
        if (versions == null) return false;
        
        foreach (var v in versions)
        {
            if (v.version == version)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Сравнивает две версии (семантическое версионирование)
    /// </summary>
    public int CompareVersions(string a, string b)
    {
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
            return 0;

        string[] aParts = a.Split('.');
        string[] bParts = b.Split('.');

        int maxParts = Mathf.Max(aParts.Length, bParts.Length);

        for (int i = 0; i < maxParts; i++)
        {
            int aPart = i < aParts.Length ? int.Parse(aParts[i]) : 0;
            int bPart = i < bParts.Length ? int.Parse(bParts[i]) : 0;

            if (aPart != bPart)
                return bPart.CompareTo(aPart); // Обратный порядок для сортировки по убыванию
        }

        return 0;
    }
}