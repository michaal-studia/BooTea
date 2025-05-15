using UnityEngine;
using UnityEngine.UI;

public class CookingPanel : MonoBehaviour
{
    public Slider temperatureSlider;
    public Button startButton;
    public GameObject qtePanel;
    public PointerController pointerController;
    public RectTransform safeZone;
    public StovePanelManager stovePanelManager;

    private int currentTemperature;
    private bool qteResult;

    void Start()
    {
        startButton.onClick.AddListener(StartQTE);
        qtePanel.SetActive(false);
    }

    void Update()
    {
        // Ustaw safe zone w pozycji slidera
        Vector2 sliderPosition = temperatureSlider.handleRect.position;
        safeZone.position = new Vector3(sliderPosition.x, safeZone.position.y, safeZone.position.z);
    }

    public void StartQTE()
    {
        currentTemperature = (int)temperatureSlider.value * 5 + 70;
        temperatureSlider.gameObject.SetActive(false);
        startButton.interactable = false;
        qtePanel.SetActive(true);
        pointerController.ResetPointer();
    }

    public void FinishQTE(bool success)
    {
        qteResult = success;
        temperatureSlider.gameObject.SetActive(true);
        startButton.interactable = true;
        qtePanel.SetActive(false);

        // Wywołaj crafting z wynikiem QTE i temperaturą
        stovePanelManager.TryCraft(currentTemperature, qteResult);
    }
}