using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlimeKing : Enemy
{

    [Header("패턴1")]
    public float pat1Width;//공격 이펙트 타격판정 너비
    public string pat1AtkName;
    public float pat1WaitTime;
    public float pat1IntervalTime;
    public float pat1WaitAfterTime;
    public int pat1RepeatTime;

    [Header("패턴2")]
    public float pat2Range;
    public string pat2AtkName;
    public float pat2WaitTime;
    public float pat2IntervalTime;
    public float pat2WaitAfterTime;
    public int pat2RepeatTime;

}
