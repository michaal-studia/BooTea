using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
[CreateAssetMenu(menuName="Quests/Quest")]
public class Quest : ScriptableObject
{
    public string QuestID; 
    public string QuestName;
    public string description;
    public List<QuestObjective> objectives;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(QuestID))
        {
            QuestID = System.Guid.NewGuid().ToString();
        }
    }
}
[System.Serializable]
public class QuestObjective
{
    public string objectiveID;
    public string description;
    public ObjectiveType type;
    public int requiredAmount;
    public int currentAmount;

    public bool isCompleted => currentAmount >= requiredAmount;
}
public enum ObjectiveType
{
    CollectItem,
    ReachLocation,
    TalkNPC,
    Craft,
    Explore,
    Custom
}

[System.Serializable]
public class QuestProgress
{
    public Quest quest;
    public List<QuestObjective> objectives;

    public QuestProgress(Quest quest)
    {
        this.quest = quest;
        objectives = new List<QuestObjective>(quest.objectives);

        foreach (var obj in quest.objectives)
        {
            objectives.Add(new QuestObjective
            {
                objectiveID = obj.objectiveID,
                description = obj.description,
                type = obj.type,
                requiredAmount = obj.requiredAmount,
                currentAmount = 0
            }); // Initialize current amount to 0
        }
    }
    public bool isCompleted => objectives.TrueForAll(o => o.isCompleted);

    public string QuestID => quest.QuestID;
}
