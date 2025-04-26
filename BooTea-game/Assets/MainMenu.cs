using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnStartClick()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Zatrzymuje gre w edytorze
#endif

        Application.Quit();
        Debug.Log("Gra zostala zakonczona.");
    }
}
