using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Zatrzymuje gre w edytorze
#endif

        Application.Quit();
        Debug.Log("Gra zostala zakonczona.");
    }
}

