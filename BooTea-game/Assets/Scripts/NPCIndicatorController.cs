using UnityEngine;

public class NPCIndicatorController : MonoBehaviour
{
    public GameObject yellowQuestionMark;
    public GameObject greyQuestionMark;

    public void ShowInitialQuestIndicator()
    {
        if (yellowQuestionMark != null)
        {
            yellowQuestionMark.SetActive(true);
        }
        if (greyQuestionMark != null)
        {
            greyQuestionMark.SetActive(false);
        }
    }

    public void ShowCompletedQuestIndicator()
    {
        if (yellowQuestionMark != null)
        {
            yellowQuestionMark.SetActive(false);
        }
        if (greyQuestionMark != null)
        {
            greyQuestionMark.SetActive(true);
        }
    }

    public void HideAllIndicators()
    {
        if (yellowQuestionMark != null)
        {
            yellowQuestionMark.SetActive(false);
        }
        if (greyQuestionMark != null)
        {
            greyQuestionMark.SetActive(false);
        }
    }
}