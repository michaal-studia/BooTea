using System.Collections;
using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private Animator animator;
    private InventoryController inventoryController;
    private PlayerMovement playerMovement; // Reference to PlayerMovement

    void Start()
    {
        animator = GetComponent<Animator>();
        inventoryController = FindFirstObjectByType<InventoryController>();
        playerMovement = GetComponent<PlayerMovement>(); // Get the PlayerMovement component
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            // Block player movement
            if (playerMovement != null)
            {
                playerMovement.BlockMovement();
            }

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

                    // Start a coroutine to handle the item's visibility, destruction, and unblocking movement
                    StartCoroutine(HandleItemAfterPickup(collision.gameObject, 0.6f));
                }
            }
        }
    }

    // Coroutine to handle the item's visibility, destruction, and unblocking movement
    private IEnumerator HandleItemAfterPickup(GameObject item, float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Disable the item's sprite renderer to hide it
        SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // Destroy the item after hiding it
        Destroy(item);

        // Unblock player movement
        if (playerMovement != null)
        {
            playerMovement.UnblockMovement();
        }
    }
}
