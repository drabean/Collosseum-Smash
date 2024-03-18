using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public int Coin;
    public List<int> Achievements = new List<int>();
    public List<int> Unlocks = new List<int>();
    public Dictionary<string, bool> unlocks = new Dictionary<string, bool>(); // 해금된 것들에 대한 정보. (캐릭터 등?)

    public int BossKill; // 게임 진행도 경험치. (캐릭터 해금 및 아이템풀 해금에 사용)
    public int NormalKill;

    #region 업적 관련 (ACHIVEMENT)
    public bool CheckAchivement(ACHIEVEMENT achievement) // 특정 업적의 클리어 여부를 반환.
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

    #region 상점 관련 (UNLOCK)
    public bool CheckUnlock(UNLOCK unlock) // 특정 업적의 클리어 여부를 반환.
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
/// 업적 Enum
/// 
/// 보스처치
/// 2번째 자리수: 보스 난이도
/// 3번째 자리수: 고유 아이디
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
/// 상점 해금 Enum
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
