using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary>
/// ���� Ŭ��� �˸��� ���� ��� ������ ����Ʈ
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
