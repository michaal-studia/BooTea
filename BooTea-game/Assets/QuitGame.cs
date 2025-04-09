using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Zatrzymuje gr� w edytorze
#endif

        Application.Quit();
        Debug.Log("Gra zosta�a zako�czona.");
    }
}

