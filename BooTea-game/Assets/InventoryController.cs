using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject slotPrefab;            // Prefab slota
    public int slotCount;                    // Liczba slotów w ekwipunku
    public GameObject[] itemPrefabs;         // Tablica slotów
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < slotCount; i++)
        {
           Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();
           if (i < itemPrefabs.Length)
            {
                GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Ustawienie pozycji przedmiotu w slocie
                slot.currentItem = item; // Przypisanie przedmiotu do slota
            }
        }
    }
}
