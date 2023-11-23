using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "StageInfo", menuName = "ScriptableObject/StageInfo")]
public class StageInfo : ScriptableObject
{
    public int stageIndex;

    /// <summary>
    /// �� ������������ ��ȯ �� �Ϲ� ���͵� 0���� �ε����� �⺻ ����)
    /// </summary>
    public List<Enemy> Enemies;
    public Enemy Boss;
    public int maxKill;

    public AudioClip Intro;
    public AudioClip BGM;
    public GameObject StageDeco;
}