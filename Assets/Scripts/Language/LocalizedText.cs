using TMPro;
using UnityEngine;

public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string localizationKey = "";
    [SerializeField] private string[] formatArgs;
    
    private TMP_Text tmpText;

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    void Start()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateText;
        }
        UpdateText();
    }

    void OnDestroy()
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
        if (tmpText == null) return;
        if (string.IsNullOrEmpty(localizationKey)) return;
        if (LocalizationManager.Instance == null) return;

        string value = LocalizationManager.Instance.Get(localizationKey);
        if (string.IsNullOrEmpty(value)) return;

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
