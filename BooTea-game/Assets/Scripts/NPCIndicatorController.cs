using System.Collections;
using UnityEngine;

public class NPCIndicatorController : MonoBehaviour
{
    public GameObject yellowQuestionMark;
    public GameObject greyQuestionMark;
    public GameObject emoteAfter2ndDialogue;

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

    public void ShowEmoteAfter2ndDialogue()
    {
        StartCoroutine(ShowAndHideEmote());   
    }

    private IEnumerator ShowAndHideEmote()
    {
        if (emoteAfter2ndDialogue != null)
        {
            emoteAfter2ndDialogue.SetActive(true);
            yield return new WaitForSeconds(2f);
            emoteAfter2ndDialogue.SetActive(false);
        }
    }
}