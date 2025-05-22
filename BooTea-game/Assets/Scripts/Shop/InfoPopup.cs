using NUnit.Framework.Interfaces;
using TMPro;
using System.Collections;
using UnityEngine;

public class InfoPopup : MonoBehaviour
{
    public CanvasGroup infoPanel;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;

    private RectTransform infoPanelRect;

    private void Awake()
    {
        infoPanelRect = GetComponent<RectTransform>();
    }

    public void ShowItemInfo(Item itemSO)
    {
        infoPanel.alpha = 1;
        itemNameText.text = itemSO.Name;
        itemDescriptionText.text = itemSO.Description;
    }
    public void HideItemInfo()
    {
        infoPanel.alpha = 0;

        itemNameText.text = "";
        itemDescriptionText.text = "";
    }
    public void FollowMouse()
    {
        Vector3 mousePoiition = Input.mousePosition;
        Vector3 offset = new Vector3(10, -10, 0);

        infoPanelRect.position = mousePoiition + offset;
    }
        
}