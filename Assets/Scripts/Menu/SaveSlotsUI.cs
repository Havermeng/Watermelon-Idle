using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SaveSlotsUI : MonoBehaviour
{
    [Header("Слот 1")]
    public TextMeshProUGUI slot1Name;
    public TextMeshProUGUI slot1Date;
    public Button slot1PlayButton;
    public Button slot1RenameButton;

    [Header("Слот 2")]
    public TextMeshProUGUI slot2Name;
    public TextMeshProUGUI slot2Date;
    public Button slot2PlayButton;
    public Button slot2RenameButton;

    [Header("Панель переименования")]
    public GameObject renamePanel;
    public TMP_InputField renameInput;
    public Button renameConfirmButton;
    public Button renameCancelButton;

    private int renamingSlot = -1;

    void Start() { RefreshUI(); }
    void OnEnable() { RefreshUI(); }

    // Вызывается когда сцена MainMenu загружается заново
    void OnApplicationFocus(bool focus)
    {
        if (focus) RefreshUI();
    }

    public void RefreshUI()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("SaveManager не найден. UI слотов сохранения не будет обновлен.");
            return;
        }
        RefreshSlot(0, slot1Name, slot1Date, slot1PlayButton, slot1RenameButton);
        RefreshSlot(1, slot2Name, slot2Date, slot2PlayButton, slot2RenameButton);

        // Обновляем видимость кнопок сброса
        foreach (var btn in FindObjectsOfType<ResetSaveButton>())
            btn.RefreshVisibility();
    }

    void RefreshSlot(int slot, TextMeshProUGUI nameText, TextMeshProUGUI dateText, Button playBtn, Button renameBtn)
    {
        if (SaveManager.Instance.SlotExists(slot))
        {
            SaveData data = SaveManager.Instance.GetSlotData(slot);
            nameText.text = data.saveName;
            dateText.text = data.saveDate;
            renameBtn.gameObject.SetActive(true);
        }
        else
        {
            nameText.text = "Пусто";
            dateText.text = "";
            renameBtn.gameObject.SetActive(false);
        }
    }

    public void OnSlotPlay(int slot)
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("SaveManager не найден. Невозможно загрузить слот.");
            return;
        }
        SaveManager.Instance.selectedSlot = slot;
        if (!SaveManager.Instance.SlotExists(slot))
        {
            SaveManager.Instance.CreateNewSave(slot, "Слот " + (slot + 1));
        }
        SceneManager.LoadScene("ArbuzFerma");
    }

    public void OnSlotRename(int slot)
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("SaveManager не найден. Невозможно переименовать слот.");
            return;
        }
        if (!SaveManager.Instance.SlotExists(slot)) return;
        renamingSlot = slot;
        renameInput.text = SaveManager.Instance.GetSlotData(slot).saveName;
        renamePanel.SetActive(true);
    }

    public void OnRenameConfirm()
    {
        if (renamingSlot >= 0 && !string.IsNullOrEmpty(renameInput.text))
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.RenameSlot(renamingSlot, renameInput.text);
            }
            renamingSlot = -1;
            renamePanel.SetActive(false);
            RefreshUI();
        }
    }

    public void OnRenameCancel()
    {
        renamingSlot = -1;
        renamePanel.SetActive(false);
    }
}