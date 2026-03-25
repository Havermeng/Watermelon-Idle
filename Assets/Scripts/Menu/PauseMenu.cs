using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;

    private bool isPaused = false;

    private void Start()
    {
        pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuPanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveToSlot(SaveManager.Instance.selectedSlot);
        }
        else
        {
            Debug.LogWarning("SaveManager не найден. Сохранение невозможно.");
        }
        SceneManager.LoadScene("MainMenu");
    }

    public void SaveGame()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveToSlot(SaveManager.Instance.selectedSlot);
        }
        else
        {
            Debug.LogWarning("SaveManager не найден. Сохранение невозможно.");
        }
        TogglePause();
    }

    public void OpenSettings()
    {
        pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }
}