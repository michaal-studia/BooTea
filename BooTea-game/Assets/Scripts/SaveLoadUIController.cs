using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class SaveLoadUIController : MonoBehaviour
{
    [Header("UI References")]
    public UIManager uiManager;
    public SaveController saveController;
    [Header("Time Display")]
    public TMPro.TMP_Text saveTimePlayedText; // For save page
    public TMPro.TMP_Text loadTimePlayedText; // For load page
    [Header("Slot Buttons")]
    public Button[] saveSlotButtons; // Assign your save slot buttons in the inspector
    public Button[] loadSlotButtons; // Assign your load slot buttons in the inspector

    [Header("Visual Settings")]
    [Range(0f, 1f)]
    public float emptySlotAlpha = 0.3f; // Transparency for empty slots
    [Range(0f, 1f)]
    public float filledSlotAlpha = 1f; // Opacity for slots with saves

    private string selectedSlotName;
    private bool isSaveMode;
    private Dictionary<string, bool> slotStates = new Dictionary<string, bool>();

    private void Start()
    {
        // Initialize slot visual states
        UpdateAllSlotVisuals();
    }

    // These functions are now callable from Buttons
    public void OpenSaveUI()
    {
        AudioManager.Play("ButtonAffirmative");
        uiManager.settingsPage.SetActive(false);
        uiManager.saveUIPage.SetActive(true);
        isSaveMode = true;
        UpdateSaveSlotVisuals(); // Refresh save slot visuals when opening save UI
        ClearSelection(); // Clear selection when switching modes
    }

    public void OpenLoadUI()
    {
        AudioManager.Play("ButtonAffirmative");
        uiManager.settingsPage.SetActive(false);
        uiManager.loadUIPage.SetActive(true);
        isSaveMode = false;
        UpdateLoadSlotVisuals(); // Refresh load slot visuals when opening load UI
        ClearSelection(); // Clear selection when switching modes
    }

    public void GoBackToSettings()
    {

        AudioManager.Play("ButtonDissenting");
        uiManager.saveUIPage.SetActive(false);
        uiManager.loadUIPage.SetActive(false);
        uiManager.settingsPage.SetActive(true);
        ClearSelection();
    }

    private void ClearSelection()
    {
        selectedSlotName = "";
        ClearSlotNameDisplay();
        ClearTimeDisplay();
    }

    public void SelectSlot(string slotName)
    {
        AudioManager.Play("ButtonAffirmative");
        selectedSlotName = slotName;
        if (isSaveMode)
        {
            uiManager.textSaveName.text = slotName;
            // For save mode, show current play time
            UpdateTimeDisplay();
        }
        else
        {
            // For load mode, only show slot name if it has a save
            if (HasSaveInSlot(slotName))
            {
                uiManager.textLoadName.text = slotName;
                // Show saved play time
                UpdateTimeDisplay();
            }
            else
            {
                uiManager.textLoadName.text = "Empty Slot";
                selectedSlotName = ""; // Don't allow selection of empty slots for loading
                ClearTimeDisplay();
            }
        }
        Debug.Log("Selected slot: " + slotName);
    }

    private void UpdateTimeDisplay()
    {
        if (string.IsNullOrEmpty(selectedSlotName))
        {
            ClearTimeDisplay();
            return;
        }

        TMPro.TMP_Text targetText = isSaveMode ? saveTimePlayedText : loadTimePlayedText;

        if (targetText == null) return;

        if (isSaveMode)
        {
            // Show current play time for save mode
            if (GameTimeTracker.Instance != null)
            {
                float currentTime = GameTimeTracker.Instance.GetCurrentTotalPlayTime();
                targetText.text = $"TIME PLAYED: {FormatTime(currentTime)}";
            }
            else
            {
                targetText.text = "TIME PLAYED: 00:00";
            }
        }
        else
        {
            // Show saved play time for load mode
            SaveData saveData = GetSaveDataFromSlot(selectedSlotName);
            if (saveData != null)
            {
                targetText.text = $"TIME PLAYED: {saveData.GetFormattedPlayTime()}";
            }
            else
            {
                targetText.text = "TIME PLAYED: 00:00";
            }
        }
    }

    private void ClearTimeDisplay()
    {
        if (saveTimePlayedText != null)
        {
            saveTimePlayedText.text = "";
        }
        if (loadTimePlayedText != null)
        {
            loadTimePlayedText.text = "";
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        int hours = (int)(timeInSeconds / 3600);
        int minutes = (int)((timeInSeconds % 3600) / 60);
        int seconds = (int)(timeInSeconds % 60);

        if (hours > 0)
        {
            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }
        else
        {
            return $"{minutes:D2}:{seconds:D2}";
        }
    }

    private SaveData GetSaveDataFromSlot(string slotName)
    {
        if (saveController != null)
        {
            return saveController.GetSaveDataFromSlot(slotName);
        }

        // Fallback method if saveController doesn't have the method
        string path = GetSavePath(slotName);
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            string saveJson = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(saveJson);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to read save data from slot {slotName}: {e.Message}");
            return null;
        }
    }

    public void SaveToSelectedSlot()
    {
        if (string.IsNullOrEmpty(selectedSlotName)) return;
        saveController.SaveGameToSlot(selectedSlotName);

        // Update save slot visuals after saving
        UpdateSaveSlotVisuals();
        // Update time display to reflect the saved time
        UpdateTimeDisplay();
    }

    public void LoadFromSelectedSlot()
    {
        if (string.IsNullOrEmpty(selectedSlotName)) return;
        if (!HasSaveInSlot(selectedSlotName))
        {
            Debug.LogWarning($"Cannot load from empty slot: {selectedSlotName}");
            return;
        }
        saveController.LoadGameFromSlot(selectedSlotName);
    }

    public void DeleteSelectedSlot()
    {
        if (string.IsNullOrEmpty(selectedSlotName)) return;
        saveController.DeleteSlot(selectedSlotName);

        // Update both save and load slot visuals after deleting
        UpdateSaveSlotVisuals();
        UpdateLoadSlotVisuals();

        // Clear selection after deletion
        ClearSelection();
    }

    private void UpdateSaveSlotVisuals()
    {
        if (saveSlotButtons == null) return;

        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            if (saveSlotButtons[i] != null)
            {
                // Slot names are "Save #1", "Save #2", etc.
                string slotName = $"Save #{i + 1}";
                UpdateSlotVisual(saveSlotButtons[i], slotName, true); // true = save mode
            }
        }
    }

    private void UpdateLoadSlotVisuals()
    {
        if (loadSlotButtons == null) return;

        for (int i = 0; i < loadSlotButtons.Length; i++)
        {
            if (loadSlotButtons[i] != null)
            {
                // Slot names are "Save #1", "Save #2", etc.
                string slotName = $"Save #{i + 1}";
                UpdateSlotVisual(loadSlotButtons[i], slotName, false); // false = load mode
            }
        }
    }

    private void UpdateAllSlotVisuals()
    {
        UpdateSaveSlotVisuals();
        UpdateLoadSlotVisuals();
    }

    private void UpdateSlotVisual(Button slotButton, string slotName, bool isSaveMode)
    {
        bool hasSave = HasSaveInSlot(slotName);
        slotStates[slotName] = hasSave;

        // Get all Image components (button itself and any child images)
        Image[] images = slotButton.GetComponentsInChildren<Image>();

        // Update alpha for all images
        foreach (Image img in images)
        {
            Color color = img.color;
            color.a = hasSave ? filledSlotAlpha : emptySlotAlpha;
            img.color = color;
        }

        // Also update text components if any
        TMPro.TextMeshProUGUI[] texts = slotButton.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        foreach (TMPro.TextMeshProUGUI text in texts)
        {
            Color color = text.color;
            color.a = hasSave ? filledSlotAlpha : emptySlotAlpha;
            text.color = color;
        }

        // Set button interactability based on mode
        if (isSaveMode)
        {
            slotButton.interactable = true; // Always allow interaction in save mode
        }
        else
        {
            slotButton.interactable = hasSave; // Only allow interaction if slot has save in load mode
        }
    }

    private bool HasSaveInSlot(string slotName)
    {
        string savePath = GetSavePath(slotName);
        return File.Exists(savePath);
    }

    private string GetSavePath(string slotName)
    {
        return Path.Combine(Application.persistentDataPath, $"saveData_{slotName}.json");
    }

    private void ClearSlotNameDisplay()
    {
        if (uiManager.textSaveName != null)
            uiManager.textSaveName.text = "";
        if (uiManager.textLoadName != null)
            uiManager.textLoadName.text = "";
    }

    // Public method to refresh visuals (can be called from other scripts)
    public void RefreshSlotVisuals()
    {
        UpdateAllSlotVisuals();
    }

    // Method to check if a specific slot has a save (useful for external scripts)
    public bool IsSlotEmpty(string slotName)
    {
        return !HasSaveInSlot(slotName);
    }
}