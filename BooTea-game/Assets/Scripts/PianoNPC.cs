using UnityEngine;

public class PianoNPC : MonoBehaviour
{
    public void PlayBackgroundMusicForChoice(int choiceIndex)
    {
        AudioClip[] PianoNPCMusicChoices = AudioManager.GetAudioClipsFromLibrary("BackgroundMusic");

        if (choiceIndex >= 0 && choiceIndex < PianoNPCMusicChoices.Length)
        {
            AudioClip selectedClip = PianoNPCMusicChoices[choiceIndex];
            AudioManager.PlayBackgroundMusic(selectedClip);
        }
        else
        {
            Debug.LogWarning("Nieprawid³owy wybór muzyki!");
        }
    }
}
