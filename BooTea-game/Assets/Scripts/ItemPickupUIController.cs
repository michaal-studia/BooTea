using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ItemPickupUIController : MonoBehaviour
{
    public static ItemPickupUIController Instance { get; private set; }

    public GameObject popupPrefab; // Prefab for the item pickup UI
    public int maxPopups = 5; // Maximum number of popups to display at once
    public float popupDuration = 3f; // Duration for which each popup is displayed

    private readonly Queue<GameObject> activePopups = new(); // Queue to manage active popups

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple ItemPickupUIManager instances detected! Destroying the extra one.");
            Destroy(gameObject); // Ensure only one instance of the controller exists
        }
    }

    public void ShowItemPickup(string itemName, Sprite itemIcon)
    {
        GameObject newPopup = Instantiate(popupPrefab, transform); // Create a new popup instance
        newPopup.GetComponentInChildren<TMP_Text>().text = itemName; // Set the item name text

        Image itemImage = newPopup.transform.Find("ItemIcon")?.GetComponent<Image>(); // Get the Image component
        if(itemImage)
        {
            itemImage.sprite = itemIcon; // Set the item icon sprite
        }
        
        activePopups.Enqueue(newPopup); // Add the new popup to the queue
        if(activePopups.Count > maxPopups) // Check if the maximum number of popups is exceeded
        {
            Destroy(activePopups.Dequeue()); // Remove the oldest popup
        }

        //Fade out and destroy
        StartCoroutine(FadeOutAndDestroy(newPopup)); // Start the coroutine to fade out and destroy the popup
    }

    private IEnumerator FadeOutAndDestroy(GameObject popup)
    {
        // Implement fade out effect here if needed
        yield return new WaitForSeconds(popupDuration); // Wait for the specified duration
        if(popup == null) yield break; // Check if the popup is still valid

        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>(); // Get the CanvasGroup component
        for(float timePassed = 0f; timePassed < 1f; timePassed += Time.deltaTime)
        {
            if(popup == null) yield break; // Check if the popup is still valid
            canvasGroup.alpha = 1f - timePassed; // Gradually reduce the alpha value for fade out
            yield return null; // Wait for the next frame
        }

        Destroy(popup); // Destroy the popup after fading out
    }
}
