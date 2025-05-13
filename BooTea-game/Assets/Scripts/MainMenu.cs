using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnStartClick()
    {
        AudioManager.Play("LoadGame");
        SceneManager.LoadScene("SampleScene");
    }

    public void OnSettingsClick()
    {
        AudioManager.Play("ButtonAffirmative");
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Zatrzymuje gre w edytorze
#endif
        AudioManager.Play("ButtonDissenting");
        Application.Quit();
        Debug.Log("Gra zostala zakonczona.");
    }
}
