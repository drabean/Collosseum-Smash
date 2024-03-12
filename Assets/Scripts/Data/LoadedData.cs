using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ���۵� �� �ε��Ǵ� �Һ�������
/// </summary>
public class LoadedData : Singleton<LoadedData>
{
    public static bool isDataLoaded = false;
   
    Dictionary<int, CharacterInfo> CharacterInfos = new Dictionary<int, CharacterInfo>();
    public int characterInfosCount = 0;

    public Dictionary<int, StageInfo> StageInfos = new Dictionary<int, StageInfo>();
    List<int> tutorialStage = new List<int>();
    List<int> easyStage = new List<int>();
    List<int> normalStage = new List<int>();
    List<int> hardStage = new List<int>();
    List<int> finalStage = new List<int>();
    Dictionary<int, Equip> Equips = new Dictionary<int, Equip>();


    string characterInfoPath = "Datas/CharacterInfo";
    string stageInfoPath = "Datas/StageInfo";
    string equipPath = "Datas/EquipInfo";
    string achivementPath = "Datas/AchivementInfo";

    Dictionary<ACHIEVEMENT, AchievementInfo> Achivements = new Dictionary<ACHIEVEMENT, AchievementInfo>();

    public void LoadData()
    {
        //ĳ���� ���� �ε�
        CharacterInfo[] characterInfos = Resources.LoadAll<CharacterInfo>(characterInfoPath);

        foreach(CharacterInfo c in characterInfos)
        {
            if(!CharacterInfos.ContainsKey(c.ID))
            {
                CharacterInfos.Add(c.ID, c);
                characterInfosCount++;
            }
        }

        StageInfo[] stageInfos = Resources.LoadAll<StageInfo>(stageInfoPath);

        foreach(StageInfo stage in stageInfos)
        {
            StageInfos.Add(stage.ID, stage);

            switch(stage.Difficulty)
            {
                case DIFFICULTY.TUTORIAL:
                    tutorialStage.Add(stage.ID);
                    break;
                case DIFFICULTY.EASY:
                    easyStage.Add(stage.ID);
                    break;
                case DIFFICULTY.NORMAL:
                    normalStage.Add(stage.ID);
                    break;
                case DIFFICULTY.HARD:
                    hardStage.Add(stage.ID);
                    break;
                case DIFFICULTY.FINALE:
                    finalStage.Add(stage.ID);
                    break;
            }
        }
        //��� ���� �ε�
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

        //���� ���� �ε�

        AchievementInfo[] achivements = Resources.LoadAll<AchievementInfo>(achivementPath);

        foreach(AchievementInfo a in achivements)
        {
            Achivements.Add(a.Achievement, a);
        }
        isDataLoaded = true;
    }

    public List<int> GetStageInfoList(DIFFICULTY difficulty)
    {
        switch(difficulty)
        {
            case DIFFICULTY.TUTORIAL:
                return tutorialStage;
            case DIFFICULTY.EASY:
                return easyStage;
            case DIFFICULTY.NORMAL:
                return normalStage;
            case DIFFICULTY.HARD:
                return hardStage;
        }

        return finalStage;
    }
    public CharacterInfo getCharacterInfoByID(int idx)
    {
        return CharacterInfos[idx];
    }

    public StageInfo getStageInfoByID(int ID)
    {
        if (StageInfos.ContainsKey(ID)) return StageInfos[ID];
        else 
        {
            Debug.Log("ID�� �ش��ϴ� ���������� �������� �ʽ��ϴ�");
            return null;
        }
    }
    public Equip getEquipByID(int ID)
    {
        if(Equips.ContainsKey(ID)) return Equips[ID];
        else
        {
            Debug.Log("ID�� �ش��ϴ� �������� �������� �ʽ��ϴ�");
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
            Debug.Log("���� ������ ��ϵǾ� ���� �ʽ��ϴ�");
            return null;
        }
    }
}
