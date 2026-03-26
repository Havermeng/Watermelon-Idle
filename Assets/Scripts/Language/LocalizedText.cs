using UnityEngine;
using TMPro;
using System;

[ExecuteInEditMode]
public class LocalizedText : MonoBehaviour
{
    [Header("Localization Key")]
    [SerializeField] private string localizationKey = "";
    
    [Header("Preview (Read Only)")]
    [SerializeField] private string previewRU = "";
    [SerializeField] private string previewEN = "";
    
    [Header("Format Arguments (optional)")]
    [SerializeField] private string[] formatArgs;
    
    private TMP_Text tmpText;
    private bool isInitialized = false;

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        if (tmpText == null)
        {
            Debug.LogWarning($"LocalizedText: No TMP_Text found on {gameObject.name}");
        }
    }

    void OnEnable()
    {
        Initialize();
        Subscribe();
        UpdateText();
        UpdatePreview();
    }

    void OnDisable()
    {
        Unsubscribe();
    }

    void OnDestroy()
    {
        Unsubscribe();
    }

    void OnValidate()
    {
        if (!isInitialized) return;
        
        if (Application.isPlaying)
        {
            UpdateText();
        }
        UpdatePreview();
    }

    void Initialize()
    {
        if (tmpText == null)
        {
            tmpText = GetComponent<TMP_Text>();
        }
        isInitialized = true;
    }

    void Subscribe()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateText;
        }
    }

    void Unsubscribe()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
        }
    }

    public void UpdateText()
    {
        if (tmpText == null) return;
        if (string.IsNullOrEmpty(localizationKey)) return;
        if (LocalizationManager.Instance == null) return;

        string value = LocalizationManager.Instance.Get(localizationKey);
        if (string.IsNullOrEmpty(value))
        {
            value = localizationKey;
        }

        if (formatArgs != null && formatArgs.Length > 0)
        {
            try
            {
                value = string.Format(value, (object[])formatArgs);
            }
            catch { }
        }

        tmpText.text = value;
    }

    void UpdatePreview()
    {
        if (LocalizationManager.Instance == null)
        {
            previewRU = "No Manager";
            previewEN = "No Manager";
            return;
        }

        previewRU = LocalizationManager.Instance.GetRussian(localizationKey);
        previewEN = LocalizationManager.Instance.GetEnglish(localizationKey);
        
        if (string.IsNullOrEmpty(previewRU))
            previewRU = localizationKey;
        if (string.IsNullOrEmpty(previewEN))
            previewEN = localizationKey;
    }

    public void SetKey(string key)
    {
        localizationKey = key;
        UpdateText();
        UpdatePreview();
    }

    public void SetFormatArgs(params string[] args)
    {
        formatArgs = args;
        UpdateText();
    }

    [ContextMenu("Update Now")]
    public void ForceUpdate()
    {
        Initialize();
        Subscribe();
        UpdateText();
        UpdatePreview();
    }

    [ContextMenu("Clear Key")]
    public void ClearKey()
    {
        localizationKey = "";
        previewRU = "";
        previewEN = "";
        if (tmpText != null)
        {
            tmpText.text = "";
        }
    }

    public string Key
    {
        get => localizationKey;
        set
        {
            localizationKey = value;
            UpdateText();
            UpdatePreview();
        }
    }

    public string CurrentText
    {
        get => tmpText != null ? tmpText.text : "";
    }
}
