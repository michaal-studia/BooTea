using UnityEngine;

public class TrashBin : MonoBehaviour, IInteractable
{
    private HotbarController hotbarController;

    private void Awake()
    {
        hotbarController = FindFirstObjectByType<HotbarController>();
    }

    public bool canInteract()
    {
        Transform hotbarPanel = hotbarController.hotbarPanel.transform;

        if (hotbarPanel.childCount > 0)
        {
            Slot firstSlot = hotbarPanel.GetChild(0).GetComponent<Slot>();
            if (firstSlot.currentItem != null) return true;
            else return false;
        }
        else return false;
    }

    public void Interact()
    {
        if (hotbarController == null)
        {
            Debug.LogWarning("Hotbar not found!");
            return;
        }

        Transform hotbarPanel = hotbarController.hotbarPanel.transform;

        if (hotbarPanel.childCount > 0)
        {
            Slot firstSlot = hotbarPanel.GetChild(0).GetComponent<Slot>();

            if (firstSlot.currentItem != null)
            {
                GameObject itemToDestroy = firstSlot.currentItem;
                firstSlot.currentItem = null;
                Destroy(itemToDestroy);
                AudioManager.Play("TrashThrow");
                Debug.Log("The item was thrown away.");
            }
            else
            {
                AudioManager.Play("Error");
                Debug.Log("There is nothing in your hand.");
            }
        }
        
    }
}
