using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject settingsPanel;
    public GameObject loadUIPanel;

    [Header("Other GameObjects")]
    public GameObject leavesSystem; // <- Dodaj obiekt systemu cz¹steczek

    [Header("Save System")]
    public MainMenuSaveController saveController;

    public void OnStartClick()
    {
        // Clear any pending loads for a fresh start
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ClearPendingLoad();
        }

        AudioManager.Play("LoadGame");
        SceneManager.LoadScene("SampleScene");
    }
    public void OnLoadClick()
    {
        if (saveController != null)
        {
            saveController.OpenLoadUI();
        }
        else
        {
            AudioManager.Play("ButtonAffirmative");
            Debug.LogWarning("MainMenuSaveController not assigned!");
        }
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
