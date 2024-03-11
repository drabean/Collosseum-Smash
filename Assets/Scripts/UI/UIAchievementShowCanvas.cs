using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIAchievementShowCanvas : MonoBehaviour
{
    public GameObject TotalPanel;
    public CanvasGroup Group;

    public TextMeshProUGUI TMP_Name;
    public TextMeshProUGUI TMP_Description;
    public TextMeshProUGUI TMP_RewardValue;

    public Color coinColor;
    public Color unlockColor;

    public float ShowTime = 3;
    public void Init(AchievementInfo achivement)
    {
        TMP_Name.text = achivement.Title;
        TMP_Description.text = achivement.Description;

        switch(achivement.RewardType)
        {
            case REWARDTYPE.COIN:
                TMP_RewardValue.text = achivement.RewardValue + " Coin";
                TMP_RewardValue.color = coinColor;
                break;
            case REWARDTYPE.UNLOCK:
                TMP_RewardValue.text = achivement.rewardText;
                TMP_RewardValue.color = unlockColor;
                break;
        }
    }

    public Coroutine Show(AchievementInfo achivement)
    {
        Init(achivement);
        return StartCoroutine(co_Show());
    }

    IEnumerator co_Show()
    {
        yield return Fade(0.5f, true);
        yield return new WaitForSeconds(2.5f);
        yield return Fade(0.5f, false);
    }

    //ver1. Fade In / Fade Out
    /// <summary>
    /// Fade In / Fade Out을 관리하는 함수
    /// </summary>
    /// <param name="duration">Fade에 걸리는 시간</param>
    /// <param name="isIn"> True일시 Fade In, False일시 Fade Out</param>
    /// <returns></returns>
    IEnumerator Fade(float duration, bool isIn)
    {
        float progress = 0;

        do
        {
            if (isIn) Group.alpha = progress;
            else Group.alpha = 1 - progress;
            progress += Time.deltaTime / duration;

            yield return null;
        } while (progress < 1);

    }



    public void Btn_Close()
    {
        TotalPanel.SetActive(false);
    }
}
