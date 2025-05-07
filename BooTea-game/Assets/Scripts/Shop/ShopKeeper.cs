using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    public static ShopKeeper currentShopKeeper;
    public Animator anim;
    public CanvasGroup shopCanvasGroup;
    public ShopManager shopManager;

    [SerializeField] private List<ShopItems> shopTeaLeaves;
    [SerializeField] private List<ShopItems> shopExtras;

    public static event Action<ShopManager, bool> OnShopStateChanged;
    private bool playerInRange;
    private bool isShopOpen;

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (!isShopOpen)
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
            playerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("playerInRange", false);
            playerInRange = false;
        }
    }
}
