using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : Enemy
{
    public override void StartAI()
    {
        StartCoroutine(co_Chase());
    }

    IEnumerator co_Chase()
    {
        while(!isDead)
        {
            moveTowardTarget(Target.transform.position);
            yield return null;
        }
    }

}
