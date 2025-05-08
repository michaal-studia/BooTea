using UnityEngine;

public class CraftingTableInteraction : MonoBehaviour, IInteractable
{
    public GameObject craftingPanel;
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
        craftingPanel.SetActive(isPanelOpen);
    }

    public bool canInteract()
    {
        return true;
    }
}
