using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    public NPCDialogue secondDialogueData; // Dialogue to use after first interaction is complete

    private DialogueController dialogueUI;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;
    private NPCIndicatorController indicatorController;
    private bool isFirstDialogueCompleted = false;
    private bool isSecondDialogueCompleted = false;

    [Header("NPC Settings")]
    public int NPCId;  // Unikalny identyfikator NPC, np. 0 = Cat, 1 = Piano, itd.

    [Header("Player Info")]
    public string playerName = "Player";
    public Sprite playerPortrait;

    [Header("Movement Settings")]
    public WaypointMover waypointMover; // Reference to WaypointMover component

    public void Start()
    {
        dialogueUI = DialogueController.Instance;
        indicatorController = GetComponent<NPCIndicatorController>();
        if (indicatorController == null)
        {
            indicatorController = gameObject.AddComponent<NPCIndicatorController>();
        }

        // Check if this NPC has a WaypointMover
        if (waypointMover == null)
        {
            waypointMover = GetComponent<WaypointMover>();
        }

        // Use default player info if not set in dialogue data
        if (dialogueData.playerName == "Player" && !string.IsNullOrEmpty(playerName))
            dialogueData.playerName = playerName;

        if (dialogueData.playerPortrait == null && playerPortrait != null)
            dialogueData.playerPortrait = playerPortrait;

        // Also set for second dialogue if present
        if (secondDialogueData != null)
        {
            if (secondDialogueData.playerName == "Player" && !string.IsNullOrEmpty(playerName))
                secondDialogueData.playerName = playerName;

            if (secondDialogueData.playerPortrait == null && playerPortrait != null)
                secondDialogueData.playerPortrait = playerPortrait;
        }
    }

    // Public accessor method for dialogue completion status
    public bool IsFirstDialogueCompleted()
    {
        return isFirstDialogueCompleted;
    }

    // Public accessor to check if dialogue is active
    public bool IsInDialogue()
    {
        return isDialogueActive;
    }

    public bool canInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if ((dialogueData == null && !isFirstDialogueCompleted) ||
            (secondDialogueData == null && isFirstDialogueCompleted) ||
            (PauseController.IsGamePaused && !isDialogueActive))
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

        // Hide indicators during dialogue
        indicatorController.HideAllIndicators();

        // Choose which dialogue to use based on completion state
        NPCDialogue currentDialogue = isFirstDialogueCompleted ? secondDialogueData : dialogueData;

        // Set initial speaker
        UpdateSpeakerInfo(currentDialogue);

        dialogueUI.ShowDialogueUI(true);
        PauseController.SetPause(true);

        DisplayCurrentLine(currentDialogue);
    }

    void NextLine()
    {
        // Choose which dialogue to use based on completion state
        NPCDialogue currentDialogue = isFirstDialogueCompleted ? secondDialogueData : dialogueData;

        if (isTyping)
        {
            StopAllCoroutines();
            dialogueUI.SetDialogueText(currentDialogue.dialogueLines[dialogueIndex]);
            isTyping = false;
            return;
        }

        //Clear choices
        dialogueUI.ClearChoices();

        //Check endDialogueLines
        if (currentDialogue.endDialogueLines.Length > dialogueIndex && currentDialogue.endDialogueLines[dialogueIndex])
        {
            // If this is the first dialogue and it's ending
            if (!isFirstDialogueCompleted && currentDialogue == dialogueData)
            {
                isFirstDialogueCompleted = true;
                // Show grey question mark after first dialogue completion
                indicatorController.ShowCompletedQuestIndicator();
            }
            else if (isFirstDialogueCompleted && secondDialogueData != null)
            {
                isSecondDialogueCompleted = true;
            }
            EndDialogue();
            return;
        }

        //Check if choices & display
        foreach (DialogueChoice dialogueChoice in currentDialogue.choices)
        {
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(dialogueChoice);
                return;
            }
        }

        if (++dialogueIndex < currentDialogue.dialogueLines.Length)
        {
            // Update speaker info for the new line
            UpdateSpeakerInfo(currentDialogue);
            DisplayCurrentLine(currentDialogue);
        }
        else
        {
            // If we've reached the end of dialogue lines without hitting a specifically marked end
            if (!isFirstDialogueCompleted && currentDialogue == dialogueData)
            {
                isFirstDialogueCompleted = true;
                // Show grey question mark after first dialogue completion
                indicatorController.ShowCompletedQuestIndicator();
            }
            EndDialogue();
        }
    }

    // Updated to use the current dialogue
    void UpdateSpeakerInfo(NPCDialogue currentDialogue)
    {
        bool isPlayer = currentDialogue.isPlayerSpeaking.Length > dialogueIndex && currentDialogue.isPlayerSpeaking[dialogueIndex];

        if (isPlayer)
        {
            dialogueUI.SetSpeakerInfo(currentDialogue.playerName, currentDialogue.playerPortrait);
        }
        else
        {
            dialogueUI.SetSpeakerInfo(currentDialogue.npcName, currentDialogue.npcPortrait);
        }
    }

    IEnumerator TypeLine(NPCDialogue currentDialogue)
    {
        isTyping = true;
        dialogueUI.SetDialogueText("");

        // Get whether the current speaker is player or NPC
        bool isPlayer = currentDialogue.isPlayerSpeaking.Length > dialogueIndex && currentDialogue.isPlayerSpeaking[dialogueIndex];
        AudioClip voiceToUse = isPlayer ? currentDialogue.playerVoiceSound : currentDialogue.voiceSound;
        float pitchToUse = isPlayer ? currentDialogue.playerVoicePitch : currentDialogue.voicePitch;

        // Type out the current line
        foreach (char letter in currentDialogue.dialogueLines[dialogueIndex])
        {
            dialogueUI.SetDialogueText(dialogueUI.dialogueTextRef.text += letter);
            if (voiceToUse != null)
            {
                float variedPitch = pitchToUse * Random.Range(0.95f, 1.15f);
                AudioManager.Instance.PlayVoice(voiceToUse, variedPitch);
            }
            yield return new WaitForSeconds(currentDialogue.typingSpeed);
        }

        isTyping = false;

        if (currentDialogue.autoProgressLines.Length > dialogueIndex && currentDialogue.autoProgressLines[dialogueIndex])
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
        NPCDialogue currentDialogue = isFirstDialogueCompleted ? secondDialogueData : dialogueData;
        UpdateSpeakerInfo(currentDialogue);
        DisplayCurrentLine(currentDialogue);
    }

    void DisplayCurrentLine(NPCDialogue currentDialogue)
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine(currentDialogue));
    }

    public void StartReverseMovement()
    {
        if (waypointMover != null)
        {
            waypointMover.StartReverseMovement();
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
        PauseController.SetPause(false);

        if (isFirstDialogueCompleted && secondDialogueData)
        {
            if (isSecondDialogueCompleted)
            {
                indicatorController.HideAllIndicators();
                StartReverseMovement();
            }
        }

        // If there's no second dialogue, reset to first dialogue
        else if (isFirstDialogueCompleted && secondDialogueData == null)
        {
            isFirstDialogueCompleted = false;
        }
        else
        {
            // Show appropriate indicator after dialogue ends
            if (isFirstDialogueCompleted)
            {
                indicatorController.ShowCompletedQuestIndicator();
            }
            else
            {
                indicatorController.ShowInitialQuestIndicator();
            }
        }
    }
}