using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StageInfo", menuName = "ScriptableObject/StageInfo")]
public class StageInfo : ScriptableObject
{
    /// <summary>
    /// 이 스테이지에서 소환 될 일반 몬스터들 0번쨰 인덱스가 기본 몬스터)
    /// </summary>
    public List<Enemy> Enemies;
    public Enemy Boss;
    public int maxKill;

    public AudioClip BGM;
    public GameObject StageDeco;
}