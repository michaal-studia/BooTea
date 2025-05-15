using UnityEngine;
using TMPro;

public class SliderController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sliderText = null;

    public void SliderChange(float value)
    {
        float localValue = value * 5 + 70;
        sliderText.text = localValue.ToString("0") + "°C";
    }
}
