using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance { get; private set; } // Singleton instance

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Set singleton instance
            DontDestroyOnLoad(gameObject); // Optional: Keep the inventory controller between scenes
        }
        else
        {
            Debug.LogError("Multiple InventoryController instances detected! Destroying the extra one.");
            Destroy(gameObject); // Prevent multiple instances
        }
    }

    private ItemDictionary itemDictionary;
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;

    void Start()
    {
        // Find the item dictionary
        FindItemDictionary();

        // Create empty slots at startup
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }
    }

    // Make sure we have a reference to the item dictionary
    private void FindItemDictionary()
    {
        if (itemDictionary == null)
        {
            itemDictionary = FindFirstObjectByType<ItemDictionary>();
            if (itemDictionary == null)
            {
                Debug.LogError("ItemDictionary not found in the scene! Inventory loading will fail.");
            }
        }
    }

    public bool AddItem(GameObject itemPrefab)
    {
        //Look for empty slot
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;
                return true; // Item added successfully
            }
        }
        Debug.Log("Inventory is full!");
        return false; // No empty slot found
    }

    public bool HasFreeSlot()
    {
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                return true; // Found an empty slot
            }
        }
        return false; // No empty slots found
    }

    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item != null)
                {
                    invData.Add(new InventorySaveData
                    {
                        itemID = item.ID,
                        slotIndex = slotTransform.GetSiblingIndex()
                    });
                }
            }
        }
        return invData;
    }

    public IEnumerator SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {
        // Make sure we have a reference to the item dictionary
        FindItemDictionary();

        if (itemDictionary == null)
        {
            Debug.LogError("ItemDictionary is null, can't load inventory items!");
            yield break;
        }

        // Clear inventory panel - destroy all children
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Wait for destruction to complete
        yield return null;

        // Create new slots
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }

        // Wait for slots to be created
        yield return null;

        // Populate slots with saved items
        foreach (InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < inventoryPanel.transform.childCount)
            {
                Transform slotTransform = inventoryPanel.transform.GetChild(data.slotIndex);
                Slot slot = slotTransform.GetComponent<Slot>();

                if (slot == null)
                {
                    Debug.LogError($"Slot component not found at index {data.slotIndex}");
                    continue;
                }

                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
                if (itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    slot.currentItem = item;
                }
                else
                {
                    Debug.LogError($"Item prefab with ID {data.itemID} not found in dictionary");
                }
            }
            else
            {
                Debug.LogError($"Slot index {data.slotIndex} out of range!");
            }
        }
    }
}