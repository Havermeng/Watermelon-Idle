using UnityEngine;
using System;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public int selectedSlot = 0;
    public const int SlotCount = 2;
    private SaveData[] slots = new SaveData[SlotCount];

    [Header("Auto Save Settings")]
    [SerializeField] private float autoSaveInterval = 30f;
    private float lastAutoSaveTime = 0f;
    private bool isDirty = false;

    private FarmCell[] cachedCells;
    private int lastCellCount = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (selectedSlot < 0) selectedSlot = 0;
        }
        else
        {
            Destroy(gameObject);
        }

        LoadAllSlotHeaders();
    }

    void Start()
    {
        lastAutoSaveTime = Time.time;
    }

    void Update()
    {
        if (Time.time - lastAutoSaveTime >= autoSaveInterval)
        {
            if (isDirty)
            {
                SaveToSlot(selectedSlot, silent: true);
                lastAutoSaveTime = Time.time;
            }
        }
    }

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

    public SaveData GetSlotData(int slot) => slots[slot];

    public void MarkDirty()
    {
        isDirty = true;
    }

    public void SaveToSlot(int slot, bool silent = false)
    {
        if (!silent && !isDirty) return;

        CacheFarmCells();

        SaveData data = new SaveData();
        data.saveName = slots[slot] != null ? slots[slot].saveName : "Слот " + (slot + 1);
        data.saveDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        data.coins = GameManager.Instance != null ? GameManager.Instance.GetCoins() : 0;
        data.growSpeedLevel = ShopManager.Instance != null ? ShopManager.Instance.growSpeedLevel : 0;
        data.harvestValueLevel = ShopManager.Instance != null ? ShopManager.Instance.harvestValueLevel : 0;
        data.critChanceLevel = ShopManager.Instance != null ? ShopManager.Instance.critChanceLevel : 0;
        data.fertilizerLevel = ShopManager.Instance != null ? ShopManager.Instance.fertilizerLevel : 0;
        data.superSeedLevel = ShopManager.Instance != null ? ShopManager.Instance.superSeedLevel : 0;

        int maxIndex = 0;
        foreach (var cell in cachedCells)
            if (cell != null && cell.cellIndex > maxIndex) maxIndex = cell.cellIndex;

        data.cells = new CellData[maxIndex + 1];
        for (int i = 0; i <= maxIndex; i++)
            data.cells[i] = new CellData();

        foreach (var cell in cachedCells)
        {
            if (cell != null && cell.cellIndex >= 0 && cell.cellIndex < data.cells.Length)
            {
                data.cells[cell.cellIndex] = cell.GetCellData();
            }
        }

        slots[slot] = data;
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("Save_Slot_" + slot, json);
        PlayerPrefs.Save();

        isDirty = false;
    }

    void CacheFarmCells()
    {
        FarmCell[] cells = FindObjectsOfType<FarmCell>();

        if (cells.Length != lastCellCount)
        {
            cachedCells = cells;
            lastCellCount = cells.Length;
        }
        else if (cachedCells == null || cachedCells.Length == 0)
        {
            cachedCells = cells;
        }
    }

    public void LoadFromSlot(int slot)
    {
        SaveData data = slots[slot];
        if (data == null) return;

        CacheFarmCells();

        if (GameManager.Instance != null)
            GameManager.Instance.SetCoins(data.coins);

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.SetLevels(
                data.growSpeedLevel,
                data.harvestValueLevel,
                data.critChanceLevel,
                data.fertilizerLevel,
                data.superSeedLevel
            );
        }

        foreach (var cell in cachedCells)
        {
            if (cell == null) continue;

            if (cell.cellIndex < data.cells.Length)
                cell.LoadCellData(data.cells[cell.cellIndex]);
            else
                cell.LoadCellData(new CellData());
        }

        isDirty = false;
    }

    public void RenameSlot(int slot, string newName)
    {
        if (slots[slot] == null) return;
        slots[slot].saveName = newName;
        PlayerPrefs.SetString("Save_Slot_" + slot, JsonUtility.ToJson(slots[slot]));
        PlayerPrefs.Save();
    }

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

    void OnDestroy()
    {
        if (isDirty)
        {
            SaveToSlot(selectedSlot, silent: true);
        }
    }

    void OnApplicationPause(bool pause)
    {
        if (pause && isDirty)
        {
            SaveToSlot(selectedSlot, silent: true);
        }
    }

    void OnApplicationFocus(bool focus)
    {
        if (!focus && isDirty)
        {
            SaveToSlot(selectedSlot, silent: true);
        }
    }
}
