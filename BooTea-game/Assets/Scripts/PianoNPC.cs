using UnityEngine;

public class PianoNPC : MonoBehaviour
{
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.Instance;
    }

    public void PlayBackgroundMusicForChoice(int choiceIndex)
    {
        AudioClip[] PianoNPCMusicChoices = { audioManager.backgroundMusic1, audioManager.backgroundMusic2, audioManager.backgroundMusic3, audioManager.backgroundMusic4 };

        if (choiceIndex >= 0 && choiceIndex < PianoNPCMusicChoices.Length)
        {
            AudioClip selectedClip = PianoNPCMusicChoices[choiceIndex];
            audioManager.PlayBackgroundMusic(selectedClip);
        }
        else
        {
            Debug.LogWarning("Nieprawid³owy wybór muzyki!");
        }
    }
}