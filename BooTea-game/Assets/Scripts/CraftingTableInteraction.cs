using UnityEngine;

public class CraftingTableInteraction : MonoBehaviour, IInteractable
{
    public GameObject craftingPanel;
    private bool isPanelOpen = false;
    public Animator anim;

    public void Interact()
    {
        if (!isPanelOpen)
        {
            AudioManager.Play("MaximizeSwoosh1");
        }
        else
        {
            AudioManager.Play("MinimizeSwoosh1");
        }
        isPanelOpen = !isPanelOpen;
        craftingPanel.SetActive(isPanelOpen);
    }

    public bool canInteract()
    {
        return true;
    }
        private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("playerInRange", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("playerInRange", false);
        }
    }

}
