using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public int Exp; // ���� ���൵ ����ġ. (ĳ���� �ر� � ���)
    public int ProgressLV;
    public List<int> Achievements = new List<int>();
    public List<int> Unlocks = new List<int>();
    public Dictionary<string, bool> unlocks = new Dictionary<string, bool>(); // �رݵ� �͵鿡 ���� ����. (ĳ���� ��?)

    public bool checkAchivement(ACHIEVEMENT achievement) // Ư�� ������ Ŭ���� ���θ� ��ȯ.
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

    #region �ر� ����


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
/// </summary>
public enum PROGRESS
{
    ITEMS1 = 0,
    REROLL = 1,
    ITEMS2 = 2,

    HONOR1 = 11,
    HONOR2 = 12,
    HONOR3 = 13,
}
/*
 * ���� ����
 * 00. Tutorial Ŭ����
 * 10 ~ 19. �� ������ Ŭ����
 * 20 ~ 29. �� ĳ���͵�� ���� Ŭ����
 * 
 * Exp ����
 * �������� Ŭ���Ƹ��� 1 ����
 * ����ġ�� 10 ���� �� ����, ���ο� ĳ���͸� ������.
 * Exp ���� ������ ���� �����, ���� óġ �� ������ ȹ�� �� �� Scene ��ȯ �� RunData�� ������ �̷������ Ÿ�ֿ̹� ����.
 * Exp�� ���� �ر��� ��¼����¼��
 */