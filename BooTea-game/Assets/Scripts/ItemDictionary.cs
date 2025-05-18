using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Security.Cryptography.X509Certificates;

public class ItemDictionary : MonoBehaviour
{
    public List<Item> itemPrefabs;
    private Dictionary<int, GameObject> itemDictionary;
    private void Awake()
    {
        itemDictionary = new Dictionary<int, GameObject>();

        //AutoIncrement ID
        for (int i = 0; i < itemPrefabs.Count; i++)
        {
            if(itemPrefabs[i] != null)
            {
                itemPrefabs[i].ID = i + 1;
                Debug.Log($" - Added item ID {itemPrefabs[i].ID}: {itemPrefabs[i].name}");
            }
        }

        foreach(Item item in itemPrefabs)
        {
            itemDictionary[item.ID] = item.gameObject;
        }
        Debug.Log($"ItemDictionary initialized with {itemDictionary.Count} items");
    }

    public GameObject GetItemPrefab(int itemID)
    {
        itemDictionary.TryGetValue(itemID, out GameObject prefab);
        if (prefab == null)
        {
            Debug.LogWarning($"Item with ID {itemID} not found in the dictionary.");
        }
        return prefab;
    }
}
