using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public Item itemSO;
    public TMP_Text itemNameText;
    public TMP_Text priceText;
    public Image itemImage;

    private int price;

    public void Initialized(Item newItemSO, int price)
    {
        if (itemNameText == null) Debug.LogError("itemNameText is not assigned in ShopSlot!");
        if (priceText == null) Debug.LogError("priceText is not assigned in ShopSlot!");
        if (itemImage == null) Debug.LogError("itemImage is not assigned in ShopSlot!");

        // Fill the slot with information
        itemSO = newItemSO;
        itemNameText.text = itemSO.Name;
        itemImage.sprite = itemSO.itemIcon;
        this.price = price;
        priceText.text = price.ToString();
    }
}
