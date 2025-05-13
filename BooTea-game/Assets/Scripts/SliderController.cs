using UnityEngine;
using TMPro;

public class SliderController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sliderText = null;

    public void SliderChange(float value)
    {
        float localValue = value;
        sliderText.text = localValue.ToString("0") + "°C";
    }
}
