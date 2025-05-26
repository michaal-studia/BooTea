using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public string mapBoundary;
    public List<InventorySaveData> inventorySaveData;
    public List<InventorySaveData> hotbarSaveData;
    public List<QuestProgress> questProgressData;

    // New time tracking fields
    public float totalPlayTime; // Total time played in seconds
    public string lastSaveDate; // When this save was last updated

    public SaveData()
    {
        totalPlayTime = 0f;
        lastSaveDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    // Helper method to get formatted play time
    public string GetFormattedPlayTime()
    {
        int hours = (int)(totalPlayTime / 3600);
        int minutes = (int)((totalPlayTime % 3600) / 60);
        int seconds = (int)(totalPlayTime % 60);

        if (hours > 0)
        {
            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }
        else
        {
            return $"{minutes:D2}:{seconds:D2}";
        }
    }
}