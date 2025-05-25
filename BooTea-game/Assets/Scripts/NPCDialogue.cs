using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    [Header("NPC Info")]
    public string npcName;
    public Sprite npcPortrait;

    [Header("Player Info")]
    public string playerName = "Player";
    public Sprite playerPortrait;

    [Header("Dialogue")]
    public string[] dialogueLines;
    public bool[] isPlayerSpeaking;  // New array to track if player is speaking
    public bool[] autoProgressLines;
    public bool[] endDialogueLines; // mark where dialogue ends

    [Header("Dialogue Settings")]
    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;
    public AudioClip voiceSound;
    public float voicePitch = 1f;
    public AudioClip playerVoiceSound;  // Optional player voice sound
    public float playerVoicePitch = 1f;

    [Header("Choices")]
    public DialogueChoice[] choices;

    public int questInProgressIndex; // index of the quest in progress
    public int questCompletedIndex; // index of the quest completed
    public Quest quest; // Quest to be given or completed
}

[System.Serializable]
public class DialogueChoice
{
    public int dialogueIndex; //dialogue line where choices appear
    public string[] choices; //player response choices
    public int[] nextDialogueIndexes; //where choice leads
    public bool[] givesQuest;
}
