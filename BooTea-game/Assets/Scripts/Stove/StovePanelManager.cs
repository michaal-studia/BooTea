using System.Collections.Generic;
using UnityEngine;

public class StovePanelManager : MonoBehaviour
{
    public Slot slotFilledPot;
    public Slot slotResult;

    [System.Serializable]
    public class CraftingRecipe
    {
        public GameObject filledPotPrefab;
        public GameObject successResultPrefab; // Prefab gdy QTE się uda
        public GameObject failResultPrefab;    // Prefab gdy QTE się nie uda
        public int requiredTemperature;        // Wymagana temperatura

        public bool Matches(GameObject inputPrefab, int temperature)
        {
            if (inputPrefab == null) return false;

            string inputName = inputPrefab.name.Replace("(Clone)", "").Trim();
            string prefabName = filledPotPrefab.name.Trim();

            return inputName == prefabName && temperature == requiredTemperature;
        }
    }

    public List<CraftingRecipe> recipes;

    public void TryCraft(int temperature, bool qteSuccess)
    {
        GameObject inputPrefab = GetItemPrefab(slotFilledPot);

        if (inputPrefab == null)
        {
            Debug.Log("Brak przedmiotu w slocie.");
            return;
        }

        foreach (CraftingRecipe recipe in recipes)
        {
            if (recipe.Matches(inputPrefab, temperature) && slotResult.currentItem == null)
            {
                ClearSlot(slotFilledPot);

                // Wybierz odpowiedni prefab w zależności od wyniku QTE
                GameObject resultPrefab = qteSuccess ? recipe.successResultPrefab : recipe.failResultPrefab;

                GameObject resultItem = Instantiate(resultPrefab, slotResult.transform);
                resultItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slotResult.currentItem = resultItem;
                return;
            }
            else if (recipe.Matches(inputPrefab, temperature) && slotResult.currentItem != null)
            {
                Debug.Log("Slot na wynik zajęty.");
                return;
            }
        }

        Debug.Log("Brak pasującego przepisu.");
    }

    private GameObject GetItemPrefab(Slot slot)
    {
        return slot.currentItem;
    }

    private void ClearSlot(Slot slot)
    {
        if (slot.currentItem != null)
        {
            GameObject itemToDestroy = slot.currentItem;
            slot.currentItem = null;
            Destroy(itemToDestroy);
        }
    }
}