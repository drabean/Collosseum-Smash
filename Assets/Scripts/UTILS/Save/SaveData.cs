using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public int Money;
    public int Exp; // ���� ���൵ ����ġ. (ĳ���� �ر� � ���)
    public List<int> Achievements = new List<int>();
    public List<int> Unlocks = new List<int>();
    public Dictionary<string, bool> unlocks = new Dictionary<string, bool>(); // �رݵ� �͵鿡 ���� ����. (ĳ���� ��?)

    #region ���� ����
    public bool CheckAchivement(ACHIEVEMENT achievement) // Ư�� ������ Ŭ���� ���θ� ��ȯ.
    {
        if (Achievements.Contains((int)achievement)) return true;
        else return false;
    }

    public void ClearAchivement(ACHIEVEMENT achievement)
    {
        if (!Achievements.Contains((int)achievement))
        {
            Achievements.Add((int)achievement);
        }
    }
    #endregion

    #region �ر� ����
    public bool CheckUnlock(UNLOCKS unlock) // Ư�� ������ Ŭ���� ���θ� ��ȯ.
    {
        if (Unlocks.Contains((int)unlock)) return true;
        else return false;
    }

    public void BuyUnlock(UNLOCKS unlock)
    {
        if (!Unlocks.Contains((int)unlock))
        {
            Unlocks.Add((int)unlock);
        }
    }

    #endregion


    #region �ر� ����
    public bool CheckProgress(PROGRESS unlock) // Ư�� ������ Ŭ���� ���θ� ��ȯ.
    {
        if (Unlocks.Contains((int)unlock)) return true;
        else return false;
    }

    public void BuyProgress(PROGRESS progress)
    {
        if (!Unlocks.Contains((int)progress))
        {
            Unlocks.Add((int)progress);
        }
    }

    #endregion
}

/// <summary>
/// ���� Enum
/// </summary>
public enum ACHIEVEMENT
{
    TUTORIALCLEAR = 0,
    NORMALCLEAR = 1,
    HARDCLEAR = 2,

}

/// <summary>
/// ���� �ر� Enum
/// </summary>
public enum UNLOCKS
{
    ATKSPD = 0,
    THRWDMG = 1,
    MAXHP = 2,
    HPREG = 3,

}

/// <summary>
/// ���൵�� ���� �ر� ����
/// 5���� óġ �� ���� ���ο�� �ر�
/// </summary>
public enum PROGRESS
{
    REROLL = 0,
    ITEMS1 = 1,
    REVIVE1 = 2,
    ITEMS2 = 3,

    HONOR1 = 11,
    HONOR2 = 12,
    HONOR3 = 13,
}
/*
 * ���� ����
 * 00. Tutorial Ŭ����
 * 10 ~ 19. �� ������ Ŭ����
 * 20 ~ 29. �� ĳ���͵�� ���� Ŭ����
 */