using UnityEngine;

public class CraftingTableInteraction : MonoBehaviour, IInteractable
{
    public GameObject craftingPanel;
    private bool isPanelOpen = false;

    public void Interact()
    {
        isPanelOpen = !isPanelOpen;
        craftingPanel.SetActive(isPanelOpen);
    }

    public bool canInteract()
    {
        return true;
    }

}
