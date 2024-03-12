using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "StageInfo", menuName = "ScriptableObject/StageInfo")]
public class StageInfo : ScriptableObject
{
    public int ID;
    public DIFFICULTY Difficulty;
    /// <summary>
    /// �� ������������ ��ȯ �� �Ϲ� ���͵� 0���� �ε����� �⺻ ����)
    /// </summary>
    public List<Enemy> Enemies;
    public Enemy Boss;
    public int maxKill;

    public AudioClip Intro;
    public AudioClip BGM;
    public GameObject StageDeco;
    [TextArea]
    public string bossText;
    [TextArea]
    public string DeathMessage;

    public ItemThrowable ThrowItem;
}

[Serializable]
public enum DIFFICULTY
{ 
    TUTORIAL = 0,
    EASY = 1,
    NORMAL = 2,
    HARD = 3,
    FINALE = 4,
}
