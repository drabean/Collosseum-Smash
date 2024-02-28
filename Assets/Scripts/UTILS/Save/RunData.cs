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

    public bool isTutorial = false;
    public bool isBoss = false; // true�� ��, �Ϲ� ���� ���������� �ǳʶٰ�, ���� ���������� ����
    public int normalProgress = 0; // �Ϲ� ���� ����ġ - �̹� ������������ ���� �ֵ� ��
    public int bossProgress = 0; // ���� ����ġ - �̹������������� ������ "���� ���ط�"

    public bool isHardMode;

    public int reviveCount = 0; // ��Ȱ ���� Ƚ��
    public bool isRevivedAd = false; // ����� ��Ȱ �ߴ���.

    public bool isGameOver;
}