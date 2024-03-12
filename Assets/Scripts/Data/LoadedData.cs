using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임이 시작될 때 로딩되는 불변데이터
/// </summary>
public class LoadedData : Singleton<LoadedData>
{
    public static bool isDataLoaded = false;
   
    Dictionary<int, CharacterInfo> CharacterInfos = new Dictionary<int, CharacterInfo>();
    public int characterInfosCount = 0;

    public StageInfo[] stageInfos;
    List<StageInfo> easyStage = new List<StageInfo>();
    List<StageInfo> normalStage = new List<StageInfo>();
    List<StageInfo> hardStage = new List<StageInfo>();
    Dictionary<int, Equip> Equips = new Dictionary<int, Equip>();

    string characterInfoPath = "Datas/CharacterInfo";
    string stageInfoPath = "Datas/StageInfo";
    string equipPath = "Datas/EquipInfo";
    string achivementPath = "Datas/AchivementInfo";

    Dictionary<ACHIEVEMENT, AchievementInfo> Achivements = new Dictionary<ACHIEVEMENT, AchievementInfo>();

    public void LoadData()
    {
        //캐릭터 정보 로딩
        CharacterInfo[] characterInfos = Resources.LoadAll<CharacterInfo>(characterInfoPath);

        foreach(CharacterInfo c in characterInfos)
        {
            if(!CharacterInfos.ContainsKey(c.ID))
            {
                CharacterInfos.Add(c.ID, c);
                characterInfosCount++;
            }
        }

        stageInfos = Resources.LoadAll<StageInfo>(stageInfoPath);
        foreach(StageInfo stage in stageInfos)
        {
            switch(stage.Difficulty)
            {
                case DIFFICULTY.EASY:
                    easyStage.Add(stage);
                    break;
                case DIFFICULTY.NORMAL:
                    normalStage.Add(stage);
                    break;
                case DIFFICULTY.HARD:
                    hardStage.Add(stage);
                    break;
            }
        }
        //장비 정보 로딩
        EquipInfo[] equips = Resources.LoadAll<EquipInfo>(equipPath);

        foreach(EquipInfo e in equips)
        {
            foreach(Equip eq in e.list)
            {
                if (!Equips.ContainsKey(eq.ID))
                {
                    Equips.Add(eq.ID, eq);
                }
            }
        }

        //업적 정보 로딩

        AchievementInfo[] achivements = Resources.LoadAll<AchievementInfo>(achivementPath);

        foreach(AchievementInfo a in achivements)
        {
            Achivements.Add(a.Achievement, a);
        }
        isDataLoaded = true;
    }

    public CharacterInfo getCharacterInfoByID(int idx)
    {
        return CharacterInfos[idx];
    }
    public Equip getEquipByID(int ID)
    {
        if(Equips.ContainsKey(ID)) return Equips[ID];
        else
        {
            Debug.Log("ID에 해당하는 아이템이 존재하지 않습니다");
            return null;
        }
    }

    public AchievementInfo getAchivementInfo(ACHIEVEMENT Achivement)
    {
        if(Achivements.ContainsKey(Achivement))
        {
            return Achivements[Achivement];
        }
        else
        {
            Debug.Log("업적 정보가 등록되어 있지 않습니다");
            return null;
        }
    }
}
