using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary>
/// 업적 클리어를 알리기 위한 대기 데이터 리스트
/// </summary>
public class AchivementShowList
{
    public List<ACHIEVEMENT> list = new List<ACHIEVEMENT>();

    public ACHIEVEMENT GetAchievement()
    {
        if (list.Count == 0) return ACHIEVEMENT.NONE;

        ACHIEVEMENT ac = list[0];
        list.Remove(ac);

        return ac;
    }
}
