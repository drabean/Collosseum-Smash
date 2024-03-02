using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearMgr : MonoBehaviour
{
    SaveData curSaveData;
    GameObject ClearTrophy;

    public void Init(SaveData data)
    {
        curSaveData = data;
        ClearTrophy = Resources.Load<GameObject>("Prefabs/Trophy/ClearTrophy");
    }

    public void SpawnTrophy()
    {
        Instantiate(ClearTrophy, Vector3.zero, Quaternion.identity);
    }
    public void GameClear()
    {
        // rundata 삭제 및 업적 클리어하기
        curSaveData.ClearAchivement(ACHIEVEMENT.NORMALCLEAR);

    }
}
