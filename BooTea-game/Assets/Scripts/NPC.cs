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

    private enum QuestState { NotStarted, InProgress, Compleated }
    private QuestState questState = QuestState.NotStarted;

    [Header("NPC Settings")]
    public int NPCId;  // Unikalny identyfikator NPC, np. 0 = Cat, 1 = Piano, itd.

    [Header("Player Info")]
    public string playerName = "Player";
    public Sprite playerPortrait;

    [Header("Movement Settings")]
    public WaypointMover waypointMover; // Reference to WaypointMover component

    void Start()
    {
        dialogueUI = DialogueController.Instance;
        indicatorController = GetComponent<NPCIndicatorController>();
        if (indicatorController == null)
        {
            indicatorController = gameObject.AddComponent<NPCIndicatorController>();
        }

        if (waypointMover == null)
            waypointMover = GetComponent<WaypointMover>();

        if (dialogueData.playerName == "Player" && !string.IsNullOrEmpty(playerName))
            dialogueData.playerName = playerName;
        if (dialogueData.playerPortrait == null && playerPortrait != null)
            dialogueData.playerPortrait = playerPortrait;

        if (secondDialogueData != null)
        {
            if (secondDialogueData.playerName == "Player" && !string.IsNullOrEmpty(playerName))
                secondDialogueData.playerName = playerName;
            if (secondDialogueData.playerPortrait == null && playerPortrait != null)
                secondDialogueData.playerPortrait = playerPortrait;
        }
    }

    public bool IsFirstDialogueCompleted() => isFirstDialogueCompleted;
    public bool IsInDialogue() => isDialogueActive;
    public bool canInteract() => !isDialogueActive;

    public void Interact()
    {
        if ((dialogueData == null && !isFirstDialogueCompleted) ||
            (secondDialogueData == null && isFirstDialogueCompleted) ||
            (PauseController.IsGamePaused && !isDialogueActive))
            return;

        if (isDialogueActive)
            NextLine();
        else
            StartDialogue();
    }

    void StartDialogue()
    {
        SyncQuestState();

        if (questState == QuestState.NotStarted)
            dialogueIndex = 0;
        else if (questState == QuestState.InProgress)
            dialogueIndex = dialogueData.questInProgressIndex;
        else // Compleated
            dialogueIndex = dialogueData.questCompletedIndex;

        isDialogueActive = true;
        dialogueUI.ClearChoices();
        indicatorController.HideAllIndicators();

        NPCDialogue currentDialogue = isFirstDialogueCompleted ? secondDialogueData : dialogueData;
        UpdateSpeakerInfo(currentDialogue);

        dialogueUI.ShowDialogueUI(true);
        PauseController.SetPause(true);
        DisplayCurrentLine(currentDialogue);
    }

    private void SyncQuestState()
    {
        if (dialogueData.quest == null) return;
        var questID = dialogueData.quest.QuestID;
        questState = QuestController.Instance.IsQuestActive(questID)
                     ? QuestState.InProgress
                     : QuestState.NotStarted;
    }

    void NextLine()
    {
        NPCDialogue currentDialogue = isFirstDialogueCompleted ? secondDialogueData : dialogueData;

        if (isTyping)
        {
            StopAllCoroutines();
            dialogueUI.SetDialogueText(currentDialogue.dialogueLines[dialogueIndex]);
            isTyping = false;
            return;
        }

        dialogueUI.ClearChoices();

        if (currentDialogue.endDialogueLines.Length > dialogueIndex &&
            currentDialogue.endDialogueLines[dialogueIndex])
        {
            if (!isFirstDialogueCompleted && currentDialogue == dialogueData)
            {
                isFirstDialogueCompleted = true;
                indicatorController.ShowCompletedQuestIndicator();
            }
            else if (isFirstDialogueCompleted && secondDialogueData != null)
            {
                isSecondDialogueCompleted = true;
            }
            EndDialogue();
            return;
        }

        foreach (var choice in currentDialogue.choices)
        {
            if (choice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(choice);
                return;
            }
        }

        if (++dialogueIndex < currentDialogue.dialogueLines.Length)
        {
            UpdateSpeakerInfo(currentDialogue);
            DisplayCurrentLine(currentDialogue);
        }
        else
        {
            if (!isFirstDialogueCompleted && currentDialogue == dialogueData)
            {
                isFirstDialogueCompleted = true;
                indicatorController.ShowCompletedQuestIndicator();
            }
            EndDialogue();
        }
    }

    void UpdateSpeakerInfo(NPCDialogue currentDialogue)
    {
        bool isPlayer = currentDialogue.isPlayerSpeaking.Length > dialogueIndex
                        && currentDialogue.isPlayerSpeaking[dialogueIndex];

        if (isPlayer)
            dialogueUI.SetSpeakerInfo(currentDialogue.playerName, currentDialogue.playerPortrait);
        else
            dialogueUI.SetSpeakerInfo(currentDialogue.npcName, currentDialogue.npcPortrait);
    }

    IEnumerator TypeLine(NPCDialogue currentDialogue)
    {
        isTyping = true;

        if (dialogueIndex >= currentDialogue.dialogueLines.Length)
        {
            Debug.LogError($"[NPC] dialogueIndex ({dialogueIndex}) out of bounds in TypeLine. Lines count: {currentDialogue.dialogueLines.Length}");
            EndDialogue();
            yield break;
        }

        dialogueUI.SetDialogueText("");
        bool isPlayer = currentDialogue.isPlayerSpeaking.Length > dialogueIndex
                        && currentDialogue.isPlayerSpeaking[dialogueIndex];
        AudioClip voiceToUse = isPlayer ? currentDialogue.playerVoiceSound : currentDialogue.voiceSound;
        float pitchToUse = isPlayer ? currentDialogue.playerVoicePitch : currentDialogue.voicePitch;

        foreach (char letter in currentDialogue.dialogueLines[dialogueIndex])
        {
            dialogueUI.SetDialogueText(dialogueUI.dialogueTextRef.text + letter);
            if (voiceToUse != null)
                AudioManager.PlayVoice(voiceToUse, pitchToUse);
            yield return new WaitForSeconds(currentDialogue.typingSpeed);
        }

        isTyping = false;
        if (currentDialogue.autoProgressLines.Length > dialogueIndex
            && currentDialogue.autoProgressLines[dialogueIndex])
        {
            NextLine();
        }
    }

    void DisplayChoices(DialogueChoice choice)
    {
        for (int i = 0; i < choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndexes[i];
            bool givesQuest = choice.givesQuest[i];
            int musicChoiceIdx = i;

            dialogueUI.CreateChoiceButton(choice.choices[i], () =>
                ChooseOption(nextIndex, musicChoiceIdx, givesQuest)
            );
        }
    }

    void ChooseOption(int nextIndex, int musicChoiceIndex, bool givesQuest)
    {
        if (givesQuest)
        {
            QuestController.Instance.AcceptQuest(dialogueData.quest);
            questState = QuestState.InProgress;
        }

        dialogueIndex = nextIndex;
        NPCDialogue currentDialogue = isFirstDialogueCompleted ? secondDialogueData : dialogueData;

        if (dialogueIndex >= currentDialogue.dialogueLines.Length)
        {
            Debug.LogError($"[NPC] nextIndex ({dialogueIndex}) out of bounds in ChooseOption. Lines count: {currentDialogue.dialogueLines.Length}");
            EndDialogue();
            return;
        }

        dialogueUI.ClearChoices();

        if (NPCId == 1)
        {
            Debug.Log(musicChoiceIndex);
            var pianoNPC = Object.FindFirstObjectByType<PianoNPC>();
            pianoNPC.PlayBackgroundMusicForChoice(musicChoiceIndex);
        }

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
        waypointMover?.StartReverseMovement();
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
                indicatorController.ShowEmoteAfter2ndDialogue();
                StartReverseMovement();
            }
        }
        else if (isFirstDialogueCompleted && secondDialogueData == null)
        {
            isFirstDialogueCompleted = false;
        }
        else
        {
            if (isFirstDialogueCompleted)
                indicatorController.ShowCompletedQuestIndicator();
            else
                indicatorController.ShowInitialQuestIndicator();
        }
    }
}