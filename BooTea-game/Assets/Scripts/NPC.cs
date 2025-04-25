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

    public void Start()
    {
        dialogueUI = DialogueController.Instance;
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
        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
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
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.SetDialogueText("");

        // Type out the current line
        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueUI.SetDialogueText(dialogueUI.dialogueText.text += letter);
            if (dialogueData.voiceSound != null)
            {
                AudioManager.PlayVoice(dialogueData.voiceSound, dialogueData.voicePitch);
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
