using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Button ruButton;
    public Button enButton;

    void OnEnable()
    {
        if (AudioManager.Instance == null) return;
        musicSlider.value = AudioManager.Instance.GetMusicVolume();
        sfxSlider.value = AudioManager.Instance.GetSFXVolume();
        UpdateLanguageButtons();
    }

    void UpdateLanguageButtons()
    {
        if (LocalizationManager.Instance == null) return;
        
        string currentLang = LocalizationManager.Instance.GetCurrentLanguage();
        
        if (ruButton != null)
            ruButton.interactable = (currentLang != "ru");
        
        if (enButton != null)
            enButton.interactable = (currentLang != "en");
    }

    public void OnMusicSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }

    public void OnSFXSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
    }

    public void SetLanguageRU()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.SetLanguage("ru");
            UpdateLanguageButtons();
        }
    }

    public void SetLanguageEN()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.SetLanguage("en");
            UpdateLanguageButtons();
        }
    }

    public void OnBackClicked()
    {
        gameObject.SetActive(false);
    }
}
