using UnityEngine;
using System.Collections.Generic;


public class QuestController : MonoBehaviour
{
    public static QuestController Instance { get; private set; }
    public List<QuestProgress> activateQuests = new();
    private QuestUI questUI;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        questUI = FindObjectOfType<QuestUI>();
        InventoryController.Instance.OnInventoryChanged += () => {
            QuestController.Instance.CheckInventoryForQuests();
            questUI.UpdateQuestUI();
        }; // Subscribe to inventory changes
    }

    public void AcceptQuest(Quest quest)
    {
        if (IsQuestActive(quest.QuestID)) return;

        activateQuests.Add(new QuestProgress(quest));
        CheckInventoryForQuests(); // Check inventory for quest items
        questUI.UpdateQuestUI(); 
    }

    public bool IsQuestActive(string questID)=> activateQuests.Exists(q => q.QuestID == questID);

    public bool IsQuestCompleted(string questID)
    {
        var questProgress = activateQuests.Find(q => q.QuestID == questID);
        return questProgress != null && questProgress.isCompleted;
    }

    public void CheckInventoryForQuests()
    {
        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();

        foreach (QuestProgress quest in activateQuests)
        {
            if (quest.isCompleted)
                continue; // Pomijaj ukoñczone questy
            foreach (QuestObjective questObjective in quest.objectives) {
                if (questObjective.type != ObjectiveType.CollectItem) continue;
                if (!int.TryParse(questObjective.objectiveID, out int itemID)) continue;

                int newAmount = itemCounts.TryGetValue(itemID, out int count) ? Mathf.Min(count, questObjective.requiredAmount) : 0;

                if (questObjective.currentAmount != newAmount) 
                {
                    questObjective.currentAmount = newAmount;
                }
            }
        }
        questUI.UpdateQuestUI(); // Update the quest UI after checking inventory
    }
    public void LoadQuestProgress(List<QuestProgress> savedQuests)
    {
        activateQuests = savedQuests ?? new();

        CheckInventoryForQuests(); // Ensure inventory is checked after loading quests
        questUI.UpdateQuestUI(); // Update the quest UI after loading quests
    }

}
