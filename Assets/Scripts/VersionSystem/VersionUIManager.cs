using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Менеджер UI для системы версий. Управляет отображением панели версий.
/// Не создает объекты автоматически - только управляет существующей панелью.
/// </summary>
public class VersionUIManager : MonoBehaviour
{
    public static VersionUIManager Instance;

    private GameObject versionPanel;

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

    IEnumerator Start()
    {
        // Ждем пока загрузится Canvas
        yield return new WaitForEndOfFrame();
        
        // Находим панель если она уже существует
        if (versionPanel == null)
        {
            versionPanel = GameObject.Find("VersionPanel");
        }
    }

    void OnEnable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateVersionButtonText;
        }
    }

    void OnDisable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateVersionButtonText;
        }
    }

    /// <summary>
    /// Показывает панель версий
    /// </summary>
    public void ShowVersionPanel()
    {
        if (versionPanel == null)
        {
            versionPanel = GameObject.Find("VersionPanel");
            if (versionPanel == null)
            {
                Debug.LogWarning("VersionUIManager: VersionPanel not found. Create a GameObject named 'VersionPanel' with VersionPanelUI component.");
                return;
            }
        }

        VersionPanelUI panelUI = versionPanel.GetComponent<VersionPanelUI>();
        if (panelUI != null)
        {
            panelUI.OpenPanel();
        }
        else
        {
            versionPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Скрывает панель версий
    /// </summary>
    public void HideVersionPanel()
    {
        if (versionPanel == null)
        {
            versionPanel = GameObject.Find("VersionPanel");
            if (versionPanel == null)
            {
                return;
            }
        }

        VersionPanelUI panelUI = versionPanel.GetComponent<VersionPanelUI>();
        if (panelUI != null)
        {
            panelUI.ClosePanel();
        }
        else
        {
            versionPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Обновляет текст кнопки (если кнопка создана вручную)
    /// </summary>
    void UpdateVersionButtonText()
    {
        // Этот метод вызывается при смене языка, но кнопка должна обновляться сама
        // Или можно найти кнопку и обновить её текст
    }
}