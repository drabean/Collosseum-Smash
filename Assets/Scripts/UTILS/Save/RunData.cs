using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//���� �� �ѹ��� Run�� ���õ� ������
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


    public int characterInfoIdx; // ĳ���� Idx
    public List<int> item = new List<int>(); //�������� ����
    public int stageProgress = 0; // �������� ���൵
    public int curHP;
}