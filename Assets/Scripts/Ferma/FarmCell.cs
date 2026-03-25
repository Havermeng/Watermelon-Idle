using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FarmCell : MonoBehaviour
{
    public Sprite emptySprite;

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
    private WatermelonData activeWatermelonData;

    private ShopManager shopManager;
    private GameManager gameManager;
    private WatermelonUnlockManager unlockManager;

    private float cachedGrowTime = -1f;
    private float lastCacheTime = -1f;
    private const float CACHE_DURATION = 0.5f;

    private bool isGrowing => growthStage > 0 && growthStage < 3;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        shopManager = ShopManager.Instance;
        gameManager = GameManager.Instance;
        unlockManager = WatermelonUnlockManager.Instance;
    }

    void Start()
    {
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

        LoadState();
        UpdateSprite();
        if (isGrowing)
            SetBarVisible(true);

        enabled = isGrowing;
    }

    void LoadState()
    {
        growthStage = PlayerPrefs.GetInt("Cell_" + cellIndex + "_Stage", 0);
        growTimer = PlayerPrefs.GetFloat("Cell_" + cellIndex + "_Timer", 0f);
        int watermelonIndex = PlayerPrefs.GetInt("Cell_" + cellIndex + "_WatermelonIndex", -1);

        if (growthStage > 0 && watermelonIndex >= 0 && unlockManager != null)
        {
            if (watermelonIndex >= 0 && watermelonIndex < unlockManager.watermelons.Length)
                activeWatermelonData = unlockManager.watermelons[watermelonIndex];
            else
                activeWatermelonData = unlockManager.GetCurrent();
        }
    }

    void Update()
    {
        if (!isGrowing)
        {
            enabled = false;
            return;
        }

        growTimer += Time.deltaTime;

        if (progressSlider != null)
            progressSlider.value = growTimer / GetCachedGrowTime();

        if (growTimer >= GetCachedGrowTime())
        {
            growTimer = 0f;
            growthStage++;
            UpdateSprite();
            SaveCell();

            if (growthStage == 3)
            {
                SetBarVisible(false);
                enabled = false;
            }
        }
    }

    float GetCachedGrowTime()
    {
        if (Time.time - lastCacheTime > CACHE_DURATION || cachedGrowTime < 0)
        {
            cachedGrowTime = CalculateGrowTime();
            lastCacheTime = Time.time;
        }
        return cachedGrowTime;
    }

    float CalculateGrowTime()
    {
        float baseTime = activeWatermelonData != null ? activeWatermelonData.growTime : growTime;
        float multiplier = shopManager != null ? shopManager.GetGrowTime() / shopManager.GetBaseGrowTime() : 1f;
        return baseTime * multiplier;
    }

    void OnMouseDown()
    {
        if (Time.timeScale == 0f) return;
        if (ShopUI.isShopOpen) return;

        if (growthStage == 0)
        {
            Plant();
        }
        else if (growthStage == 3)
        {
            Harvest();
        }
    }

    void Plant()
    {
        if (unlockManager == null) return;

        activeWatermelonData = unlockManager.GetCurrent();
        if (activeWatermelonData == null) return;

        int startStage = shopManager != null ? shopManager.GetStartStage() : 1;

        growthStage = startStage;
        growTimer = 0f;

        if (progressSlider != null)
            progressSlider.value = 0f;

        SetBarVisible(startStage < 3);
        UpdateSprite();
        SaveCell();
        AudioManager.Instance?.PlayPlantSound();

        cachedGrowTime = -1f;
        enabled = isGrowing;
    }

    void Harvest()
    {
        int harvestValue = GetCurrentHarvestValue();
        gameManager?.AddCoins(harvestValue);
        HarvestTextPool.Instance?.Show(cellCanvas.transform, harvestValue);
        HarvestEffect.Instance?.Play(transform.position);
        AudioManager.Instance?.PlayWatermelonHarvestSound();

        growthStage = 0;
        growTimer = 0f;
        activeWatermelonData = null;
        UpdateSprite();
        SaveCell();
        cachedGrowTime = -1f;
    }

    int GetCurrentHarvestValue()
    {
        int baseValue = activeWatermelonData != null ? activeWatermelonData.harvestValue : 10;
        int bonus = shopManager?.GetFertilizerBonus() ?? 0;
        float crit = shopManager?.GetCritChance() ?? 0f;
        int total = baseValue + bonus;
        return Random.value < crit ? total * 2 : total;
    }

    void UpdateSprite()
    {
        if (sr == null) return;

        switch (growthStage)
        {
            case 0:
                sr.sprite = emptySprite;
                break;
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

    void SaveCell()
    {
        PlayerPrefs.SetInt("Cell_" + cellIndex + "_Stage", growthStage);
        PlayerPrefs.SetFloat("Cell_" + cellIndex + "_Timer", growTimer);

        if (activeWatermelonData != null && unlockManager != null)
        {
            int index = System.Array.IndexOf(unlockManager.watermelons, activeWatermelonData);
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

        if (activeWatermelonData != null && unlockManager != null)
        {
            int index = System.Array.IndexOf(unlockManager.watermelons, activeWatermelonData);
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

        if (growthStage > 0 && unlockManager != null)
        {
            int watermelonIndex = data.watermelonIndex;
            if (watermelonIndex >= 0 && watermelonIndex < unlockManager.watermelons.Length)
                activeWatermelonData = unlockManager.watermelons[watermelonIndex];
            else
                activeWatermelonData = unlockManager.GetCurrent();
        }
        else
        {
            activeWatermelonData = null;
        }

        UpdateSprite();
        SetBarVisible(isGrowing);
        cachedGrowTime = -1f;
        enabled = isGrowing;
    }
}