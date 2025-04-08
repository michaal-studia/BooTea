using System.IO;
using UnityEngine;
using Unity.Cinemachine;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private GameObject player;
    private CinemachineConfiner2D cinemachineConfiner; // Uøycie CinemachineConfiner2D

    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        player = GameObject.FindGameObjectWithTag("Player");
        cinemachineConfiner = Object.FindFirstObjectByType<CinemachineConfiner2D>(); // Znajdü CinemachineConfiner2D

        if (player == null || cinemachineConfiner == null)
        {
            Debug.LogError("Player or CinemachineConfiner2D not found in the scene.");
            return;
        }

        LoadGame();
    }

    public void SaveGame()
    {
        if (player == null || cinemachineConfiner == null)
        {
            Debug.LogError("Cannot save game. Required components are missing.");
            return;
        }

        try
        {
            SaveData saveData = new SaveData
            {
                playerPosition = player.transform.position,
                mapBoundary = cinemachineConfiner.BoundingShape2D?.gameObject.name // Poprawiono na BoundingShape2D
            };

            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(saveLocation, json);
            Debug.Log("Game saved successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save game: {ex.Message}");
        }
    }

    public void LoadGame()
    {
        if (!File.Exists(saveLocation))
        {
            Debug.LogWarning("Save file not found. Creating a new save.");
            SaveGame();
            return;
        }

        try
        {
            string json = File.ReadAllText(saveLocation);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            if (saveData != null)
            {
                player.transform.position = saveData.playerPosition;

                if (!string.IsNullOrEmpty(saveData.mapBoundary))
                {
                    GameObject boundaryObject = GameObject.Find(saveData.mapBoundary);
                    if (boundaryObject != null)
                    {
                        cinemachineConfiner.BoundingShape2D = boundaryObject.GetComponent<PolygonCollider2D>(); // Poprawiono na BoundingShape2D
                    }
                    else
                    {
                        Debug.LogWarning("Boundary object not found in the scene.");
                    }
                }

                Debug.Log("Game loaded successfully.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load game: {ex.Message}");
        }
    }
}