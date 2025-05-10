using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarController : MonoBehaviour
{
    public GameObject hotbarPanel;
    public GameObject slotPrefab;
    public int slotCount = 4; // Number of slots in the hotbar 1-0 on the keyboard

    private ItemDictionary itemDictionary;

    private Key[] hotbarKeys;

    private void Awake()
    {
        itemDictionary = FindFirstObjectByType<ItemDictionary>(); // Poprawiono z FindObjectByType na FindFirstObjectByType

        hotbarKeys = new Key[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            hotbarKeys[i] = i < 4 ? (Key)((int)Key.Digit1 + i) : Key.Digit0; // e.g. Slot 0 is the first slot on the hotbar but needs to be set to Key.Digit1
            // Arrays in code start at 0, not 1
            //Slot9 = Digit0
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check for key presses
        for (int i = 0; i < slotCount; i++)
        {
            if (Keyboard.current[hotbarKeys[i]].wasPressedThisFrame)
            {
                //UseItem
                UseItemInSlot(i);
            }
        }
    }

    void UseItemInSlot(int index)
    {
        Slot slot = hotbarPanel.transform.GetChild(index).GetComponent<Slot>();
        if (slot.currentItem != null)
        {
            Item item = slot.currentItem.GetComponent<Item>();
            item.UseItem();
        }
    }

    public List<InventorySaveData> GetHotbarItems()
    {
        List<InventorySaveData> hotbarData = new List<InventorySaveData>();
        foreach (Transform slotTransform in hotbarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                hotbarData.Add(new InventorySaveData
                {
                    itemID = item.ID,
                    slotIndex = slotTransform.GetSiblingIndex()
                });
            }
        }
        return hotbarData;
    }

    public void SetHotbarItems(List<InventorySaveData> inventorySaveData)
    {
        //Clear inventory panel - avoid duplication
        foreach (Transform child in hotbarPanel.transform)
        {
            Destroy(child.gameObject);
        }

        //Create new slots
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, hotbarPanel.transform);
        }

        //Populate slots with saved items
        foreach (InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < slotCount)
            {
                Slot slot = hotbarPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
                if (itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Ustawienie pozycji przedmiotu w slocie
                    slot.currentItem = item; // Przypisanie przedmiotu do slota
                }
            }
        }
    }
}
