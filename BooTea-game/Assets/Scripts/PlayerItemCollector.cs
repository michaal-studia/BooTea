using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController inventoryController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryController = FindFirstObjectByType<InventoryController>(); // Poprawiono na FindFirstObjectByType zamiast FindObjectOfType
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                //Add item to the inventory
                bool itemAdded = inventoryController.AddItem(collision.gameObject);

                if (itemAdded)
                {
                    item.PickUp();
                    // Destroy the item in the world
                    Destroy(collision.gameObject);
                }
            }
        }
    }

}
