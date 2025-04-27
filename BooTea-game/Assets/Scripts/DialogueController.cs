using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; } //Singleton instance
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;

    // Make dialogueText publicly accessible for the NPC class
    [HideInInspector] public TMP_Text dialogueTextRef => dialogueText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); //Make sure only one instance
    }

    public void ShowDialogueUI(bool show)
    {
        dialoguePanel.SetActive(show);//Toggle UI visabillity
    }

    // Renamed from SetNPCInfo to SetSpeakerInfo to be more generic
    public void SetSpeakerInfo(string speakerName, Sprite portrait)
    {
        nameText.text = speakerName;
        portraitImage.sprite = portrait;
    }

    public void SetDialogueText(string text)
    {
        dialogueText.text = text;
    }

    public void ClearChoices()
    {
        foreach (Transform child in choiceContainer) Destroy(child.gameObject);
    }

    public void CreateChoiceButton(string choiceText, UnityEngine.Events.UnityAction onClick)
    {
        GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceContainer);
        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText;
        choiceButton.GetComponent<Button>().onClick.AddListener(onClick);
    }
}