using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<ShopItems> shopItems;
    [SerializeField] private ShopSlot[] shopSlots;


    private void Start()
    {
        PopulateShopItems();   
    }
    
    public void PopulateShopItems()
    {
        Debug.Log("PopulateShopItems called.");
        for(int i = 0; i < shopItems.Count && i < shopSlots.Length; i++)
        {
            if (shopSlots[i] == null)
            {
                Debug.LogError($"shopSlots[{i}] is null!");
                continue;
            }

            if (shopItems[i].itemSO == null)
            {
                Debug.LogError($"shopItems[{i}].itemSO is null!");
                continue;
            }

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
