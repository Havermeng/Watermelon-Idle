using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FarmCell : MonoBehaviour
{
    public Sprite emptySprite; // пустая грядка — одна для всех сортов

    [Header("Настройки")]
    public float growTime = 6f;

    [Header("Сохранение")]
    public int cellIndex;

    private SpriteRenderer sr;
    private Slider progressSlider;
    private GameObject growthBarRoot;
    private Canvas cellCanvas;
    private int growthStage = 0;
    private float growTimer = 0f;
    private WatermelonData activeWatermelonData; // сорт который посажен на этой грядке

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null && emptySprite != null)
            sr.sprite = emptySprite;

        progressSlider = GetComponentInChildren<Slider>(true);
        if (progressSlider != null)
        {
            growthBarRoot = progressSlider.gameObject;
            growthBarRoot.SetActive(false);
        }

        Transform canvasT = transform.Find("Canvas");
        if (canvasT != null)
        {
            cellCanvas = canvasT.GetComponent<Canvas>();
            if (cellCanvas != null)
            {
                cellCanvas.renderMode = RenderMode.WorldSpace;
                cellCanvas.transform.localScale = new Vector3(0.01f, 0.01f, 1f);
            }
        }

        // Загружаем состояние
        growthStage = PlayerPrefs.GetInt("Cell_" + cellIndex + "_Stage", 0);
        growTimer = PlayerPrefs.GetFloat("Cell_" + cellIndex + "_Timer", 0f);
        int watermelonIndex = PlayerPrefs.GetInt("Cell_" + cellIndex + "_WatermelonIndex", -1);
        
        // Восстанавливаем сорт если грядка не пустая
        if (growthStage > 0 && watermelonIndex >= 0 && WatermelonUnlockManager.Instance != null)
        {
            // Пытаемся получить сорт по сохраненному индексу
            if (watermelonIndex >= 0 && watermelonIndex < WatermelonUnlockManager.Instance.watermelons.Length)
            {
                activeWatermelonData = WatermelonUnlockManager.Instance.watermelons[watermelonIndex];
            }
            else
            {
                // Если индекс невалидный, используем текущий разблокированный
                activeWatermelonData = WatermelonUnlockManager.Instance.GetCurrent();
            }
        }
        
        UpdateSprite();
        if (growthStage > 0 && growthStage < 3)
            SetBarVisible(true);
    }

    void Update()
    {
        if (growthStage > 0 && growthStage < 3)
        {
            growTimer += Time.deltaTime;

            if (progressSlider != null)
                progressSlider.value = growTimer / GetCurrentGrowTime();

            if (growTimer >= GetCurrentGrowTime())
            {
                growTimer = 0f;
                growthStage++;
                UpdateSprite();
                SaveCell();

                if (growthStage == 3)
                    SetBarVisible(false);
            }
        }
    }

    void OnMouseDown()
    {
        // Блокируем клики если пауза или магазин открыт
        if (Time.timeScale == 0f) return;
        if (ShopUI.isShopOpen) return;

        if (growthStage == 0)
        {
            // Проверяем что менеджер готов
            if (WatermelonUnlockManager.Instance == null) return;
            
            activeWatermelonData = WatermelonUnlockManager.Instance.GetCurrent();
            
            // Дополнительная проверка
            if (activeWatermelonData == null) return;
            
            growthStage = ShopManager.Instance != null
                ? ShopManager.Instance.GetStartStage()
                : 1;
            growTimer = 0f;
            if (progressSlider != null)
                progressSlider.value = 0f;
            SetBarVisible(growthStage < 3);
            UpdateSprite();
            SaveCell();
            AudioManager.Instance?.PlayPlantSound();
        }
        else if (growthStage == 3)
        {
            int harvestValue = GetCurrentHarvestValue();
            if (GameManager.Instance != null)
                GameManager.Instance.AddCoins(harvestValue);
            ShowHarvestText(harvestValue);
            
            // Частицы!
            HarvestEffect.Instance?.Play(transform.position);
            
            AudioManager.Instance?.PlayWatermelonHarvestSound();
            
            growthStage = 0;
            growTimer = 0f;
            activeWatermelonData = null; // сбрасываем сорт после сбора
            UpdateSprite();
            SaveCell();
        }
    }

    void UpdateSprite()
    {
        if (sr == null) return;
        switch (growthStage)
        {
            case 0: sr.sprite = emptySprite; break;
            case 1:
                if (activeWatermelonData != null) sr.sprite = activeWatermelonData.seedSprite;
                break;
            case 2:
                if (activeWatermelonData != null) sr.sprite = activeWatermelonData.sproutSprite;
                break;
            case 3:
                if (activeWatermelonData != null) sr.sprite = activeWatermelonData.watermelonSprite;
                break;
        }
    }

    void SetBarVisible(bool visible)
    {
        if (growthBarRoot != null)
            growthBarRoot.SetActive(visible);
    }

    float GetCurrentGrowTime()
    {
        // Базовое время из сорта арбуза
        float baseTime = activeWatermelonData != null
            ? activeWatermelonData.growTime
            : growTime;

        // Множитель из магазина (отношение улучшенного времени к базовому)
        float shopMultiplier = ShopManager.Instance != null
            ? ShopManager.Instance.GetGrowTime() / ShopManager.Instance.GetBaseGrowTime()
            : 1f;

        return baseTime * shopMultiplier;
    }

    int GetCurrentHarvestValue()
    {
        // Базовый доход из сорта арбуза
        int baseValue = activeWatermelonData != null
            ? activeWatermelonData.harvestValue
            : 10;

        int bonus = ShopManager.Instance != null
            ? ShopManager.Instance.GetFertilizerBonus()
            : 0;

        float crit = ShopManager.Instance != null
            ? ShopManager.Instance.GetCritChance()
            : 0f;

        bool isCrit = Random.value < crit;
        int total = baseValue + bonus;
        return isCrit ? total * 2 : total;
    }

    void SaveCell()
    {
        PlayerPrefs.SetInt("Cell_" + cellIndex + "_Stage", growthStage);
        PlayerPrefs.SetFloat("Cell_" + cellIndex + "_Timer", growTimer);
        // Сохраняем индекс сорта
        if (activeWatermelonData != null && WatermelonUnlockManager.Instance != null)
        {
            int index = System.Array.IndexOf(WatermelonUnlockManager.Instance.watermelons, activeWatermelonData);
            PlayerPrefs.SetInt("Cell_" + cellIndex + "_WatermelonIndex", index);
        }
        else
        {
            PlayerPrefs.SetInt("Cell_" + cellIndex + "_WatermelonIndex", -1);
        }
    }

    public CellData GetCellData()
    {
        CellData data = new CellData
        {
            stage = growthStage,
            timer = growTimer
        };
        
        // Сохраняем индекс сорта если он есть
        if (activeWatermelonData != null && WatermelonUnlockManager.Instance != null)
        {
            int index = System.Array.IndexOf(WatermelonUnlockManager.Instance.watermelons, activeWatermelonData);
            data.watermelonIndex = index;
        }
        else
        {
            data.watermelonIndex = -1;
        }
        
        return data;
    }

    public void LoadCellData(CellData data)
    {
        growthStage = data.stage;
        growTimer = data.timer;
        
        // Восстанавливаем сорт если грядка не пустая
        if (growthStage > 0 && WatermelonUnlockManager.Instance != null)
        {
            // Используем индекс из данных сохранения
            int watermelonIndex = data.watermelonIndex;
            if (watermelonIndex >= 0 && watermelonIndex < WatermelonUnlockManager.Instance.watermelons.Length)
            {
                activeWatermelonData = WatermelonUnlockManager.Instance.watermelons[watermelonIndex];
            }
            else
            {
                // Если индекс невалидный, используем текущий разблокированный
                activeWatermelonData = WatermelonUnlockManager.Instance.GetCurrent();
            }
        }
        else
        {
            activeWatermelonData = null;
        }
        
        UpdateSprite();
        if (growthStage > 0 && growthStage < 3)
            SetBarVisible(true);
        else
            SetBarVisible(false);
    }

    private void ShowHarvestText(int amount)
    {
        if (cellCanvas == null) return;
        GameObject go = new GameObject("FloatingHarvestText");
        go.transform.SetParent(cellCanvas.transform, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = "+" + amount;
        tmp.fontSize = 96;
        tmp.color = Color.green;
        tmp.alignment = TextAlignmentOptions.Center;
        CanvasGroup cg = go.AddComponent<CanvasGroup>();
        cg.alpha = 1f;
        StartCoroutine(AnimateHarvestText(go));
    }

    private IEnumerator AnimateHarvestText(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();
        cg.alpha = 1f;
        RectTransform rt = obj.GetComponent<RectTransform>();
        Vector3 startPos = rt.anchoredPosition;
        Vector3 endPos = startPos + new Vector3(0f, 100f, 0f);
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed);
            rt.anchoredPosition = Vector3.Lerp(startPos, endPos, t);
            cg.alpha = 1f - t;
            yield return null;
        }
        Destroy(obj);
    }
}