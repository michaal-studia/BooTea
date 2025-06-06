using System.Collections.Generic;
using UnityEngine;

public class SinkPanelManager : MonoBehaviour
{
    public Slot slotEmptyPot; // Slot na pusty dzbanek
    public Slot slotResult;   // Slot na wynik

    [System.Serializable]
    public class CraftingRecipe
    {
        public GameObject emptyPotPrefab; // Prefab pustego dzbanka
        public GameObject resultPrefab;   // Prefab wyniku

        public bool Matches(GameObject inputPrefab)
        {
            if (inputPrefab == null) return false;

            // Porównaj nazwy obiektów, ignorując "(Clone)"
            return inputPrefab.name.Replace("(Clone)", "").Trim() == emptyPotPrefab.name.Trim();
        }
    }

    public List<CraftingRecipe> recipes; // Lista przepisów

    public void TryCraft()
    {
        GameObject inputPrefab = GetItemPrefab(slotEmptyPot);

        Debug.Log($"Input Prefab: {inputPrefab?.name}");

        foreach (CraftingRecipe recipe in recipes)
        {
            Debug.Log($"Checking recipe: {recipe.emptyPotPrefab?.name}");
            if (recipe.Matches(inputPrefab))
            {
                AudioManager.Play("RunningWaterTap");
                Debug.Log("Recipe matched!");
                ClearSlot(slotEmptyPot);

                GameObject resultItem = Instantiate(recipe.resultPrefab, slotResult.transform);
                resultItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slotResult.currentItem = resultItem;
                return;
            }
        }
        AudioManager.Play("Error");
        Debug.Log("Brak pasującego przepisu.");
    }

    private GameObject GetItemPrefab(Slot slot)
    {
        if (slot.currentItem == null) return null; // Jeśli slot jest pusty, zwróć null
        return slot.currentItem; // Zwróć prefab obecny w slocie
    }

    private void ClearSlot(Slot slot)
    {
        if (slot.currentItem != null)
        {
            Destroy(slot.currentItem); // Usuń obiekt z slotu
            slot.currentItem = null;   // Ustaw slot jako pusty
        }
    }
}
