using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private ShopSlot[] shopSlots;

    public void PopulateShopItems(List<ShopItems> shopItems)
    {
        for (int i = 0; i < shopItems.Count && i < shopSlots.Length; i++)
        {
            ShopItems shopItem = shopItems[i];
            shopSlots[i].Initialized(shopItem.itemSO, shopItem.price);
            shopSlots[i].gameObject.SetActive(true);
        }

        for (int i = shopItems.Count; i < shopSlots.Length; i++)
        {
            shopSlots[i].gameObject.SetActive(false);
        }
    }

    public void TryBuyItem(Item itemSO, int price)
    {
        if (itemSO != null)
        {
            if (HasSpaceforItem(itemSO))
            {
                if (InventoryController.Instance.AddItem(itemSO.gameObject))
                {
                    AudioManager.Play("CashRegister");
                    Debug.Log($"Bought item: {itemSO.Name}");
                }
            }
            else
            {
                Debug.Log("No space in inventory!");
            }
        }
    }

    private bool HasSpaceforItem(Item itemSO)
    {
        return InventoryController.Instance.HasFreeSlot();
    }
}

[System.Serializable]
public class ShopItems
{
    public Item itemSO;
    public int price;
}
