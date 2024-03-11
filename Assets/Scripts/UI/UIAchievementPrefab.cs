using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class UIAchievementPrefab : MonoBehaviour
{
    public Action onPurchase;

    public TextMeshProUGUI TMP_Title;
    public TextMeshProUGUI TMP_Description;
    public TextMeshProUGUI TMP_RewardValue;
    public GameObject BlockGroup;
    public Color CoinColor;
    public Color UnlockColor;

    ACHIEVEMENT achievement;
    public void Init(AchievementInfo info)
    {
        TMP_Title.text = info.Title;
        TMP_Description.text = info.Description;
        achievement = info.Achievement;
        switch(info.RewardType)
        {
            case REWARDTYPE.COIN:
                TMP_RewardValue.text = info.RewardValue + " Coin";
                TMP_RewardValue.color = CoinColor;
                break;
            case REWARDTYPE.UNLOCK:
                TMP_RewardValue.text = info.rewardText;
                TMP_RewardValue.color = UnlockColor;
                break;
        }

        if (!LoadedSave.Inst.save.CheckAchivement(info.Achievement)) BlockGroup.SetActive(true);
        else BlockGroup.SetActive(false);
    }
    public void Init(string title, string description, string reward)
    {
        TMP_Title.text = title;
        TMP_Description.text = description;
        TMP_RewardValue.text = reward;

    }

    private void OnEnable()
    {
        if (!LoadedSave.Inst.save.CheckAchivement(achievement)) BlockGroup.SetActive(true);
        else BlockGroup.SetActive(false);
    }
}
