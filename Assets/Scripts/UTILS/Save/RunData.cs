using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//게임 중 한번의 Run에 관련된 데이터
[Serializable]
public class RunData
{
    public RunData() { }
    public RunData(int characterInfoIdx, List<int> item, int stageProgress)
    {
        this.characterInfoIdx = characterInfoIdx;
        this.item = item;
        this.stageProgress = stageProgress;
    }


    public int characterInfoIdx; // 캐릭터 Idx
    public List<int> item = new List<int>(); //소지중인 장비들
    public int stageProgress = 0; // 스테이지 진행도
    public int curHP;
}