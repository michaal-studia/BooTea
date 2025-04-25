using System.IO;
using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private InventoryController inventoryController; // INVENTORY
    private HotbarController hotbarController;
    private GameObject player;
    private CinemachineConfiner2D cinemachineConfiner; // Uzycie CinemachineConfiner2D

    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        player = GameObject.FindGameObjectWithTag("Player");
        cinemachineConfiner = Object.FindFirstObjectByType<CinemachineConfiner2D>(); // Znajdz CinemachineConfiner2D

        // INVENTORY
        inventoryController = FindFirstObjectByType<InventoryController>(); // Zamieniono z FindObjectOfType na FindFirstObjectByType
        //
        hotbarController = FindFirstObjectByType<HotbarController>();

        LoadGame();
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPosition = player.transform.position,
            mapBoundary = cinemachineConfiner.BoundingShape2D?.gameObject.name, // Poprawiono na BoundingShape2D

            // INVENTORY
            //inventorySaveData = inventoryController.GetInventoryItems(),
            inventorySaveData = inventoryController.GetInventoryItems() ?? new List<InventorySaveData>(),
            //
            hotbarSaveData = hotbarController.GetHotbarItems()
        };

        //string json = JsonUtility.ToJson(saveData, true);
        //File.WriteAllText(saveLocation, json);
        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
        Debug.Log("Game saved successfully.");
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            //string json = File.ReadAllText(saveLocation);
            //SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));

            player.transform.position = saveData.playerPosition;
            GameObject boundaryObject = GameObject.Find(saveData.mapBoundary);
            cinemachineConfiner.BoundingShape2D = boundaryObject.GetComponent<PolygonCollider2D>(); // Poprawiono na BoundingShape2D

            // INVENTORY
            inventoryController.SetInventoryItems(saveData.inventorySaveData); // Ustawienie przedmiotow w ekwipunku
            //
            hotbarController.SetHotbarItems(saveData.hotbarSaveData);

            Debug.Log("Game loaded successfully.");
        }
        else
        {
            Debug.LogWarning("Save file not found. Creating a new save.");
            SaveGame(); // Save game if no save file exists

            // INVENTORY
            inventoryController.SetInventoryItems(new List<InventorySaveData>());
            //
            hotbarController.SetHotbarItems(new List<InventorySaveData>());
        }
    }
}