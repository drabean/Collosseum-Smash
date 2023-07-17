using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : Enemy
{
    private void Awake()
    {
        StartAI();
    }
    public override void StartAI()
    {
        StartCoroutine(co_Chase());
    }

    IEnumerator co_Chase()
    {
        while(!isDead)
        {
            moveTowardTarget(Target.position);
            yield return null;
        }
    }

}
