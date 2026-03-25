using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Автоматически привязывает кнопки главного меню к функциям MenuManager
/// </summary>
public class MenuButtonBinder : MonoBehaviour
{
    [Header("Сопоставление кнопок с функциями")]
    public Button startGameButton;      // Кнопка "Начать игру" -> OpenSaveSlots
    public Button settingsButton;       // Кнопка "Настройки" -> OpenSettings
    public Button quitButton;           // Кнопка "Выход" -> QuitGame

    void Start()
    {
        // Работаем только в WebGL сборке
#if UNITY_WEBGL && !UNITY_EDITOR
        BindButtons();
#endif
    }

    void BindButtons()
    {
        if (MenuManager.Instance == null)
        {
            Debug.LogError("MenuButtonBinder: MenuManager.Instance не найден!");
            return;
        }

        // Привязываем кнопку "Начать игру"
        if (startGameButton != null)
        {
            startGameButton.onClick.RemoveAllListeners();
            startGameButton.onClick.AddListener(() => 
            {
                Debug.Log("MenuButtonBinder: Нажата кнопка 'Начать игру'");
                MenuManager.Instance.OpenSaveSlots();
            });
            Debug.Log("MenuButtonBinder: StartGameButton привязана к OpenSaveSlots");
        }
        else
        {
            // Пытаемся найти кнопку по имени
            Button foundBtn = FindButtonByName("StartGameButton");
            if (foundBtn != null)
            {
                foundBtn.onClick.RemoveAllListeners();
                foundBtn.onClick.AddListener(() => 
                {
                    Debug.Log("MenuButtonBinder: Нажата кнопка 'Начать игру'");
                    MenuManager.Instance.OpenSaveSlots();
                });
                Debug.Log("MenuButtonBinder: StartGameButton найдена и привязана");
            }
        }

        // Привязываем кнопку "Настройки"
        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(() => 
            {
                Debug.Log("MenuButtonBinder: Нажата кнопка 'Настройки'");
                MenuManager.Instance.OpenSettings();
            });
            Debug.Log("MenuButtonBinder: SettingsButton привязана к OpenSettings");
        }
        else
        {
            // Пытаемся найти кнопку по имени
            Button foundBtn = FindButtonByName("SettingsButton");
            if (foundBtn != null)
            {
                foundBtn.onClick.RemoveAllListeners();
                foundBtn.onClick.AddListener(() => 
                {
                    Debug.Log("MenuButtonBinder: Нажата кнопка 'Настройки'");
                    MenuManager.Instance.OpenSettings();
                });
                Debug.Log("MenuButtonBinder: SettingsButton найдена и привязана");
            }
        }

        // Привязываем кнопку "Выход"
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(() => 
            {
                Debug.Log("MenuButtonBinder: Нажата кнопка 'Выход'");
                MenuManager.Instance.QuitGame();
            });
            Debug.Log("MenuButtonBinder: QuitButton привязана к QuitGame");
        }
        else
        {
            // Пытаемся найти кнопку по имени
            Button foundBtn = FindButtonByName("QuitButton");
            if (foundBtn != null)
            {
                foundBtn.onClick.RemoveAllListeners();
                foundBtn.onClick.AddListener(() => 
                {
                    Debug.Log("MenuButtonBinder: Нажата кнопка 'Выход'");
                    MenuManager.Instance.QuitGame();
                });
                Debug.Log("MenuButtonBinder: QuitButton найдена и привязана");
            }
        }

        Debug.Log("MenuButtonBinder: Привязка кнопок завершена");
    }

    Button FindButtonByName(string buttonName)
    {
        // Ищем кнопку в UIContainer (созданном WebGLUIFixer)
        GameObject container = GameObject.Find("UIContainer");
        if (container != null)
        {
            Transform btnTransform = container.transform.Find(buttonName);
            if (btnTransform != null)
            {
                Button btn = btnTransform.GetComponent<Button>();
                if (btn != null)
                {
                    return btn;
                }
            }
        }

        // Ищем во всей сцене
        Button[] allButtons = FindObjectsOfType<Button>(true);
        foreach (Button btn in allButtons)
        {
            if (btn.gameObject.name == buttonName)
            {
                return btn;
            }
        }

        Debug.LogWarning($"MenuButtonBinder: Кнопка '{buttonName}' не найдена");
        return null;
    }
}