using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스테이지를 지나면서 삭제되지 않는 데이터 클래스
/// 현재캐릭터, 장비, 다음스테이지, 관객 수 등을 관리
/// </summary>
public class StageData : MonoSingleton<StageData>
{
    public StageInfo curStageInfo;

    public List<Equip> equips;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    //여기부터 데모버전용 임시코드

    int stageIndex = 0;
    public void selectStage()
    {
        switch(stageIndex)
        {
            case 0:
                curStageInfo = Resources.Load<StageInfo>("StageInfo/Slime");
                break;
            case 1:
                curStageInfo = Resources.Load<StageInfo>("StageInfo/Champion");
                break;
        }

        stageIndex++;
        stageIndex %= 2;    }
}
