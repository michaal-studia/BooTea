using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public Image[] tabImages;
    public GameObject[] pages;
    private int lastActiveTab = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        ActivateTab(0);
    }

    public void ActivateTab(int tabNO)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            tabImages[i].color = Color.gray;
        }
        if (lastActiveTab != tabNO)
        {
            AudioManager.Play("MaximizeSwoosh2");
        }
        pages[tabNO].SetActive(true);
        lastActiveTab = tabNO;
        tabImages[tabNO].color = Color.white;
    }
}
