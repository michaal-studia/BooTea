using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int ID; 
    public string Name;
    public Sprite itemIcon;

    public virtual void UseItem()
    {
        // Implement item usage logic here
        Debug.Log($"Using item: {Name}");
    }

    public virtual void PickUp()
    {
        //Sprite itemIcon = GetComponent<SpriteRenderer>().sprite;
        Sprite itemIcon = GetComponent<Image>().sprite;
        if (ItemPickupUIController.Instance != null)
        {
            ItemPickupUIController.Instance.ShowItemPickup(Name, itemIcon);
        }
    }
}
