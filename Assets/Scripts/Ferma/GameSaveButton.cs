using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSaveButton : MonoBehaviour
{
    public void OnSaveClicked()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveToSlot(SaveManager.Instance.selectedSlot);
            Debug.Log("Сохранено в слот " + SaveManager.Instance.selectedSlot);
        }
        else
        {
            Debug.LogWarning("SaveManager не найден. Сохранение невозможно.");
        }
    }

    public void OnExitClicked()
    {
        Time.timeScale = 1f;
        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveToSlot(SaveManager.Instance.selectedSlot);
        
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadMainMenu()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MainMenu");
    }
}