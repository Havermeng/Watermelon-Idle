using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string localizationKey = "";
    [SerializeField] private string[] formatArgs;
    
    private TMP_Text tmpText;
    private Text legacyText;

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        if (tmpText == null)
        {
            legacyText = GetComponent<Text>();
        }
    }

    void OnEnable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateText;
        }
        UpdateText();
    }

    void OnDisable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
        }
    }

    public void SetKey(string key)
    {
        localizationKey = key;
        UpdateText();
    }

    public void SetFormatArgs(params string[] args)
    {
        formatArgs = args;
        UpdateText();
    }

    private void UpdateText()
    {
        if (string.IsNullOrEmpty(localizationKey))
        {
            Debug.Log($"[LocalizedText] No key set on {gameObject.name}");
            return;
        }
        if (LocalizationManager.Instance == null)
        {
            Debug.LogWarning($"[LocalizedText] LocalizationManager.Instance is null!");
            return;
        }

        string value = LocalizationManager.Instance.Get(localizationKey);
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogWarning($"[LocalizedText] No translation for key: {localizationKey}");
            return;
        }

        if (formatArgs != null && formatArgs.Length > 0)
        {
            try
            {
                value = string.Format(value, (object[])formatArgs);
            }
            catch { }
        }

        if (tmpText != null)
        {
            tmpText.text = value;
            Debug.Log($"[LocalizedText] Updated '{localizationKey}' to '{value}'");
        }
        else if (legacyText != null)
        {
            legacyText.text = value;
            Debug.Log($"[LocalizedText] Updated '{localizationKey}' to '{value}'");
        }
    }

    public string Key
    {
        get => localizationKey;
        set
        {
            localizationKey = value;
            UpdateText();
        }
    }
}
