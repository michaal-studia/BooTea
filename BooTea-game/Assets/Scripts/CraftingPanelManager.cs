using System.Collections.Generic;
using UnityEngine;

public class CraftingPanelManager : MonoBehaviour
{
    public Slot slotTeaCup;
    public Slot slotTea;
    public Slot slotWater;
    public Slot slotAddons;
    public Slot slotResult;

    public GameObject spoiledTeaPrefab;

    [System.Serializable]
    public class CraftingRecipe
    {
        public GameObject teaCupPrefab;
        public GameObject teaPrefab;
        public GameObject waterPrefab;
        public GameObject addonPrefab;
        public GameObject resultPrefab;

        public bool Matches(GameObject cup, GameObject tea, GameObject water, GameObject addon)
        {
            return CompareNames(cup, teaCupPrefab)
                && CompareNames(tea, teaPrefab)
                && CompareNames(water, waterPrefab)
                && CompareNames(addon, addonPrefab);
        }

        private bool CompareNames(GameObject obj, GameObject prefab)
        {
            if (obj == null || prefab == null) return false;
            string objName = obj.name.Replace("(Clone)", "").Trim();
            string prefabName = prefab.name.Trim();
            return objName == prefabName;
        }
    }

    public List<CraftingRecipe> recipes;

    public void TryCraft()
    {
        GameObject cupObj = GetItemPrefab(slotTeaCup);
        GameObject teaObj = GetItemPrefab(slotTea);
        GameObject waterObj = GetItemPrefab(slotWater);
        GameObject addonObj = GetItemPrefab(slotAddons);

        foreach (CraftingRecipe recipe in recipes)
        {
            if (recipe.Matches(cupObj, teaObj, addonObj, waterObj))
            {
                ClearSlot(slotTeaCup);
                ClearSlot(slotTea);
                ClearSlot(slotWater);
                ClearSlot(slotAddons);
                AudioManager.Play("StoveSuccess");
                AudioManager.Play("TeaCrafted");
                GameObject resultItem = Instantiate(recipe.resultPrefab, slotResult.transform);
                resultItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slotResult.currentItem = resultItem;
                return;
            }
        }
        if (cupObj != null && teaObj != null && waterObj != null && addonObj != null && spoiledTeaPrefab != null)
        {
            ClearSlot(slotTeaCup);
            ClearSlot(slotTea);
            ClearSlot(slotWater);
            ClearSlot(slotAddons);
            AudioManager.Play("Error2");
            AudioManager.Play("TeaSpoiled");
            Debug.Log("Brak pasującego przepisu. Tworzę zepsutą herbatę.");
            GameObject spoiledTea = Instantiate(spoiledTeaPrefab, slotResult.transform);
            spoiledTea.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            slotResult.currentItem = spoiledTea;
            return;
        }

        AudioManager.Play("Error");
        Debug.Log("Brak pasującego przepisu.");
    }

    private void ClearSlot(Slot slot)
    {
        if (slot.currentItem != null)
        {
            Destroy(slot.currentItem);
            slot.currentItem = null;
        }
    }

    private GameObject GetItemPrefab(Slot slot)
    {
        if (slot.currentItem == null) return null;
        return slot.currentItem;
    }
}
