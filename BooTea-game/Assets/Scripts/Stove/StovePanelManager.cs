using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StovePanelManager : MonoBehaviour
{
    public Slot slotFilledPot; // Slot na pełny dzbanek
    public Slot slotResult;   // Slot na wynik

    [System.Serializable]
    public class CraftingRecipe
    {
        public GameObject filledPotPrefab; // Prefab pełnego dzbanka
        public GameObject resultPrefab;   // Prefab wyniku

        public bool Matches(GameObject inputPrefab)
        {
            if (inputPrefab == null) return false;

            // Porównaj nazwy obiektów, ignorując "(Clone)"
            return inputPrefab.name.Replace("(Clone)", "").Trim() == filledPotPrefab.name.Trim();
        }
    }

    public List<CraftingRecipe> recipes; // Lista przepisów

    public void TryCraft()
    {
        GameObject inputPrefab = GetItemPrefab(slotFilledPot);

        Debug.Log($"Input Prefab: {inputPrefab?.name}");

        foreach (CraftingRecipe recipe in recipes)
        {
            Debug.Log($"Checking recipe: {recipe.filledPotPrefab?.name}");
            if (recipe.Matches(inputPrefab) && slotResult.currentItem == null)
            {
                Debug.Log("Recipe matched!");
                ClearSlot(slotFilledPot);

                GameObject resultItem = Instantiate(recipe.resultPrefab, slotResult.transform);
                resultItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slotResult.currentItem = resultItem;
                return;
            }
            else if (recipe.Matches(inputPrefab) && slotResult.currentItem != null)
            {
                Debug.Log("Slot na wynik nie jest pusty.");
                return;
            }
        }

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
