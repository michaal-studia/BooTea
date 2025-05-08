using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public Item itemSO;
    public TMP_Text itemNameText;
    public TMP_Text priceText;
    public Image itemImage;

    [SerializeField] private ShopManager shopManager;
    private int price;

    public void Initialized(Item newItemSO/*, int price*/, int price)
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
}
