using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedSave : Singleton<LoadedSave>
{
    public static bool isInit = false;
    public SaveData save;
    public Settings setting;
    public AchivementShowList achivementShowList;

    public void Init()
    {
        isInit = true;
        save = UTILS.LoadSaveData();
        setting = UTILS.GetSettingData();
        achivementShowList = UTILS.GetAchivementShowListData();
    }
    public void SyncSaveData()
    {
        UTILS.SaveSaveData(save);
    }

    public void SyncSetting()
    {
        UTILS.SaveSettingData(setting);
    }

    public void SyncAchivementShowList()
    {
        UTILS.SaveAchivementShowListData(achivementShowList);
    }


    public void TryAddAchievement(ACHIEVEMENT achievement)
    {
        if (save.CheckAchivement(achievement)) return;
        achivementShowList.list.Add(achievement);
        SyncAchivementShowList();
    }
}
