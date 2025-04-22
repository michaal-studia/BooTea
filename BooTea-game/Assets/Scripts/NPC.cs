using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;

    private DialogueController dialogueUI;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    [Header("NPC Settings")]
    public int NPCId;  // Unikalny identyfikator NPC, np. 0 = Cat, 1 = Piano, itd.

    [Header("Player Info")]
    public string playerName = "Player";
    public Sprite playerPortrait;

    public void Start()
    {
        dialogueUI = DialogueController.Instance;

        // Use default player info if not set in dialogue data
        if (dialogueData.playerName == "Player" && !string.IsNullOrEmpty(playerName))
            dialogueData.playerName = playerName;

        if (dialogueData.playerPortrait == null && playerPortrait != null)
            dialogueData.playerPortrait = playerPortrait;
    }

    public bool canInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if (dialogueData == null || (PauseController.IsGamePaused && !isDialogueActive))
            return;

        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;
        dialogueUI.ClearChoices();

        // Set initial speaker
        UpdateSpeakerInfo();

        dialogueUI.ShowDialogueUI(true);
        PauseController.SetPause(true);

        DisplayCurrentLine();
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
            return;
        }

        //Clear choices
        dialogueUI.ClearChoices();
        //Check endDialogueLines
        if (dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }
        //Check if choices & display
        foreach (DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(dialogueChoice);
                return;
            }
        }

        if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            // Update speaker info for the new line
            UpdateSpeakerInfo();
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    // New method to update the speaker information based on current line
    void UpdateSpeakerInfo()
    {
        bool isPlayer = dialogueData.isPlayerSpeaking.Length > dialogueIndex && dialogueData.isPlayerSpeaking[dialogueIndex];

        if (isPlayer)
        {
            dialogueUI.SetSpeakerInfo(dialogueData.playerName, dialogueData.playerPortrait);
        }
        else
        {
            dialogueUI.SetSpeakerInfo(dialogueData.npcName, dialogueData.npcPortrait);
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.SetDialogueText("");

        // Get whether the current speaker is player or NPC
        bool isPlayer = dialogueData.isPlayerSpeaking.Length > dialogueIndex && dialogueData.isPlayerSpeaking[dialogueIndex];
        AudioClip voiceToUse = isPlayer ? dialogueData.playerVoiceSound : dialogueData.voiceSound;
        float pitchToUse = isPlayer ? dialogueData.playerVoicePitch : dialogueData.voicePitch;

        // Type out the current line
        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueUI.SetDialogueText(dialogueUI.dialogueText.text += letter);
            if (voiceToUse != null)
            {
                float variedPitch = pitchToUse * Random.Range(0.95f, 1.15f);
                AudioManager.Instance.PlayVoice(voiceToUse, variedPitch);
            }
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            // Only show choices if not auto-progressing
            NextLine();
        }
    }

    void DisplayChoices(DialogueChoice choice)
    {
        for (int i = 0; i < choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndexes[i];
            int musicChoiceIndex = i;
            dialogueUI.CreateChoiceButton(choice.choices[i], () => ChooseOption(nextIndex, musicChoiceIndex));
        }
    }

    void ChooseOption(int nextIndex, int musicChoiceIndex)
    {
        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();

        if (NPCId == 1) // if Piano is interacting with us we want to change music based on the clicked option that we were given
        {
            Debug.Log(musicChoiceIndex);
            PianoNPC pianoNPC = FindFirstObjectByType<PianoNPC>();
            pianoNPC.PlayBackgroundMusicForChoice(musicChoiceIndex);
        }

        // Update speaker info for the new dialogue line
        UpdateSpeakerInfo();
        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
        PauseController.SetPause(false);
    }
}