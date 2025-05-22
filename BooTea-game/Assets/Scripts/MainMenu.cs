using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject settingsPanel;

    [Header("Other GameObjects")]
    public GameObject leavesSystem; // <- Dodaj obiekt systemu cz¹steczek

    public void OnStartClick()
    {
        AudioManager.Play("LoadGame");
        SceneManager.LoadScene("SampleScene");
    }
    public void OnLoadClick()
    {
        AudioManager.Play("ButtonAffirmative");
    }

    public void OnSettingsClick()
    {

        if (settingsPanel != null)
        {
            bool shouldShow = !settingsPanel.activeSelf;
            if (settingsPanel.activeSelf)
            {
                AudioManager.Play("ButtonDissenting");
            }
            else
            {
                AudioManager.Play("ButtonAffirmative");
            }
            settingsPanel.SetActive(shouldShow);

            // Ukryj lub poka¿ system liœci
            if (leavesSystem != null)
            {
                leavesSystem.SetActive(!shouldShow);
            }
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        AudioManager.Play("ButtonDissenting");
        Application.Quit();
        Debug.Log("Gra zostala zakonczona.");
    }
}
