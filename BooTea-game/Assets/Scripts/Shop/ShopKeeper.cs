using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour, IInteractable
{
    public static ShopKeeper currentShopKeeper;
    public Animator anim;
    public CanvasGroup shopCanvasGroup;
    public ShopManager shopManager;

    [SerializeField] private List<ShopItems> shopTeaLeaves;
    [SerializeField] private List<ShopItems> shopExtras;

    public static event Action<ShopManager, bool> OnShopStateChanged;

    private bool isShopOpen;

    public bool canInteract()
    {
<<<<<<< HEAD
        if (playerInRange)
=======
        return true; // Mo¿esz tu dodaæ warunki, np. czy sklep nie jest zablokowany
    }

    public void Interact()
    {
        if (!isShopOpen)
>>>>>>> 7d03565a755505e971121084bfae9738c172e7f7
        {
            Time.timeScale = 0;
            currentShopKeeper = this;
            isShopOpen = true;
            OnShopStateChanged?.Invoke(shopManager, true);

            shopCanvasGroup.alpha = 1;
            shopCanvasGroup.interactable = true;
            shopCanvasGroup.blocksRaycasts = true;

            OpenTeaLeavesShop();
        }
        else
        {
            Time.timeScale = 1;
            currentShopKeeper = null;
            isShopOpen = false;
            OnShopStateChanged?.Invoke(shopManager, false);

            shopCanvasGroup.alpha = 0;
            shopCanvasGroup.interactable = false;
            shopCanvasGroup.blocksRaycasts = false;
        }
    }

    public void OpenTeaLeavesShop()
    {
        shopManager.PopulateShopItems(shopTeaLeaves);
    }

    public void OpenExtrasShop()
    {
        shopManager.PopulateShopItems(shopExtras);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("playerInRange", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("playerInRange", false);
        }
    }
}
