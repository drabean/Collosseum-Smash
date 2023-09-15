using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageData : MonoSingleton<StageData>
{
    public StageInfo curStageInfo;

    public List<Equip> equips;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    //������� ��������� �ӽ��ڵ�

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
    }
}
