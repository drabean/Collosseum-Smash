using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StageInfo", menuName = "ScriptableObject/StageInfo")]
public class StageInfo : ScriptableObject
{
    /// <summary>
    /// �� ������������ ��ȯ �� �Ϲ� ���͵� 0���� �ε����� �⺻ ����)
    /// </summary>
    public List<Enemy> Enemies;
    public Enemy Boss;
    public int maxKill;

    public AudioClip BGM;
    public GameObject StageDeco;
}