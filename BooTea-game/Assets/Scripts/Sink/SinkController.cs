using UnityEngine;

public class SinkController : MonoBehaviour, IInteractable
{
    public Animator anim;

    public GameObject sinkPanel;
    private bool isPanelOpen = false;

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
        sinkPanel.SetActive(isPanelOpen);
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
