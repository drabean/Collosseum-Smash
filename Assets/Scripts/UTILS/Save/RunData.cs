using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//���� �� �ѹ��� Run�� ���õ� ������
[Serializable]
public class RunData
{
    public RunData() { }
    public RunData(int characterInfoIdx, int nextStage, List<int> item, int stageProgress)
    {
        this.characterInfoIdx = characterInfoIdx;
        this.item = item;
        this.stageProgress = stageProgress;
    }


    public int characterInfoIdx;
    public List<int> item = new List<int>();
    public int stageProgress = 0;
}