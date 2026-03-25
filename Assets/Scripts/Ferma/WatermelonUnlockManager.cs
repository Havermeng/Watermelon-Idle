using UnityEngine;

public class WatermelonUnlockManager : MonoBehaviour
{
    public static WatermelonUnlockManager Instance;

    [Header("Сорта арбузов (по порядку разблокировки)")]
    public WatermelonData[] watermelons;

    private int unlockedCount = 1; // зелёный всегда доступен

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            unlockedCount = PlayerPrefs.GetInt("UnlockedWatermelons", 1);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Текущий активный сорт (последний разблокированный)
    public WatermelonData GetCurrent()
    {
        if (watermelons == null || watermelons.Length == 0)
        {
            Debug.LogWarning("WatermelonUnlockManager: массив watermelons пуст!");
            return null;
        }
        if (unlockedCount < 1 || unlockedCount > watermelons.Length)
        {
            Debug.LogWarning($"WatermelonUnlockManager: некорректный unlockedCount={unlockedCount}");
            return null;
        }
        return watermelons[unlockedCount - 1];
    }

    // Следующий сорт для разблокировки (null если все открыты)
    public WatermelonData GetNext()
    {
        if (watermelons == null || watermelons.Length == 0)
        {
            Debug.LogWarning("WatermelonUnlockManager: массив watermelons пуст!");
            return null;
        }
        if (unlockedCount >= watermelons.Length) return null;
        return watermelons[unlockedCount];
    }

    public int GetNextUnlockCost()
    {
        WatermelonData next = GetNext();
        return next != null ? next.unlockCost : -1;
    }

    public bool UnlockNext()
    {
        if (GetNext() == null) return false;
        int cost = GetNextUnlockCost();
        if (!GameManager.Instance.SpendCoins(cost)) return false;

        unlockedCount++;
        PlayerPrefs.SetInt("UnlockedWatermelons", unlockedCount);
        PlayerPrefs.Save();
        return true;
    }

    public bool IsAllUnlocked() => unlockedCount >= watermelons.Length;

    public void ResetUnlocks()
    {
        unlockedCount = 1;
    }
}
