using UnityEngine;
using UnityEngine.UI;

public class ResetSaveButton : MonoBehaviour
{
    public int slot = 0;
    public SaveSlotsUI saveSlotsUI;

    private Button btn;

    void Start()
    {
        btn = GetComponent<Button>();
        RefreshVisibility();
    }

    public void RefreshVisibility()
    {
        // Получаем btn каждый раз, не только в Start()
        if (btn == null) btn = GetComponent<Button>();
        if (btn == null) return; // защита если компонент не найден
        
        bool exists = SaveManager.Instance != null && SaveManager.Instance.SlotExists(slot);
        btn.gameObject.SetActive(exists);
    }

    public void ResetSave()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("SaveManager не найден. Невозможно сбросить сохранение.");
            return;
        }
        
        SaveManager.Instance.DeleteSlot(slot);
        
        // Сбрасываем разблокированные сорта
        PlayerPrefs.DeleteKey("UnlockedWatermelons");
        
        // Сбрасываем уровни улучшений
        PlayerPrefs.DeleteKey("GrowSpeedLevel");
        PlayerPrefs.DeleteKey("HarvestValueLevel");
        PlayerPrefs.DeleteKey("CritChanceLevel");
        PlayerPrefs.DeleteKey("FertilizerLevel");
        PlayerPrefs.DeleteKey("SuperSeedLevel");
        PlayerPrefs.DeleteKey("Coins");
        
        PlayerPrefs.Save();
        
        // Сбрасываем в памяти
        if (WatermelonUnlockManager.Instance != null)
            WatermelonUnlockManager.Instance.ResetUnlocks();
        
        if (ShopManager.Instance != null)
            ShopManager.Instance.SetLevels(0, 0, 0, 0, 0);
        
        saveSlotsUI.RefreshUI();
        RefreshVisibility();
    }
}