using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider voiceSlider;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.AssignSliders(musicSlider, sfxSlider, voiceSlider);
        }
    }
}