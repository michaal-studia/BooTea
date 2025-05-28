using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuSaveController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject loadUIPanel;
    public TMPro.TMP_Text selectedSlotText;
    public TMPro.TMP_Text timePlayedText; // New field for time display

    [Header("Slot Buttons")]
    public Button[] slotButtons; // Array of slot buttons

    [Header("Visual Settings")]
    [Range(0f, 1f)]
    public float emptySlotAlpha = 0.3f; // Transparency for empty slots
    [Range(0f, 1f)]
    public float filledSlotAlpha = 1f; // Opacity for filled slots

    [Header("Other GameObjects")]
    public GameObject leavesSystem;

    private string selectedSlotName = "";

    private void Start()
    {
        // Make sure GameManager exists
        if (GameManager.Instance == null)
        {
            GameObject gameManagerObj = new GameObject("GameManager");
            gameManagerObj.AddComponent<GameManager>();
        }

        // Update slot button visuals on start
        UpdateSlotButtonVisuals();
    }

    private string GetSavePath(string slotName)
    {
        return Path.Combine(Application.persistentDataPath, $"saveData_{slotName}.json");
    }

    public void UpdateSlotButtonVisuals()
    {
        if (slotButtons == null) return;

        for (int i = 0; i < slotButtons.Length; i++)
        {
            if (slotButtons[i] == null) continue;

            string slotName = $"Save #{i + 1}"; // Assuming slots are named Save #1, Save #2, etc.
            bool hasData = HasSaveInSlot(slotName);

            SetSlotButtonVisual(slotButtons[i], hasData);
        }
    }

    private void SetSlotButtonVisual(Button button, bool hasData)
    {
        if (button == null) return;

        // Get all Image components (button itself and children)
        Image[] images = button.GetComponentsInChildren<Image>();

        float targetAlpha = hasData ? filledSlotAlpha : emptySlotAlpha;

        foreach (Image img in images)
        {
            Color color = img.color;
            color.a = targetAlpha;
            img.color = color;
        }

        // Also update text components if any
        TMPro.TMP_Text[] texts = button.GetComponentsInChildren<TMPro.TMP_Text>();
        foreach (TMPro.TMP_Text text in texts)
        {
            Color color = text.color;
            color.a = targetAlpha;
            text.color = color;
        }
    }

    public void OpenLoadUI()
    {
        if (loadUIPanel != null)
        {
            loadUIPanel.SetActive(true);
            leavesSystem.SetActive(false);
            AudioManager.Play("ButtonAffirmative");

            // Update visuals when opening the UI
            UpdateSlotButtonVisuals();

            // Clear selection when opening
            selectedSlotName = "";
            UpdateSelectedSlotDisplay();
        }
    }

    public void CloseLoadUI()
    {
        if (loadUIPanel != null)
        {
            leavesSystem.SetActive(true);
            loadUIPanel.SetActive(false);
            selectedSlotName = "";
            UpdateSelectedSlotDisplay();
            AudioManager.Play("ButtonDissenting");
        }
    }

    public void SelectSlot(string slotName)
    {
        // Check if save exists in this slot
        string savePath = GetSavePath(slotName);
        if (File.Exists(savePath))
        {
            selectedSlotName = slotName;
            UpdateSelectedSlotDisplay();
            Debug.Log($"Selected slot: {slotName}");
            AudioManager.Play("ButtonAffirmative");
        }
        else
        {
            // No save in this slot - clear selection
            selectedSlotName = "";
            UpdateSelectedSlotDisplay();
            Debug.Log($"No save found in slot: {slotName}");
            AudioManager.Play("ButtonDissenting");
        }
    }

    private void UpdateSelectedSlotDisplay()
    {
        if (string.IsNullOrEmpty(selectedSlotName))
        {
            // No slot selected or empty slot
            if (selectedSlotText != null)
                selectedSlotText.text = "";
            if (timePlayedText != null)
                timePlayedText.text = "";
        }
        else
        {
            // Valid slot selected
            if (selectedSlotText != null)
                selectedSlotText.text = selectedSlotName;

            // Get and display time played
            SaveData saveData = GetSaveDataFromSlot(selectedSlotName);
            if (saveData != null && timePlayedText != null)
            {
                timePlayedText.text = $"TIME PLAYED: {saveData.GetFormattedPlayTime()}";
            }
            else if (timePlayedText != null)
            {
                timePlayedText.text = "TIME PLAYED: 00:00";
            }
        }
    }

    private SaveData GetSaveDataFromSlot(string slotName)
    {
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

    public void LoadSelectedSlotAndStartGame()
    {
        if (string.IsNullOrEmpty(selectedSlotName))
        {
            Debug.LogWarning("No slot selected or slot is empty!");
            AudioManager.Play("ButtonDissenting");
            return;
        }

        string savePath = GetSavePath(selectedSlotName);
        if (!File.Exists(savePath))
        {
            Debug.LogWarning($"Save file not found: {savePath}");
            AudioManager.Play("ButtonDissenting");
            return;
        }

        try
        {
            // Load the save data
            string saveJson = File.ReadAllText(savePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(saveJson);

            // Set it as pending load in GameManager
            GameManager.Instance.SetPendingLoad(saveData, selectedSlotName);

            // Start the game scene
            AudioManager.Play("LoadGame");
            SceneManager.LoadScene("SampleScene");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load save from slot {selectedSlotName}: {e.Message}");
            AudioManager.Play("ButtonDissenting");
        }
    }

    public void DeleteSelectedSlot()
    {
        if (string.IsNullOrEmpty(selectedSlotName)) return;
        DeleteSlot(selectedSlotName);

        UpdateSpecificSlotVisual(selectedSlotName);
        UpdateSlotButtonVisuals();
        // Clear selection after deletion
        selectedSlotName = "";
        UpdateSelectedSlotDisplay();
    }

    public void DeleteSlot(string slotName)
    {
        string path = GetSavePath(slotName);
        if (File.Exists(path))
        {
            File.Delete(path);
            AudioManager.Play("ButtonAffirmative");
            Debug.Log($"Deleted save slot: {slotName}");
        }
        else
        {
            AudioManager.Play("ButtonDissenting");
            Debug.LogWarning($"No save to delete in slot: {slotName}");
        }
    }

    public bool IsSlotEmpty(string slotName)
    {
        string savePath = GetSavePath(slotName);
        return !File.Exists(savePath);
    }

    public bool HasSaveInSlot(string slotName)
    {
        return !IsSlotEmpty(slotName);
    }

    // Method to update a specific slot's visual after save/delete operations
    public void UpdateSpecificSlotVisual(string slotName)
    {
        if (slotButtons == null) return;

        // Extract slot number from slot name (assuming format like "Save #1", "Save #2", etc.)
        if (slotName.StartsWith("Save #") && int.TryParse(slotName.Substring(6), out int slotNumber))
        {
            int index = slotNumber - 1; // Convert to 0-based index
            if (index >= 0 && index < slotButtons.Length && slotButtons[index] != null)
            {
                bool hasData = HasSaveInSlot(slotName);
                SetSlotButtonVisual(slotButtons[index], hasData);
            }
        }
    }
}