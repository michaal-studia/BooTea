using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSaveController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject loadUIPanel;
    public TMPro.TMP_Text selectedSlotText;

    [Header("Other GameObjects")]
    public GameObject leavesSystem; // <- Dodaj obiekt systemu cz¹steczek

    private string selectedSlotName = "";

    private void Start()
    {
        // Make sure GameManager exists
        if (GameManager.Instance == null)
        {
            GameObject gameManagerObj = new GameObject("GameManager");
            gameManagerObj.AddComponent<GameManager>();
        }
    }

    private string GetSavePath(string slotName)
    {
        return Path.Combine(Application.persistentDataPath, $"saveData_{slotName}.json");
    }

    public void OpenLoadUI()
    {
        if (loadUIPanel != null)
        {
            loadUIPanel.SetActive(true);
            leavesSystem.SetActive(false);
            AudioManager.Play("ButtonAffirmative");
        }
    }

    public void CloseLoadUI()
    {
        if (loadUIPanel != null)
        {
            leavesSystem.SetActive(true);
            loadUIPanel.SetActive(false);
            selectedSlotName = "";
            if (selectedSlotText != null)
                selectedSlotText.text = "";
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
            if (selectedSlotText != null)
            {
                selectedSlotText.text = slotName;
            }
            Debug.Log($"Selected slot: {slotName}");
            AudioManager.Play("ButtonAffirmative");
        }
        else
        {
            // No save in this slot - clear selection
            selectedSlotName = "";
            if (selectedSlotText != null)
            {
                selectedSlotText.text = "No Save Found";
            }
            Debug.Log($"No save found in slot: {slotName}");
            AudioManager.Play("ButtonDissenting");
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

    public bool IsSlotEmpty(string slotName)
    {
        string savePath = GetSavePath(slotName);
        return !File.Exists(savePath);
    }

    // Method to check if a slot has a save (useful for UI visual updates)
    public bool HasSaveInSlot(string slotName)
    {
        return !IsSlotEmpty(slotName);
    }
}