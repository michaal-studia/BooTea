using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public Transform questListContent;
    public GameObject questEntryPrefab;
    public GameObject objectiveTextPrefab;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateQuestUI();
    }

    // Update is called once per frame
    public void UpdateQuestUI()
    {
        foreach (Transform child in questListContent)
        {
            Destroy(child.gameObject);
        }
        foreach (var quest in QuestController.Instance.activateQuests)
        {
            GameObject entry = Instantiate(questEntryPrefab, questListContent);
            TMP_Text questNameText = entry.transform.Find("QuestNameText").GetComponent<TMP_Text>();
            Transform objectiveList = entry.transform.Find("ObjectiveList");

            questNameText.text = quest.quest.QuestName;

            // WYSZARZANIE i komunikat
            if (quest.isCompleted)
            {
                // Wyszarz tekst questa
                questNameText.color = Color.gray;

                // Dodaj komunikat o ukoñczeniu
                GameObject objTextGO = Instantiate(objectiveTextPrefab, objectiveList);
                TMP_Text objText = objTextGO.GetComponent<TMP_Text>();
                objText.text = "<b>Quest ukoñczony!</b>";
                objText.color = Color.green;
            }

            foreach (var objective in quest.objectives)
            {
                GameObject objTextGO = Instantiate(objectiveTextPrefab, objectiveList);
                TMP_Text objText = objTextGO.GetComponent<TMP_Text>();
                objText.text = $"{objective.description} ({objective.currentAmount}/{objective.requiredAmount})";
                if (quest.isCompleted)
                    objText.color = Color.gray; // Wyszarz cele
            }
        }
    }
}
