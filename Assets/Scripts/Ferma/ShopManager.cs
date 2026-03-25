using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    public int growSpeedLevel = 0;
    public int harvestValueLevel = 0;
    public int critChanceLevel = 0;
    public int fertilizerLevel = 0;
    public int superSeedLevel = 0;

    public int[] growSpeedCosts = { 100, 250, 625, 1562 };
    public int[] harvestValueCosts = { 120, 300, 750 };
    public int[] critChanceCosts = { 150, 375, 938 };
    public int[] fertilizerCosts = { 100, 250, 625 };
    public int[] superSeedCosts = { 350, 875 };

    private float[] growTimes = { 6f, 5.25f, 4.5f, 3.75f, 3f };
    private int[] harvestValues = { 10, 12, 15, 20 };
    private float[] critChances = { 0f, 0.1f, 0.2f, 0.35f };
    private int[] fertilizerBonus = { 0, 2, 5, 10 };
    private int[] superSeedStages = { 1, 2, 3 };

    // Универсальный метод для масштабирования стоимости
    private int GetScaledCost(int level, int[] baseArray, float multiplier)
    {
        if (baseArray == null || baseArray.Length == 0)
            return -1;

        if (level < baseArray.Length)
            return baseArray[level];

        int lastCost = baseArray[baseArray.Length - 1];
        int extraLevel = level - (baseArray.Length - 1);

        return Mathf.RoundToInt(lastCost * Mathf.Pow(multiplier, extraLevel));
    }

    void Awake()
    {
        Instance = this;
        growSpeedLevel = PlayerPrefs.GetInt("GrowSpeedLevel", 0);
        harvestValueLevel = PlayerPrefs.GetInt("HarvestValueLevel", 0);
        critChanceLevel = PlayerPrefs.GetInt("CritChanceLevel", 0);
        fertilizerLevel = PlayerPrefs.GetInt("FertilizerLevel", 0);
        superSeedLevel = PlayerPrefs.GetInt("SuperSeedLevel", 0);
    }

    public float GetGrowTime()
    {
        if (growTimes == null || growTimes.Length == 0) return 6f;
        if (growSpeedLevel < 0) return growTimes[0];
        if (growSpeedLevel < growTimes.Length)
            return growTimes[growSpeedLevel];
        
        // Плавная прогрессия после окончания массива
        float baseTime = growTimes[growTimes.Length - 1];
        int extraLevel = growSpeedLevel - (growTimes.Length - 1);
        return Mathf.Max(1.5f, baseTime * Mathf.Pow(0.9f, extraLevel));
    }

    public float GetBaseGrowTime()
    {
        if (growTimes == null || growTimes.Length == 0) return 6f;
        return growTimes[0];
    }

    public int GetHarvestValue()
    {
        if (harvestValues == null || harvestValues.Length == 0) return 10;
        if (harvestValueLevel < 0) return harvestValues[0];
        if (harvestValueLevel < harvestValues.Length)
            return harvestValues[harvestValueLevel];
        
        // Плавная прогрессия после окончания массива
        int baseValue = harvestValues[harvestValues.Length - 1];
        int extraLevel = harvestValueLevel - (harvestValues.Length - 1);
        return Mathf.RoundToInt(baseValue * Mathf.Pow(1.25f, extraLevel));
    }

    public float GetCritChance()
    {
        if (critChances == null || critChances.Length == 0) return 0f;
        if (critChanceLevel < 0) return critChances[0];
        if (critChanceLevel < critChances.Length)
            return critChances[critChanceLevel];
        
        // Плавная прогрессия после окончания массива
        float baseChance = critChances[critChances.Length - 1];
        int extraLevel = critChanceLevel - (critChances.Length - 1);
        return Mathf.Min(0.9f, baseChance + (0.15f * extraLevel));
    }

    public int GetFertilizerBonus()
    {
        if (fertilizerBonus == null || fertilizerBonus.Length == 0) return 0;
        if (fertilizerLevel < 0) return fertilizerBonus[0];
        if (fertilizerLevel < fertilizerBonus.Length)
            return fertilizerBonus[fertilizerLevel];
        
        // Плавная прогрессия после окончания массива
        int baseBonus = fertilizerBonus[fertilizerBonus.Length - 1];
        int extraLevel = fertilizerLevel - (fertilizerBonus.Length - 1);
        return baseBonus + (3 * extraLevel);
    }

    public int GetStartStage()
    {
        if (superSeedStages == null || superSeedStages.Length == 0) return 1;
        if (superSeedLevel < 0) return superSeedStages[0];
        if (superSeedLevel < superSeedStages.Length)
            return superSeedStages[superSeedLevel];
        
        // Плавная прогрессия после окончания массива
        int baseStage = superSeedStages[superSeedStages.Length - 1];
        int extraLevel = superSeedLevel - (superSeedStages.Length - 1);
        return baseStage + extraLevel;
    }

    public int GetGrowSpeedCost()
    {
        return GetScaledCost(growSpeedLevel, growSpeedCosts, 1.5f);
    }

    public int GetHarvestValueCost()
    {
        return GetScaledCost(harvestValueLevel, harvestValueCosts, 1.6f);
    }

    public int GetCritChanceCost()
    {
        return GetScaledCost(critChanceLevel, critChanceCosts, 1.7f);
    }

    public int GetFertilizerCost()
    {
        return GetScaledCost(fertilizerLevel, fertilizerCosts, 1.8f);
    }

    public int GetSuperSeedCost()
    {
        return GetScaledCost(superSeedLevel, superSeedCosts, 2.0f);
    }

    public void SetLevels(int growSpeed, int harvestValue, int critChance, int fertilizer, int superSeed)
    {
        growSpeedLevel = growSpeed;
        harvestValueLevel = harvestValue;
        critChanceLevel = critChance;
        fertilizerLevel = fertilizer;
        superSeedLevel = superSeed;
    }

    public bool BuyGrowSpeed()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("ShopManager: GameManager.Instance is null, cannot buy grow speed upgrade");
            return false;
        }
        int cost = GetGrowSpeedCost();
        if (cost == -1) return false;
        if (GameManager.Instance.SpendCoins(cost))
        {
            growSpeedLevel++;
            PlayerPrefs.SetInt("GrowSpeedLevel", growSpeedLevel);
            AudioManager.Instance?.PlayUpgradeSound();
            return true;
        }
        return false;
    }

    public bool BuyHarvestValue()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("ShopManager: GameManager.Instance is null, cannot buy harvest value upgrade");
            return false;
        }
        int cost = GetHarvestValueCost();
        if (cost == -1) return false;
        if (GameManager.Instance.SpendCoins(cost))
        {
            harvestValueLevel++;
            PlayerPrefs.SetInt("HarvestValueLevel", harvestValueLevel);
            AudioManager.Instance?.PlayUpgradeSound();
            return true;
        }
        return false;
    }

    public bool BuyCritChance()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("ShopManager: GameManager.Instance is null, cannot buy crit chance upgrade");
            return false;
        }
        int cost = GetCritChanceCost();
        if (cost == -1) return false;
        if (GameManager.Instance.SpendCoins(cost))
        {
            critChanceLevel++;
            PlayerPrefs.SetInt("CritChanceLevel", critChanceLevel);
            AudioManager.Instance?.PlayUpgradeSound();
            return true;
        }
        return false;
    }

    public bool BuyFertilizer()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("ShopManager: GameManager.Instance is null, cannot buy fertilizer upgrade");
            return false;
        }
        int cost = GetFertilizerCost();
        if (cost == -1) return false;
        if (GameManager.Instance.SpendCoins(cost))
        {
            fertilizerLevel++;
            PlayerPrefs.SetInt("FertilizerLevel", fertilizerLevel);
            AudioManager.Instance?.PlayUpgradeSound();
            return true;
        }
        return false;
    }

    public bool BuySuperSeed()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("ShopManager: GameManager.Instance is null, cannot buy super seed upgrade");
            return false;
        }
        int cost = GetSuperSeedCost();
        if (cost == -1) return false;
        if (GameManager.Instance.SpendCoins(cost))
        {
            superSeedLevel++;
            PlayerPrefs.SetInt("SuperSeedLevel", superSeedLevel);
            AudioManager.Instance?.PlayUpgradeSound();
            return true;
        }
        return false;
    }
}