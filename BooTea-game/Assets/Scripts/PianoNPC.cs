using UnityEngine;

public class PianoNPC : MonoBehaviour
{
    private AudioManager audioManager;

    [Header("Background Music for Choices")]
    public AudioClip[] backgroundMusicChoices;
    private int currentChoiceIndex = -1;

    private void Start()
    {
        audioManager = AudioManager.Instance;
    }

    public void OnChoiceMade(int choiceIndex)
    {
        if (choiceIndex >= 0 && choiceIndex < backgroundMusicChoices.Length)
        {
            currentChoiceIndex = choiceIndex;
            audioManager.StopMusic();
            audioManager.PlayBackgroundMusic(backgroundMusicChoices[currentChoiceIndex]);
        }
    }
}