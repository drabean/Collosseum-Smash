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

    public void Init(string title, string description, string reward)
    {
        TMP_Title.text = title;
        TMP_Description.text = description;
        TMP_RewardValue.text = reward;

    }
}
