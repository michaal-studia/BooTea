using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    
    [SerializeField] private ShopSlot[] shopSlots;
    
    public void PopulateShopItems(List<ShopItems> shopItems)
    {
        for(int i = 0; i < shopItems.Count && i < shopSlots.Length; i++)
        {
            ShopItems shopItem = shopItems[i];
            shopSlots[i].Initialized(shopItem.itemSO, shopItem.price);
            shopSlots[i].gameObject.SetActive(true);
        }

        for(int i = shopItems.Count; i < shopSlots.Length; i++)
        {
            shopSlots[i].gameObject.SetActive(false);
        }

    }

}

[System.Serializable]
public class ShopItems
{
    public Item itemSO;
    public int price;
}
