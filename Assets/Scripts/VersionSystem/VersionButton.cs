using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Компонент для кнопки открытия панели версий.
/// Назначается на кнопку в UI.
/// </summary>
public class VersionButton : MonoBehaviour
{
    [Header("References")]
    public GameObject versionPanel;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        
        if (button == null)
        {
            Debug.LogError("VersionButton: Button component not found on this GameObject!");
            return;
        }

        // Настраиваем кнопку если еще не настроена
        if (button.onClick.GetPersistentEventCount() == 0)
        {
            button.onClick.AddListener(OpenVersionPanel);
        }
    }

    /// <summary>
    /// Открывает панель версий
    /// </summary>
    public void OpenVersionPanel()
    {
        if (versionPanel != null)
        {
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
        else
        {
            Debug.LogWarning("VersionButton: versionPanel reference is not set");
            
            // Пытаемся найти панель автоматически
            GameObject foundPanel = GameObject.Find("VersionPanel");
            if (foundPanel != null)
            {
                VersionPanelUI panelUI = foundPanel.GetComponent<VersionPanelUI>();
                if (panelUI != null)
                {
                    panelUI.OpenPanel();
                }
                else
                {
                    foundPanel.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Закрывает панель версий
    /// </summary>
    public void CloseVersionPanel()
    {
        if (versionPanel != null)
        {
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
    }

    /// <summary>
    /// Устанавливает ссылку на панель версий
    /// </summary>
    public void SetVersionPanel(GameObject panel)
    {
        versionPanel = panel;
    }
}