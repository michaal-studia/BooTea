using System.Collections.Generic;
using UnityEngine;

public class CraftingPanelManager : MonoBehaviour
{
    public Slot slotTeaCup;
    public Slot slotTea;
    public Slot slotWater;
    public Slot slotAddons;
    public Slot slotResult;

    [System.Serializable]
    public class CraftingRecipe
    {
        public int teaCupID;
        public int teaID;
        public int waterID;
        public int addonID;
        public GameObject resultPrefab;

        public bool Matches(int cup, int tea, int water, int addon)
        {
            return cup == teaCupID && tea == teaID && water == waterID && addon == addonID;
        }
    }

    public List<CraftingRecipe> recipes;

    public void TryCraft()
    {
        int cupID = GetItemID(slotTeaCup);
        int teaID = GetItemID(slotTea);
        int waterID = GetItemID(slotWater);
        int addonID = GetItemID(slotAddons);

        foreach (CraftingRecipe recipe in recipes)
        {
            if (recipe.Matches(cupID, teaID, waterID, addonID))
            {
                ClearSlot(slotTeaCup);
                ClearSlot(slotTea);
                ClearSlot(slotWater);
                ClearSlot(slotAddons);

                GameObject resultItem = Instantiate(recipe.resultPrefab, slotResult.transform);
                resultItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slotResult.currentItem = resultItem;
                return;
            }
        }

        Debug.Log("Brak pasuj¹cego przepisu.");
    }

    private int GetItemID(Slot slot)
    {
        if (slot.currentItem == null) return -1;
        Item item = slot.currentItem.GetComponent<Item>();
        return item != null ? item.ID : -1;
    }

    private void ClearSlot(Slot slot)
    {
        if (slot.currentItem != null)
        {
            Destroy(slot.currentItem);
            slot.currentItem = null;
        }
    }
}
