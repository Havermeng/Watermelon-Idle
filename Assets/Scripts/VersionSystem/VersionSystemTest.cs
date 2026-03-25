using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Тестовый скрипт для проверки системы версий.
/// Прикрепите к любому GameObject в сцене MainMenu для тестирования.
/// </summary>
public class VersionSystemTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== VersionSystemTest Started ===");

        // Проверяем VersionManager
        if (VersionManager.Instance == null)
        {
            Debug.LogError("VersionManager.Instance is NULL! Make sure VersionManager prefab is in the scene.");
        }
        else
        {
            Debug.Log($"VersionManager found. Current version: {VersionManager.Instance.GetCurrentVersion()}");
            
            List<GameVersion> versions = VersionManager.Instance.GetVersions();
            Debug.Log($"Total versions loaded: {versions.Count}");
            
            foreach (var v in versions)
            {
                Debug.Log($"  - v{v.version}: {v.changes.Replace("\n", " ")}");
            }
        }

        // Проверяем VersionUIManager
        if (VersionUIManager.Instance == null)
        {
            Debug.LogError("VersionUIManager.Instance is NULL! Make sure VersionUIManager prefab is in the scene.");
        }
        else
        {
            Debug.Log("VersionUIManager found and initialized.");
        }

        // Проверяем VersionNotifier
        if (VersionNotifier.Instance == null)
        {
            Debug.LogError("VersionNotifier.Instance is NULL! Make sure VersionNotifier prefab is in the scene.");
        }
        else
        {
            Debug.Log("VersionNotifier found and initialized.");
            
            // Проверяем, новая ли версия
            bool isNew = VersionNotifier.Instance.gameObject.activeSelf ? true : false;
            Debug.Log($"Is new version: {VersionManager.Instance.IsNewVersion()}");
        }

        Debug.Log("=== VersionSystemTest Complete ===");
    }

    /// <summary>
    /// Ручная проверка - показать панель версий
    /// </summary>
    public void TestShowVersionPanel()
    {
        Debug.Log("Test: Opening version panel...");
        if (VersionUIManager.Instance != null)
        {
            VersionUIManager.Instance.ShowVersionPanel();
        }
    }

    /// <summary>
    /// Ручная проверка - показать уведомление
    /// </summary>
    public void TestShowNotification()
    {
        Debug.Log("Test: Showing notification...");
        if (VersionNotifier.Instance != null)
        {
            VersionNotifier.Instance.ShowUpdateNotification();
        }
    }

    /// <summary>
    /// Ручная проверка - проверить обновление версии
    /// </summary>
    public void TestCheckVersion()
    {
        Debug.Log("Test: Checking version...");
        if (VersionManager.Instance != null)
        {
            bool isNew = VersionManager.Instance.IsNewVersion();
            Debug.Log($"Is new version: {isNew}");
            
            if (isNew)
            {
                Debug.Log("New version detected! Would show notification.");
            }
            else
            {
                Debug.Log("No new version. Current version is up to date.");
            }
        }
    }

    /// <summary>
    /// Ручная проверка - сбросить сохраненную версию (для тестирования показа уведомления)
    /// </summary>
    public void TestResetSavedVersion()
    {
        Debug.Log("Test: Resetting saved version...");
        PlayerPrefs.DeleteKey("LastGameVersion");
        PlayerPrefs.Save();
        Debug.Log("Saved version cleared. Next check will show notification if version is new.");
    }
}