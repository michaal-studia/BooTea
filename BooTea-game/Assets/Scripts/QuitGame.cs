using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
        AudioManager.Play("ButtonDissenting");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Zatrzymuje gre w edytorze
                                                         //#else
                                                         //SceneManager.LoadScene("StartScene");
#endif

        Application.Quit();
        Debug.Log("Gra zostala zakonczona.");
    }
}

