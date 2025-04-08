using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
        // Dzia³a tylko po zbudowaniu gry (nie zadzia³a w edytorze)
        Application.Quit();
        Debug.Log("Gra zosta³a zakoñczona.");
    }
}

