using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Cinemachine;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    private InventoryController inventoryController;
    private HotbarController hotbarController;
    private GameObject player;
    private CinemachineConfiner2D cinemachineConfiner;
    private ItemDictionary itemDictionary;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cinemachineConfiner = Object.FindFirstObjectByType<CinemachineConfiner2D>();
        inventoryController = FindFirstObjectByType<InventoryController>();
        hotbarController = FindFirstObjectByType<HotbarController>();
        itemDictionary = FindFirstObjectByType<ItemDictionary>();

        // Verify all required components were found
        if (player == null) Debug.LogError("Player not found!");
        if (cinemachineConfiner == null) Debug.LogError("CinemachineConfiner2D not found!");
        if (inventoryController == null) Debug.LogError("InventoryController not found!");
        if (hotbarController == null) Debug.LogError("HotbarController not found!");
        if (itemDictionary == null) Debug.LogError("ItemDictionary not found!");

        // Initialize time tracker if it doesn't exist
        if (GameTimeTracker.Instance == null)
        {
            GameObject timeTrackerObj = new GameObject("GameTimeTracker");
            timeTrackerObj.AddComponent<GameTimeTracker>();
        }

        // Check if we need to load a save from main menu
        CheckForPendingLoad();
    }

    private void CheckForPendingLoad()
    {
        if (GameManager.Instance != null && GameManager.Instance.HasPendingLoad())
        {
            Debug.Log($"Loading pending save: {GameManager.Instance.loadedSlotName}");
            StartCoroutine(LoadPendingSaveCoroutine());
        }
        else
        {
            // If starting fresh, reset the timer
            if (GameTimeTracker.Instance != null)
            {
                GameTimeTracker.Instance.ResetTimer();
            }
        }
    }

    private IEnumerator LoadPendingSaveCoroutine()
    {
        // Wait for QuestController to be initialized
        while (QuestController.Instance == null)
        {
            yield return null;
        }

        // Wait a few frames to ensure all components are ready
        yield return null;

        if (GameManager.Instance?.pendingSaveData != null)
        {
            SaveData saveData = GameManager.Instance.pendingSaveData;

            // Set player position
            if (player != null)
            {
                player.transform.position = saveData.playerPosition;
            }

            // Set camera boundary if it exists
            if (!string.IsNullOrEmpty(saveData.mapBoundary))
            {
                GameObject boundaryObject = GameObject.Find(saveData.mapBoundary);
                if (boundaryObject && cinemachineConfiner != null)
                {
                    cinemachineConfiner.BoundingShape2D = boundaryObject.GetComponent<PolygonCollider2D>();
                }
            }

            // Load inventory and hotbar items
            yield return LoadItemsCoroutine(saveData);

            // Load quest progress after inventory is loaded
            if (saveData.questProgressData != null && QuestController.Instance != null)
            {
                QuestController.Instance.LoadQuestProgress(saveData.questProgressData);
            }

            // Set the play time in the time tracker
            if (GameTimeTracker.Instance != null)
            {
                GameTimeTracker.Instance.SetTotalPlayTime(saveData.totalPlayTime);
            }

            // Clear the pending load
            GameManager.Instance.ClearPendingLoad();
        }
    }

    private string GetSavePath(string slotName)
    {
        return Path.Combine(Application.persistentDataPath, $"saveData_{slotName}.json");
    }

    public void SaveGameToSlot(string slotName)
    {
        if (string.IsNullOrEmpty(slotName)) return;

        // Check if required components exist
        if (player == null || inventoryController == null || hotbarController == null)
        {
            AudioManager.Play("Error");
            Debug.LogError("Cannot save: Missing required components!");
            return;
        }

        AudioManager.Play("ButtonAffirmative");

        // Get inventory and hotbar items
        List<InventorySaveData> inventoryItems = inventoryController.GetInventoryItems();
        List<InventorySaveData> hotbarItems = hotbarController.GetHotbarItems();

        SaveData saveData = new SaveData
        {
            playerPosition = player.transform.position,
            mapBoundary = cinemachineConfiner.BoundingShape2D?.gameObject.name,
            inventorySaveData = inventoryItems ?? new List<InventorySaveData>(),
            hotbarSaveData = hotbarItems ?? new List<InventorySaveData>(),
            questProgressData = QuestController.Instance.activateQuests,
            totalPlayTime = GameTimeTracker.Instance != null ? GameTimeTracker.Instance.GetCurrentTotalPlayTime() : 0f,
            lastSaveDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        string path = GetSavePath(slotName);
        string saveJson = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(path, saveJson);

        Debug.Log($"Game saved to slot: {slotName} with play time: {saveData.GetFormattedPlayTime()}");
    }

    public void LoadGameFromSlot(string slotName)
    {
        string path = GetSavePath(slotName);
        if (!File.Exists(path))
        {
            AudioManager.Play("Error");
            Debug.LogWarning($"Save slot '{slotName}' not found.");
            return;
        }

        if (player == null || inventoryController == null || hotbarController == null || itemDictionary == null)
        {
            AudioManager.Play("Error");
            Debug.LogError("Cannot load: Missing required components!");
            return;
        }

        AudioManager.Play("ButtonAffirmative");

        string saveJson = File.ReadAllText(path);
        SaveData saveData = JsonUtility.FromJson<SaveData>(saveJson);

        // Set player position
        player.transform.position = saveData.playerPosition;

        // Set camera boundary if it exists
        GameObject boundaryObject = GameObject.Find(saveData.mapBoundary);
        if (boundaryObject)
        {
            cinemachineConfiner.BoundingShape2D = boundaryObject.GetComponent<PolygonCollider2D>();
        }

        // Set the play time in the time tracker
        if (GameTimeTracker.Instance != null)
        {
            GameTimeTracker.Instance.SetTotalPlayTime(saveData.totalPlayTime);
        }

        // Load inventory and hotbar items with coroutine
        StartCoroutine(LoadItemsCoroutine(saveData));
        QuestController.Instance.LoadQuestProgress(saveData.questProgressData);
    }

    private IEnumerator LoadItemsCoroutine(SaveData saveData)
    {
        // Wait one frame to ensure everything is ready
        yield return null;

        yield return inventoryController.StartCoroutine(inventoryController.SetInventoryItems(saveData.inventorySaveData));
        yield return hotbarController.StartCoroutine(hotbarController.SetHotbarItems(saveData.hotbarSaveData));
    }

    public void DeleteSlot(string slotName)
    {
        string path = GetSavePath(slotName);
        if (File.Exists(path))
        {
            AudioManager.Play("ButtonAffirmative");
            File.Delete(path);
            Debug.Log($"Deleted save slot: {slotName}");
        }
        else
        {
            AudioManager.Play("Error");
            Debug.LogWarning($"No save to delete in slot: {slotName}");
        }
    }

    // Helper method to get save data for UI display
    public SaveData GetSaveDataFromSlot(string slotName)
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
}