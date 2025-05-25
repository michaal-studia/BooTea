using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider voiceSlider;

    private void OnEnable()
    {
        if (AudioManager.Instance != null && musicSlider != null && sfxSlider != null && voiceSlider != null)
        {
            AudioManager.Instance.AssignSliders(musicSlider, sfxSlider, voiceSlider);
        }
    }
}