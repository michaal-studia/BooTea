using UnityEngine;
using UnityEngine.EventSystems;

public class SlotTooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [SerializeField] private InfoPopup infoPopup;
    [SerializeField] private bool isShopSlot = false; // Ustaw true tylko w prefabie sklepu

    public void OnPointerEnter(PointerEventData eventData)
    {
        Item item = GetCurrentItem();
        if (item != null && infoPopup != null)
            infoPopup.ShowItemInfo(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (infoPopup != null)
            infoPopup.HideItemInfo();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        Item item = GetCurrentItem();
        if (item != null && infoPopup != null)
            infoPopup.FollowMouse();
    }

    private Item GetCurrentItem()
    {
        if (isShopSlot)
            return GetComponent<ShopSlot>()?.itemSO;
        else
        {
            var slot = GetComponent<Slot>();
            if (slot == null || slot.currentItem == null)
                return null;
            return slot.currentItem.GetComponent<Item>();
        }
    }
}