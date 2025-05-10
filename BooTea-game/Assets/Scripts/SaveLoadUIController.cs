using UnityEngine;

public class SaveLoadUIController : MonoBehaviour
{
    public UIManager uiManager;
    public SaveController saveController;

    private string selectedSlotName;
    private bool isSaveMode;

    // These functions are now callable from Buttons
    public void OpenSaveUI()
    {
        uiManager.settingsPage.SetActive(false);
        uiManager.saveUIPage.SetActive(true);
        isSaveMode = true;
    }

    public void OpenLoadUI()
    {
        uiManager.settingsPage.SetActive(false);
        uiManager.loadUIPage.SetActive(true);
        isSaveMode = false;
    }

    public void GoBackToSettings()
    {
        uiManager.saveUIPage.SetActive(false);
        uiManager.loadUIPage.SetActive(false);
        uiManager.settingsPage.SetActive(true);
    }

    public void SelectSlot(string slotName)
    {
        selectedSlotName = slotName;
        if (isSaveMode)
        {
            uiManager.textSaveName.text = slotName;
        }
        else
        {
            uiManager.textLoadName.text = slotName;
        }
        Debug.Log("Selected slot: " + slotName);
    }

    public void SaveToSelectedSlot()
    {
        if (string.IsNullOrEmpty(selectedSlotName)) return;
        saveController.SaveGameToSlot(selectedSlotName);
    }

    public void LoadFromSelectedSlot()
    {
        if (string.IsNullOrEmpty(selectedSlotName)) return;
        saveController.LoadGameFromSlot(selectedSlotName);
    }

    public void DeleteSelectedSlot()
    {
        if (string.IsNullOrEmpty(selectedSlotName)) return;
        saveController.DeleteSlot(selectedSlotName);
    }
}
