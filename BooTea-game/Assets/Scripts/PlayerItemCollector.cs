using System.Collections;
using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{

    private Animator animator;

    private InventoryController inventoryController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        inventoryController = FindFirstObjectByType<InventoryController>(); // Poprawiono na FindFirstObjectByType zamiast FindObjectOfType
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            animator.SetTrigger("pickUp"); // Trigger the pick-up animation
            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                // Add item to the inventory
                bool itemAdded = inventoryController.AddItem(collision.gameObject);

                if (itemAdded)
                {
                    item.PickUp();
                    // Disable the collider to prevent further interactions
                    collision.enabled = false;

                    // Start a coroutine to handle the item's visibility and destruction
                    StartCoroutine(HandleItemAfterPickup(collision.gameObject, 0.6f));
                }
            }
        }
    }

    // Coroutine to handle the item's visibility and destruction
    private IEnumerator HandleItemAfterPickup(GameObject item, float delay)
    {
        // Wait for the specified delay (0.8 seconds)
        yield return new WaitForSeconds(delay);

        // Disable the item's sprite renderer to hide it
        SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // Destroy the item after hiding it
        Destroy(item);
    }


}
