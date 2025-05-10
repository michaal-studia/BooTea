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

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cinemachineConfiner = Object.FindFirstObjectByType<CinemachineConfiner2D>();
        inventoryController = FindFirstObjectByType<InventoryController>();
        hotbarController = FindFirstObjectByType<HotbarController>();
    }

    private string GetSavePath(string slotName)
    {
        return Path.Combine(Application.persistentDataPath, $"saveData_{slotName}.json");
    }

    public void SaveGameToSlot(string slotName)
    {
        if (string.IsNullOrEmpty(slotName)) return;

        AudioManager.Play("ButtonAffirmative");

        SaveData saveData = new SaveData
        {
            playerPosition = player.transform.position,
            mapBoundary = cinemachineConfiner.BoundingShape2D?.gameObject.name,
            inventorySaveData = inventoryController.GetInventoryItems() ?? new List<InventorySaveData>(),
            hotbarSaveData = hotbarController.GetHotbarItems()
        };

        string path = GetSavePath(slotName);
        File.WriteAllText(path, JsonUtility.ToJson(saveData));
        Debug.Log($"Game saved to slot: {slotName}");
    }

    public void LoadGameFromSlot(string slotName)
    {
        string path = GetSavePath(slotName);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"Save slot '{slotName}' not found.");
            return;
        }

        AudioManager.Play("ButtonAffirmative");
        SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));

        player.transform.position = saveData.playerPosition;

        GameObject boundaryObject = GameObject.Find(saveData.mapBoundary);
        if (boundaryObject)
        {
            cinemachineConfiner.BoundingShape2D = boundaryObject.GetComponent<PolygonCollider2D>();
        }

        inventoryController.SetInventoryItems(saveData.inventorySaveData);
        hotbarController.SetHotbarItems(saveData.hotbarSaveData);

        Debug.Log($"Game loaded from slot: {slotName}");
    }

    public void DeleteSlot(string slotName)
    {
        string path = GetSavePath(slotName);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Deleted save slot: {slotName}");
        }
        else
        {
            Debug.LogWarning($"No save to delete in slot: {slotName}");
        }
    }
}
