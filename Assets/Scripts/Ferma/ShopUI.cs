using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;

    public static bool isShopOpen = false;

    public GameObject shopPanel;
    public GameObject openShopButton;
    private TextMeshProUGUI growSpeedText;
    private TextMeshProUGUI harvestValueText;
    private TextMeshProUGUI critChanceText;
    private TextMeshProUGUI fertilizerText;
    private TextMeshProUGUI superSeedText;
    private TextMeshProUGUI watermelonUnlockText;

    public GameObject tooltipPanel; // теперь это временное сообщение
    public TextMeshProUGUI tooltipText;
    public TextMeshProUGUI errorMessageText;

    private Canvas mainCanvas;

    private bool canvasFound = false;

    void Awake()
    {
        Instance = this;
        
        // Создаем LocalizationManager если его нет
        if (LocalizationManager.Instance == null)
        {
            GameObject locObj = new GameObject("LocalizationManager");
            locObj.AddComponent<LocalizationManager>();
        }
    }

    void Start()
    {
        // Кэшируем Canvas один раз при старте
        if (!canvasFound)
        {
            mainCanvas = FindValidCanvas();
            canvasFound = mainCanvas != null;
        }
        
        InitializeUI();
        CacheUIElements();
        if (shopPanel != null) shopPanel.SetActive(false);
    }

    void CacheUIElements()
    {
        if (shopPanel != null)
        {
            Image panelImage = shopPanel.GetComponent<Image>();
            if (panelImage != null)
                panelImage.raycastTarget = true;
        }

        Transform[] allChildren = shopPanel != null 
            ? shopPanel.GetComponentsInChildren<Transform>(true)
            : GetComponentsInChildren<Transform>(true);

        foreach (Transform child in allChildren)
        {
            string name = child.name;
            if (growSpeedText == null && name.Contains("GrowSpeed") && name.Contains("Text"))
                growSpeedText = child.GetComponent<TextMeshProUGUI>();
            else if (harvestValueText == null && name.Contains("HarvestValue") && name.Contains("Text"))
                harvestValueText = child.GetComponent<TextMeshProUGUI>();
            else if (critChanceText == null && name.Contains("CritChance") && name.Contains("Text"))
                critChanceText = child.GetComponent<TextMeshProUGUI>();
            else if (fertilizerText == null && name.Contains("Fertilizer") && name.Contains("Text"))
                fertilizerText = child.GetComponent<TextMeshProUGUI>();
            else if (superSeedText == null && name.Contains("SuperSeed") && name.Contains("Text"))
                superSeedText = child.GetComponent<TextMeshProUGUI>();
            else if (watermelonUnlockText == null && name.Contains("WatermelonUnlock") && name.Contains("Text"))
                watermelonUnlockText = child.GetComponent<TextMeshProUGUI>();
        }
    }

    void InitializeUI()
    {
        InitializeTooltip();
        InitializeErrorMessage();
    }

    void InitializeTooltip()
    {
        if (mainCanvas == null)
        {
            mainCanvas = FindValidCanvas();
            if (mainCanvas == null)
            {
                Debug.LogError("Canvas не найден для создания подсказки");
                return;
            }
        }
        
        if (tooltipPanel == null)
        {
            tooltipPanel = new GameObject("TooltipPanel");
            tooltipPanel.SetActive(false);
            
            tooltipPanel.transform.SetParent(mainCanvas.transform, false);
            
            RectTransform rt = tooltipPanel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(480, 140);
            rt.anchoredPosition = new Vector2(0, -40);
            
            Image bg = tooltipPanel.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            bg.raycastTarget = false;
        }
        
        if (tooltipText == null)
        {
            GameObject textObj = new GameObject("TooltipText");
            textObj.transform.SetParent(tooltipPanel.transform, false);
            
            tooltipText = textObj.AddComponent<TextMeshProUGUI>();
            tooltipText.alignment = TextAlignmentOptions.Center;
            tooltipText.fontSize = 28;
            tooltipText.color = Color.white;
            tooltipText.textWrappingMode = TextWrappingModes.Normal;
            tooltipText.richText = true;
            
            RectTransform rt = tooltipText.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(15, 15);
            rt.offsetMax = new Vector2(-5, -5);
        }
        
        AssignDefaultFont(tooltipText);
    }
    
    Canvas FindValidCanvas()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null && canvas.isActiveAndEnabled)
            return canvas;
        
        Canvas[] allCanvases = GameObject.FindObjectsOfType<Canvas>();
        foreach (Canvas c in allCanvases)
        {
            if (c.isActiveAndEnabled)
                return c;
        }
        
        GameObject canvasObj = GameObject.FindGameObjectWithTag("Canvas");
        if (canvasObj != null)
        {
            Canvas c = canvasObj.GetComponent<Canvas>();
            if (c != null) return c;
        }
        
        GameObject mainCanvasObj = GameObject.Find("Canvas");
        if (mainCanvasObj != null)
        {
            Canvas c = mainCanvasObj.GetComponent<Canvas>();
            if (c != null) return c;
        }
        
        Debug.LogError("Не удалось найти активный Canvas в сцене");
        return null;
    }

    void AssignDefaultFont(TextMeshProUGUI tmp)
    {
        if (tmp.font != null) return;
        
        if (TMPro.TMP_Settings.defaultFontAsset != null)
        {
            tmp.font = TMPro.TMP_Settings.defaultFontAsset;
            return;
        }
        
        TMPro.TMP_FontAsset[] fonts = Resources.FindObjectsOfTypeAll<TMPro.TMP_FontAsset>();
        if (fonts.Length > 0)
        {
            tmp.font = fonts[0];
        }
        else
        {
            Debug.LogError("Не найден ни один TMP_FontAsset в проекте!");
        }
    }

    void InitializeErrorMessage()
    {
        if (errorMessageText != null)
            return;
            
        Transform parent = null;
        if (shopPanel != null)
            parent = shopPanel.transform;
        else if (mainCanvas != null)
            parent = mainCanvas.transform;
        
        if (parent == null)
        {
            Debug.LogError("Не найден родитель для errorMessage");
            return;
        }
        
        GameObject errorObj = new GameObject("ErrorMessage");
        errorObj.transform.SetParent(parent, false);
        
        errorMessageText = errorObj.AddComponent<TextMeshProUGUI>();
        errorMessageText.alignment = TextAlignmentOptions.Center;
        errorMessageText.fontSize = 24;
        errorMessageText.color = new Color(1, 0.3f, 0.3f);
        errorMessageText.text = "";
        
        RectTransform rt = errorMessageText.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300, 50);
        rt.anchoredPosition = new Vector2(0, -100);
    }

    void ShowErrorMessage(string message)
    {
        if (errorMessageText != null)
        {
            errorMessageText.text = message;
            errorMessageText.gameObject.SetActive(true);
            StartCoroutine(ClearErrorMessage());
        }
    }

    System.Collections.IEnumerator ClearErrorMessage()
    {
        yield return new WaitForSeconds(2f);
        if (errorMessageText != null)
            errorMessageText.gameObject.SetActive(false);
    }

    void ShowTooltipMessage(string message)
    {
        if (tooltipPanel == null || tooltipText == null) return;
        
        tooltipText.text = message;
        tooltipPanel.SetActive(true);
    }

    void HideTooltip()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    public static bool IsOpen =>
        Instance != null &&
        Instance.shopPanel != null &&
        Instance.shopPanel.activeSelf;



    void SetupButtons()
    {
        if (shopPanel != null)
        {
            Button[] allButtons = shopPanel.GetComponentsInChildren<Button>(true);
            
            foreach (Button btn in allButtons)
            {
                string name = btn.gameObject.name;
                if (name.Contains("GrowSpeed"))
                {
                    SetupButtonWithTooltip(btn.gameObject, BuyGrowSpeed, GenerateGrowSpeedTooltip());
                }
                else if (name.Contains("HarvestValue"))
                {
                    SetupButtonWithTooltip(btn.gameObject, BuyHarvestValue, GenerateHarvestValueTooltip());
                }
                else if (name.Contains("Close"))
                {
                    btn.onClick.AddListener(CloseShop);
                }
                else if (name.Contains("CritChance"))
                {
                    SetupButtonWithTooltip(btn.gameObject, BuyCritChance, GenerateCritChanceTooltip());
                }
                else if (name.Contains("Fertilizer"))
                {
                    SetupButtonWithTooltip(btn.gameObject, BuyFertilizer, GenerateFertilizerTooltip());
                }
                else if (name.Contains("SuperSeed"))
                {
                    SetupButtonWithTooltip(btn.gameObject, BuySuperSeed, GenerateSuperSeedTooltip());
                }
                else if (name.Contains("WatermelonUnlock"))
                {
                    SetupButtonWithTooltip(btn.gameObject, BuyWatermelonUnlock, GenerateWatermelonUnlockTooltip());
                }
            }
        }
        else
        {
            Button[] allButtons = GameObject.FindObjectsOfType<Button>(true);
            
            foreach (Button btn in allButtons)
            {
                string name = btn.gameObject.name;
                if (name.Contains("GrowSpeed"))
                    SetupButtonWithTooltip(btn.gameObject, BuyGrowSpeed, GenerateGrowSpeedTooltip());
                else if (name.Contains("HarvestValue"))
                    SetupButtonWithTooltip(btn.gameObject, BuyHarvestValue, GenerateHarvestValueTooltip());
                else if (name.Contains("Close"))
                    btn.onClick.AddListener(CloseShop);
                else if (name.Contains("CritChance"))
                    SetupButtonWithTooltip(btn.gameObject, BuyCritChance, GenerateCritChanceTooltip());
                else if (name.Contains("Fertilizer"))
                    SetupButtonWithTooltip(btn.gameObject, BuyFertilizer, GenerateFertilizerTooltip());
                else if (name.Contains("SuperSeed"))
                    SetupButtonWithTooltip(btn.gameObject, BuySuperSeed, GenerateSuperSeedTooltip());
                else if (name.Contains("WatermelonUnlock"))
                    SetupButtonWithTooltip(btn.gameObject, BuyWatermelonUnlock, GenerateWatermelonUnlockTooltip());
            }
        }
    }

    void SetupButtonWithTooltip(GameObject button, UnityEngine.Events.UnityAction clickAction, string tooltipText)
    {
        if (button == null) return;
        
        Button btn = button.GetComponent<Button>();
        if (btn == null) return;
        
        // Очищаем существующие обработчики чтобы избежать дублирования
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(clickAction);
        
        // Удаляем старый EventTrigger если есть
        EventTrigger oldTrigger = button.GetComponent<EventTrigger>();
        if (oldTrigger != null)
        {
            DestroyImmediate(oldTrigger);
        }
        
        // Удаляем старый ButtonTooltipData если есть
        ButtonTooltipData oldData = button.GetComponent<ButtonTooltipData>();
        if (oldData != null)
        {
            DestroyImmediate(oldData);
        }
        
        EventTrigger trigger = button.AddComponent<EventTrigger>();
        
        button.AddComponent<ButtonTooltipData>().tooltipText = tooltipText;
        
        AddPointerEvent(trigger, EventTriggerType.PointerEnter, () => OnButtonHover(button));
        AddPointerEvent(trigger, EventTriggerType.PointerExit, () => HideTooltip());
    }

    void OnButtonHover(GameObject button)
    {
        ButtonTooltipData data = button.GetComponent<ButtonTooltipData>();
        if (data != null && !string.IsNullOrEmpty(data.tooltipText))
        {
            ShowTooltipMessage(data.tooltipText);
        }
    }

    string GenerateGrowSpeedTooltip()
    {
        if (ShopManager.Instance == null) return "";
        if (LocalizationManager.Instance == null) return "";
        
        int currentLevel = ShopManager.Instance.growSpeedLevel;
        int maxLevel = ShopManager.Instance.growSpeedCosts.Length;
        float effect = 0.5f * (currentLevel + 1);
        
        if (currentLevel >= maxLevel)
            return LocalizationManager.Instance.Get("tooltip_grow_speed_max");
        
        return LocalizationManager.Instance.GetFormatted("tooltip_grow_speed", 
            currentLevel + 1, maxLevel, effect, effect + 0.5f);
    }

    string GenerateHarvestValueTooltip()
    {
        if (ShopManager.Instance == null) return "";
        if (LocalizationManager.Instance == null) return "";
        
        int currentLevel = ShopManager.Instance.harvestValueLevel;
        int maxLevel = ShopManager.Instance.harvestValueCosts.Length;
        int effect = 2 * (currentLevel + 1);
        
        if (currentLevel >= maxLevel)
            return LocalizationManager.Instance.Get("tooltip_harvest_value_max");
        
        return LocalizationManager.Instance.GetFormatted("tooltip_harvest_value", 
            currentLevel + 1, maxLevel, effect, effect + 2);
    }

    string GenerateCritChanceTooltip()
    {
        if (ShopManager.Instance == null) return "";
        if (LocalizationManager.Instance == null) return "";
        
        int currentLevel = ShopManager.Instance.critChanceLevel;
        int maxLevel = ShopManager.Instance.critChanceCosts.Length;
        float effect = 10f * (currentLevel + 1);
        
        if (currentLevel >= maxLevel)
            return LocalizationManager.Instance.Get("tooltip_crit_chance_max");
        
        return LocalizationManager.Instance.GetFormatted("tooltip_crit_chance", 
            currentLevel + 1, maxLevel, effect, effect + 10f);
    }

    string GenerateFertilizerTooltip()
    {
        if (ShopManager.Instance == null) return "";
        if (LocalizationManager.Instance == null) return "";
        
        int currentLevel = ShopManager.Instance.fertilizerLevel;
        int maxLevel = ShopManager.Instance.fertilizerCosts.Length;
        int effect = 2 * (currentLevel + 1);
        
        if (currentLevel >= maxLevel)
            return LocalizationManager.Instance.Get("tooltip_fertilizer_max");
        
        return LocalizationManager.Instance.GetFormatted("tooltip_fertilizer", 
            currentLevel + 1, maxLevel, effect, effect + 2);
    }

    string GenerateSuperSeedTooltip()
    {
        if (ShopManager.Instance == null) return "";
        if (LocalizationManager.Instance == null) return "";
        
        int currentLevel = ShopManager.Instance.superSeedLevel;
        int maxLevel = ShopManager.Instance.superSeedCosts.Length;
        
        if (currentLevel >= maxLevel)
            return LocalizationManager.Instance.Get("tooltip_super_seed_max");
        
        return LocalizationManager.Instance.GetFormatted("tooltip_super_seed", 
            currentLevel + 1, maxLevel, currentLevel + 1, currentLevel + 2);
    }

    string GenerateWatermelonUnlockTooltip()
    {
        if (WatermelonUnlockManager.Instance == null) return "";
        if (LocalizationManager.Instance == null) return "";
        
        if (WatermelonUnlockManager.Instance.IsAllUnlocked())
            return LocalizationManager.Instance.Get("tooltip_all_unlocked");
        
        WatermelonData next = WatermelonUnlockManager.Instance.GetNext();
        if (next == null) return "";
        return LocalizationManager.Instance.GetFormatted("tooltip_watermelon_unlock", 
            next.GetName(), WatermelonUnlockManager.Instance.GetNextUnlockCost(), next.harvestValue);
    }

    void AddPointerEvent(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction callback)
    {
        var entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener((data) => callback());
        trigger.triggers.Add(entry);
    }

    public void OpenShop()
    {
        isShopOpen = true;
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            StartCoroutine(DelayedSetup());
        }
    }
    
    System.Collections.IEnumerator DelayedSetup()
    {
        yield return null;
        
        SetupButtons();
        
        if (openShopButton != null)
            openShopButton.SetActive(false);

        UpdateUI();
    }

    public void CloseShop()
    {
        isShopOpen = false;
        if (shopPanel != null)
            shopPanel.SetActive(false);

        if (openShopButton != null)
            openShopButton.SetActive(true);
    }

    void UpdateUI()
    {
        if (growSpeedText != null)
        {
            int cost = ShopManager.Instance != null ? ShopManager.Instance.GetGrowSpeedCost() : -1;
            string name = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("grow_speed") : "Grow Speed";
            string level = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("level") : "LVL";
            string coins = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("coins") : "coins";
            string maxLevel = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("max_level") : "MAX LEVEL";
            
            growSpeedText.text = cost == -1 
                ? $"{name}\n{maxLevel}"
                : $"{name}\n{level} {ShopManager.Instance.growSpeedLevel + 1}: {cost} {coins}";
        }

        if (harvestValueText != null)
        {
            int cost = ShopManager.Instance != null ? ShopManager.Instance.GetHarvestValueCost() : -1;
            string name = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("harvest_value") : "Harvest Value";
            string level = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("level") : "LVL";
            string coins = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("coins") : "coins";
            string maxLevel = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("max_level") : "MAX LEVEL";
            
            harvestValueText.text = cost == -1 
                ? $"{name}\n{maxLevel}"
                : $"{name}\n{level} {ShopManager.Instance.harvestValueLevel + 1}: {cost} {coins}";
        }

        if (critChanceText != null)
        {
            int cost = ShopManager.Instance != null ? ShopManager.Instance.GetCritChanceCost() : -1;
            string name = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("crit_harvest") : "Crit Harvest";
            string level = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("level") : "LVL";
            string coins = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("coins") : "coins";
            string maxLevel = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("max_level") : "MAX LEVEL";
            
            critChanceText.text = cost == -1 
                ? $"{name}\n{maxLevel}"
                : $"{name}\n{level} {ShopManager.Instance.critChanceLevel + 1}: {cost} {coins}";
        }

        if (fertilizerText != null)
        {
            int cost = ShopManager.Instance != null ? ShopManager.Instance.GetFertilizerCost() : -1;
            string name = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("fertilizer") : "Fertilizer";
            string level = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("level") : "LVL";
            string coins = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("coins") : "coins";
            string maxLevel = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("max_level") : "MAX LEVEL";
            
            fertilizerText.text = cost == -1 
                ? $"{name}\n{maxLevel}"
                : $"{name}\n{level} {ShopManager.Instance.fertilizerLevel + 1}: {cost} {coins}";
        }

        if (superSeedText != null)
        {
            int cost = ShopManager.Instance != null ? ShopManager.Instance.GetSuperSeedCost() : -1;
            string name = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("super_seeds") : "Super Seeds";
            string level = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("level") : "LVL";
            string coins = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("coins") : "coins";
            string maxLevel = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("max_level") : "MAX LEVEL";
            
            superSeedText.text = cost == -1 
                ? $"{name}\n{maxLevel}"
                : $"{name}\n{level} {ShopManager.Instance.superSeedLevel + 1}: {cost} {coins}";
        }

        if (watermelonUnlockText != null)
        {
            if (WatermelonUnlockManager.Instance != null && WatermelonUnlockManager.Instance.IsAllUnlocked())
            {
                watermelonUnlockText.text = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("all_unlocked") : "ALL UNLOCKED";
            }
            else if (WatermelonUnlockManager.Instance != null)
            {
                WatermelonData next = WatermelonUnlockManager.Instance.GetNext();
                int cost = WatermelonUnlockManager.Instance.GetNextUnlockCost();
                string unlockTemplate = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("unlock") : "Unlock {0}";
                string coins = LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("coins") : "coins";
                watermelonUnlockText.text = string.Format(unlockTemplate, next.GetName()) + $"\n{cost} {coins}";
            }
            else
            {
                watermelonUnlockText.text = "N/A";
            }
        }
    }

    public void BuyGrowSpeed()
    {
        if (ShopManager.Instance == null || ShopManager.Instance.growSpeedLevel >= ShopManager.Instance.growSpeedCosts.Length)
        {
            ShowTooltipMessage(LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("tooltip_max_upgrade") : "Max level");
        }
        else if (ShopManager.Instance.BuyGrowSpeed())
        {
            UpdateUI();
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.upgradeSound);
        }
        else
        {
            ShowTooltipMessage(LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("tooltip_not_enough_coins") : "Not enough coins");
        }
    }

    public void BuyHarvestValue()
    {
        if (ShopManager.Instance == null || ShopManager.Instance.harvestValueLevel >= ShopManager.Instance.harvestValueCosts.Length)
        {
            ShowTooltipMessage(LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("tooltip_max_upgrade") : "Max level");
        }
        else if (ShopManager.Instance.BuyHarvestValue())
        {
            UpdateUI();
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.upgradeSound);
        }
        else
        {
            ShowTooltipMessage(LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("tooltip_not_enough_coins") : "Not enough coins");
        }
    }

    public void BuyCritChance()
    {
        if (ShopManager.Instance == null || ShopManager.Instance.critChanceLevel >= ShopManager.Instance.critChanceCosts.Length)
        {
            ShowTooltipMessage(LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("tooltip_max_upgrade") : "Max level");
        }
        else if (ShopManager.Instance.BuyCritChance())
        {
            UpdateUI();
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.upgradeSound);
        }
        else
        {
            ShowTooltipMessage(LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("tooltip_not_enough_coins") : "Not enough coins");
        }
    }

    public void BuyFertilizer()
    {
        if (ShopManager.Instance == null || ShopManager.Instance.fertilizerLevel >= ShopManager.Instance.fertilizerCosts.Length)
        {
            ShowTooltipMessage(LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("tooltip_max_upgrade") : "Max level");
        }
        else if (ShopManager.Instance.BuyFertilizer())
        {
            UpdateUI();
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.upgradeSound);
        }
        else
        {
            ShowTooltipMessage(LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("tooltip_not_enough_coins") : "Not enough coins");
        }
    }

    public void BuySuperSeed()
    {
        if (ShopManager.Instance == null || ShopManager.Instance.superSeedLevel >= ShopManager.Instance.superSeedCosts.Length)
        {
            ShowTooltipMessage(LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("tooltip_max_upgrade") : "Max level");
        }
        else if (ShopManager.Instance.BuySuperSeed())
        {
            UpdateUI();
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.upgradeSound);
        }
        else
        {
            ShowTooltipMessage(LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("tooltip_not_enough_coins") : "Not enough coins");
        }
    }

    public void BuyWatermelonUnlock()
    {
        if (WatermelonUnlockManager.Instance == null || WatermelonUnlockManager.Instance.IsAllUnlocked())
        {
            ShowTooltipMessage(LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("tooltip_all_unlocked") : "All unlocked");
        }
        else if (WatermelonUnlockManager.Instance.UnlockNext())
        {
            UpdateUI();
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.upgradeSound);
        }
        else
        {
            ShowTooltipMessage(LocalizationManager.Instance != null ? LocalizationManager.Instance.Get("tooltip_not_enough_coins") : "Not enough coins");
        }
    }
}

// Класс для хранения текста подсказки у кнопки
public class ButtonTooltipData : MonoBehaviour
{
    public string tooltipText;
}