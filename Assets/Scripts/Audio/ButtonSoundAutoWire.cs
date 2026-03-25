using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonSoundAutoWire : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        WireButtons();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        WireButtons();
    }

    void WireButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>(true);
        foreach (Button btn in buttons)
        {
            btn.onClick.RemoveListener(PlayButtonSound);
            btn.onClick.AddListener(PlayButtonSound);
        }
    }

    void PlayButtonSound()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.buttonSound);
    }
}
