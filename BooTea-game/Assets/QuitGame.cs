using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
        // Dzia�a tylko po zbudowaniu gry (nie zadzia�a w edytorze)
        Application.Quit();
        Debug.Log("Gra zosta�a zako�czona.");
    }
}

