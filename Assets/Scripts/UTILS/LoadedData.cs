using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedData : Singleton<LoadedData>
{
    public bool isDataLoaded = false;

    public CharacterInfo[] characterInfos;
    string characterInfoPath = "Datas/CharacterInfo";

    public void LoadData()
    {
        characterInfos = Resources.LoadAll<CharacterInfo>(characterInfoPath);

        isDataLoaded = true;
    }
}
