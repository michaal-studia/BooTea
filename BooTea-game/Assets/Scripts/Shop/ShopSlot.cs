using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public Item itemSO;
    public TMP_Text itemNameText;
    public TMP_Text priceText;
    public Image itemImage;

    [SerializeField] private ShopManager shopManager;
    [SerializeField] private InfoPopup infoPopup;
    private int price;

    public void Initialized(Item newItemSO, int price)
    {
        // Fill the slot with information
        itemSO = newItemSO;
        itemNameText.text = itemSO.Name;
        itemImage.sprite = itemSO.itemIcon;
        this.price = price;
        priceText.text = price.ToString();
    }

    public void OnBuyButtonClicked()
    {
        shopManager.TryBuyItem(itemSO, price);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(itemSO != null)
        {
            infoPopup.ShowItemInfo(itemSO);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        infoPopup.HideItemInfo();
    }
    public void OnPointerMove(PointerEventData eventData)
    {
        if(itemSO != null)
        {
            infoPopup.FollowMouse();
        }
    }
}
