using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AchivementInfo", menuName = "ScriptableObject/AchivementInfo")]
public class AchievementInfo : ScriptableObject
{
    public ACHIEVEMENT Achievement;
    public string Title;
    [TextArea]
    public string Description;

    public REWARDTYPE RewardType;
    public int RewardValue;
    [TextArea]
    public string rewardText;
}

public enum REWARDTYPE
{
    COIN,
    UNLOCK
}
