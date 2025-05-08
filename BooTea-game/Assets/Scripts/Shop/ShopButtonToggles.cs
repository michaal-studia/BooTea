using UnityEngine;

public class ShopButtonTogles : MonoBehaviour
{
    public void OpenTeaLeavesShop()
    {
        if(ShopKeeper.currentShopKeeper != null)
        {
            ShopKeeper.currentShopKeeper.OpenTeaLeavesShop();
        }
    }
    public void OpenExtrasShop()
    {
        if(ShopKeeper.currentShopKeeper != null)
        {
            ShopKeeper.currentShopKeeper.OpenExtrasShop();
        }
    }

        public void OpenCupsShop()
    {
        if(ShopKeeper.currentShopKeeper != null)
        {
            ShopKeeper.currentShopKeeper.OpenCupsShop();
        }
    }
}    
