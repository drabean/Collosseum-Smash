using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedSave : Singleton<LoadedSave>
{
    public static bool isInit = false;
    public SaveData save;
    public Settings setting;

    public void Init()
    {
        isInit = true;
        save = UTILS.LoadSaveData();
        setting = UTILS.GetSettingData();
    }
    public void SyncSaveData()
    {
        UTILS.SaveSaveData(save);
    }

    public void SyncSetting()
    {
        UTILS.SaveSettingData(setting);
    }
}
