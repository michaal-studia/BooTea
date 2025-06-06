using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.C))
        {
            if (!menuCanvas.activeSelf && PauseController.IsGamePaused)
            {
                return; // if game is paused by talking with the npc
            }
            if (menuCanvas.activeSelf)
                AudioManager.Play("MinimizeSwoosh1");
            else
                AudioManager.Play("MaximizeSwoosh1");
            menuCanvas.SetActive(!menuCanvas.activeSelf);
            PauseController.SetPause(menuCanvas.activeSelf);
        }
    }
}
