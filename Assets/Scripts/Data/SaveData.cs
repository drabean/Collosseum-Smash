using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public int Coin;
    public List<int> Achievements = new List<int>();
    public List<int> Unlocks = new List<int>();
    public Dictionary<string, bool> unlocks = new Dictionary<string, bool>(); // �رݵ� �͵鿡 ���� ����. (ĳ���� ��?)

    public int BossKill; // ���� ���൵ ����ġ. (ĳ���� �ر� �� ������Ǯ �رݿ� ���)
    public int NormalKill;

    #region ���� ���� (ACHIVEMENT)
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
        else
        {
            Debug.Log((int)achievement + " not Exist");
        }
    }
    #endregion

    #region ���� ���� (UNLOCK)
    public bool CheckUnlock(UNLOCK unlock) // Ư�� ������ Ŭ���� ���θ� ��ȯ.
    {
        if (Unlocks.Contains((int)unlock)) return true;
        else return false;
    }

    public void BuyUnlock(UNLOCK unlock)
    {
        if (!Unlocks.Contains((int)unlock))
        {
            Unlocks.Add((int)unlock);
        }
    }

    #endregion

}

/// <summary>
/// ���� Enum
/// 
/// ����óġ
/// 2��° �ڸ���: ���� ���̵�
/// 3��° �ڸ���: ���� ���̵�
/// </summary>
public enum ACHIEVEMENT
{
    TUTORIALCLEAR = 0,
    NORMALCLEAR = 1,
    HARDCLEAR = 2,

    KILLTUTORIAL =  100,
    KILLHAIRBALL =  110,
    KILLGOBLIN =    111,
    KILLMUSHROOM =  112,
    KILLCHAMPION =  120,
    KILLSLIME =     121,
    KILLENT =       122,
    KILLLICH =      130,
    KILLBLOCK =     131,

    SMASHBOSS1 = 200,
    SMASHBOSS2 = 201,
    SMASHNORMAL1 = 202,
    SMASHNORMAL2 = 203,

    FIRSTRETRY = 300,
    

    NONE = 999

}

/// <summary>
/// ���� �ر� Enum
/// </summary>
public enum UNLOCK
{
    ATKSPD = 0,
    THRWDMG = 1,
    MAXHP = 2,
    HPREG = 3,
    MONEY = 4,
    REVIVE = 5,
    ATKDMG = 6,
    MOVSPD = 7,
    ADDITEM = 8,
}
