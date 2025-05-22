using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public bool shouldLoadSave = false;
    public SaveData pendingSaveData = null;
    public string loadedSlotName = "";

    private void Awake()
    {
        // Singleton pattern - persist across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPendingLoad(SaveData saveData, string slotName)
    {
        shouldLoadSave = true;
        pendingSaveData = saveData;
        loadedSlotName = slotName;
        Debug.Log($"Pending load set for slot: {slotName}");
    }

    public void ClearPendingLoad()
    {
        shouldLoadSave = false;
        pendingSaveData = null;
        loadedSlotName = "";
    }

    public bool HasPendingLoad()
    {
        return shouldLoadSave && pendingSaveData != null;
    }
}