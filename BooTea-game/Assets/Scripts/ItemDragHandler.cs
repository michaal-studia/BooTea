using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;

    public float minDropDistance = 2f;
    public float maxDropDistance = 3f;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        AudioManager.Play("DragItem");
        originalParent = transform.parent; // Save OG parent
        transform.SetParent(transform.root); // Above other canvas'
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f; // Semi-transparent during drag
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; // Follow the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // Enables raycasts
        canvasGroup.alpha = 1f; // No longer transparent
        AudioManager.Play("DropItem");

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>(); // Slot where item dropped
        if (dropSlot == null)
        {
            GameObject dropItem = eventData.pointerEnter;
            if (dropItem != null)
            {
                dropSlot = dropItem.GetComponentInParent<Slot>();
            }
        }
        Slot originalSlot = originalParent.GetComponent<Slot>();

        if (dropSlot != null)
        {
            // Is a slot under drop point
            if (dropSlot.currentItem != null)
            {
                // Slot has an item - swap items
                dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                originalSlot.currentItem = dropSlot.currentItem;
                dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else
            {
                originalSlot.currentItem = null;
            }

            // Move item into drop slot
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;
        }
        else
        {
            // If where we're dropping is not within the inventory
            if (!IsWithintheInventory(eventData.position))
            {
                // Drop our item
                DropItem(originalSlot);

                // OTHER VERSION - Destroy item
                // originalSlot.currentItem = null; // Remove item from original slot
                // Destroy(gameObject);
                // return;
            }
            else
            {
                // Snap back item to original slot
                transform.SetParent(originalParent);
            }
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero; //Center
    }

    bool IsWithintheInventory(Vector2 mousePosition)
    {
        RectTransform invetoryRect = originalParent.parent.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(invetoryRect, mousePosition);
    }

    void DropItem(Slot originalSlot)
    {
        originalSlot.currentItem = null; // Remove item from original slot

        // Find player
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
        {
            Debug.LogError("Missing 'Player' tag in the scene.");
            return;
        }

        // Random drop position
        Vector2 dropOffset = Random.insideUnitCircle.normalized * Random.Range(minDropDistance, maxDropDistance);
        Vector2 dropPosition = (Vector2)playerTransform.position + dropOffset; // Random position around player

        // Instantiate drop item
        //Instantiate(gameObject, dropPosition, Quaternion.identity);
        // but now with BOUNCE
        GameObject dropItem = Instantiate(gameObject, dropPosition, Quaternion.identity);
        dropItem.GetComponent<BounceEffect>().StartBounce(); // Start bounce effect

        // Destroy the UI one
        Destroy(gameObject);
    }
}
