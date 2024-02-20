using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public int Exp; // 게임 진행도 경험치. (캐릭터 해금 등에 사용)
    public int ProgressLV;
    public List<int> Achievements = new List<int>();
    public List<int> Unlocks = new List<int>();
    public Dictionary<string, bool> unlocks = new Dictionary<string, bool>(); // 해금된 것들에 대한 정보. (캐릭터 등?)

    public bool checkAchivement(ACHIEVEMENT achievement) // 특정 업적의 클리어 여부를 반환.
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

    #region 해금 관련


    #endregion
}

/// <summary>
/// 업적 Enum
/// </summary>
public enum ACHIEVEMENT
{
    TUTORIALCLEAR = 0,
    NORMALCLEAR = 1,
    HARDCLEAR = 2,

}

/// <summary>
/// 상점 해금 Enum
/// </summary>
public enum UNLOCKS
{
    ATKSPD = 0,
    THRWDMG = 1,
    MAXHP = 2,
    HPREG = 3,

}

/// <summary>
/// 진행도에 따른 해금 정도
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
 * 업적 정보
 * 00. Tutorial 클리어
 * 10 ~ 19. 각 보스들 클리어
 * 20 ~ 29. 각 캐릭터들로 게임 클리어
 * 
 * Exp 정보
 * 스테이지 클리아마다 1 쌓임
 * 경험치를 10 쌓을 때 마다, 새로운 캐릭터를 열어줌.
 * Exp 정보 저장은 각각 사망시, 보스 처치 후 아이템 획득 후 등 Scene 전환 및 RunData의 갱신이 이루어지는 타이밍에 저장.
 * Exp를 통한 해금은 어쩌구저쩌구
 */