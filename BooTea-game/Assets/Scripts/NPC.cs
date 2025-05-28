using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;

    private DialogueController dialogueUI;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;
    private NPCIndicatorController indicatorController;
    private bool isDialogueCompleted = false;

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
    }

    public bool IsDialogueCompleted() => isDialogueCompleted;
    public bool IsInDialogue() => isDialogueActive;
    public bool canInteract() => !isDialogueActive;

    public void Interact()
    {
        if (dialogueData == null || (PauseController.IsGamePaused && !isDialogueActive))
            return;

        // First check and update quest state if needed
        if (dialogueData.quest != null && questState == QuestState.InProgress)
        {
            var questProgress = QuestController.Instance.LoadQuestProgress(dialogueData.quest.QuestID);
            if (questProgress != null)
            {
                bool allOtherObjectivesComplete = true;
                foreach (var obj in questProgress.objectives)
                {
                    if (obj.type == ObjectiveType.TalkNPC) continue;
                    if (!obj.isCompleted)
                    {
                        allOtherObjectivesComplete = false;
                        break;
                    }
                }

                if (allOtherObjectivesComplete)
                {
                    foreach (var obj in questProgress.objectives)
                    {
                        if (obj.type == ObjectiveType.TalkNPC && obj.currentAmount < obj.requiredAmount)
                        {
                            obj.currentAmount = obj.requiredAmount;
                            QuestController.Instance.UpdateQuestProgress(dialogueData.quest);
                            questState = QuestState.Compleated;
                            break;
                        }
                    }
                }
            }
        }

        // Then sync the quest state again to get the updated state
        SyncQuestState();

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

        UpdateSpeakerInfo(dialogueData);

        dialogueUI.ShowDialogueUI(true);
        PauseController.SetPause(true);
        DisplayCurrentLine(dialogueData);
    }

    private void SyncQuestState()
    {
        if (dialogueData.quest == null) return;

        var questID = dialogueData.quest.QuestID;

        // Check if quest is completed first
        if (QuestController.Instance.IsQuestCompleted(questID))
        {
            questState = QuestState.Compleated;
        }
        // Then check if quest is active (in progress)
        else if (QuestController.Instance.IsQuestActive(questID))
        {
            questState = QuestState.InProgress;
        }
        // Otherwise it's not started
        else
        {
            questState = QuestState.NotStarted;
        }
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

        dialogueUI.ClearChoices();

        if (dialogueData.endDialogueLines.Length > dialogueIndex &&
            dialogueData.endDialogueLines[dialogueIndex])
        {
            if (!isDialogueCompleted)
            {
                isDialogueCompleted = true;
                indicatorController.ShowCompletedQuestIndicator();
            }
            EndDialogue();
            return;
        }

        foreach (var choice in dialogueData.choices)
        {
            if (choice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(choice);
                return;
            }
        }

        if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            UpdateSpeakerInfo(dialogueData);
            DisplayCurrentLine(dialogueData);
        }
        else
        {
            if (!isDialogueCompleted)
            {
                isDialogueCompleted = true;
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
            // Add bounds checking to prevent IndexOutOfRangeException
            int nextIndex = i < choice.nextDialogueIndexes.Length ? choice.nextDialogueIndexes[i] : 0;
            bool givesQuest = i < choice.givesQuest.Length ? choice.givesQuest[i] : false;
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

        if (dialogueIndex >= dialogueData.dialogueLines.Length)
        {
            Debug.LogError($"[NPC] nextIndex ({dialogueIndex}) out of bounds in ChooseOption. Lines count: {dialogueData.dialogueLines.Length}");
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

        UpdateSpeakerInfo(dialogueData);
        DisplayCurrentLine(dialogueData);
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

        if (isDialogueCompleted && questState == QuestState.Compleated)
        {
            indicatorController.HideAllIndicators();
            indicatorController.ShowEmoteAfter2ndDialogue();
            StartReverseMovement();
        }
    }
}