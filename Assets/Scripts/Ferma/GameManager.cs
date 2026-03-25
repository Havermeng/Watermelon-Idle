using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TextMeshProUGUI coinText;
    private int coins = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Start()
    {
        GameReadyBridge.Initialize();
        
        if (SaveManager.Instance == null)
        {
            coins = 0;
            if (coinText != null) coinText.text = "0";
            return;
        }
        
        int slot = SaveManager.Instance.selectedSlot;
        if (SaveManager.Instance.SlotExists(slot))
            SaveManager.Instance.LoadFromSlot(slot);
        else
        {
            coins = 0;
            if (coinText != null) coinText.text = "0";
        }
        
        GameReadyBridge.GameReady();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        if (coinText != null)
            coinText.text = coins.ToString();
        else
            Debug.LogWarning("GameManager: coinText is null, cannot update UI");
        PlayerPrefs.SetInt("Coins", coins);
        SaveManager.Instance?.MarkDirty();
    }

    public int GetCoins() { return coins; }

    public void SetCoins(int amount)
    {
        coins = amount;
        if (coinText != null)
            coinText.text = coins.ToString();
        else
            Debug.LogWarning("GameManager: coinText is null, cannot update UI");
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            if (coinText != null)
                coinText.text = coins.ToString();
            else
                Debug.LogWarning("GameManager: coinText is null, cannot update UI");
            PlayerPrefs.SetInt("Coins", coins);
            SaveManager.Instance?.MarkDirty();
            return true;
        }
        return false;
    }
}