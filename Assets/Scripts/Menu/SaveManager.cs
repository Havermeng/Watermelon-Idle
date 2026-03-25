using UnityEngine;
using System;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public int selectedSlot = 0;
    public const int SlotCount = 2;
    private SaveData[] slots = new SaveData[SlotCount];

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Если запустили сцену напрямую — используем слот 0
            if (selectedSlot < 0) selectedSlot = 0;
        }
        else
        {
            Destroy(gameObject);
        }
        
        LoadAllSlotHeaders();
    }

    // Загружаем заголовки всех слотов (имя + дата) при старте
    void LoadAllSlotHeaders()
    {
        for (int i = 0; i < SlotCount; i++)
        {
            string json = PlayerPrefs.GetString("Save_Slot_" + i, "");
            if (!string.IsNullOrEmpty(json))
                slots[i] = JsonUtility.FromJson<SaveData>(json);
            else
                slots[i] = null;
        }
    }

    // Получить данные слота (для UI)
    public SaveData GetSlotData(int slot) => slots[slot];

    // Сохранить текущий прогресс в слот
    public void SaveToSlot(int slot)
    {
        FarmCell[] cells = FindObjectsOfType<FarmCell>();

        SaveData data = new SaveData();
        data.saveName = slots[slot] != null ? slots[slot].saveName : "Слот " + (slot + 1);
        data.saveDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        data.coins = GameManager.Instance != null ? GameManager.Instance.GetCoins() : 0;
        data.growSpeedLevel = ShopManager.Instance != null ? ShopManager.Instance.growSpeedLevel : 0;
        data.harvestValueLevel = ShopManager.Instance != null ? ShopManager.Instance.harvestValueLevel : 0;
        data.critChanceLevel = ShopManager.Instance != null ? ShopManager.Instance.critChanceLevel : 0;
        data.fertilizerLevel = ShopManager.Instance != null ? ShopManager.Instance.fertilizerLevel : 0;
        data.superSeedLevel = ShopManager.Instance != null ? ShopManager.Instance.superSeedLevel : 0;

        // Сохраняем грядки по индексу
        int maxIndex = 0;
        foreach (var cell in cells)
            if (cell.cellIndex > maxIndex) maxIndex = cell.cellIndex;

        data.cells = new CellData[maxIndex + 1];
        for (int i = 0; i <= maxIndex; i++)
            data.cells[i] = new CellData();

        foreach (var cell in cells)
        {
            if (cell.cellIndex >= 0 && cell.cellIndex < data.cells.Length)
            {
                data.cells[cell.cellIndex] = cell.GetCellData();
            }
        }

        slots[slot] = data;
        PlayerPrefs.SetString("Save_Slot_" + slot, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    // Загрузить прогресс из слота
    public void LoadFromSlot(int slot)
    {
        SaveData data = slots[slot];
        if (data == null) return;

        GameManager.Instance.SetCoins(data.coins);
        ShopManager.Instance.SetLevels(
            data.growSpeedLevel,
            data.harvestValueLevel,
            data.critChanceLevel,
            data.fertilizerLevel,
            data.superSeedLevel
        );

        FarmCell[] cells = FindObjectsOfType<FarmCell>();
        foreach (var cell in cells)
        {
            if (cell.cellIndex < data.cells.Length)
                cell.LoadCellData(data.cells[cell.cellIndex]);
            else
                cell.LoadCellData(new CellData());
        }
    }

    // Переименовать слот
    public void RenameSlot(int slot, string newName)
    {
        if (slots[slot] == null) return;
        slots[slot].saveName = newName;
        PlayerPrefs.SetString("Save_Slot_" + slot, JsonUtility.ToJson(slots[slot]));
        PlayerPrefs.Save();
    }

    // Проверить, занят ли слот
    public bool SlotExists(int slot) => slots[slot] != null;

    public void CreateNewSave(int slot, string name)
    {
        SaveData data = new SaveData();
        data.saveName = name;
        data.saveDate = System.DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        data.coins = 0;
        data.growSpeedLevel = 0;
        data.harvestValueLevel = 0;
        data.critChanceLevel = 0;
        data.fertilizerLevel = 0;
        data.superSeedLevel = 0;
        data.cells = new CellData[0];

        slots[slot] = data;
        PlayerPrefs.SetString("Save_Slot_" + slot, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    public void DeleteSlot(int slot)
    {
        slots[slot] = null;
        PlayerPrefs.DeleteKey("Save_Slot_" + slot);
        PlayerPrefs.Save();
    }
}
