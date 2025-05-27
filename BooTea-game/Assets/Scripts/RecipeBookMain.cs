using UnityEngine;

public class RecipeBookMain : MonoBehaviour
{
    public GameObject RecipeBookPanel;

    private bool playerInRange = false;
    private bool bookIsOpen = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (bookIsOpen)
            {
                CloseBook();
            }
            else
            {
                OpenBook();
            }
        }
    }

    private void OpenBook()
    {
        RecipeBookPanel.SetActive(true);
        bookIsOpen = true;

        // OPTIONAL: freeze time or player here
        Time.timeScale = 0f;  // pauses all in-game movement (including physics)
    }

    private void CloseBook()
    {
        RecipeBookPanel.SetActive(false);
        bookIsOpen = false;

        // Unfreeze time or re-enable movement
        Time.timeScale = 1f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            // auto-close book if player walks away
            if (bookIsOpen)
            {
                CloseBook();
            }
        }
    }
}
