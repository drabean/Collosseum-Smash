using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGhost : EnemyCharge
{
    [Header("GHOstÀü¿ë")]
    public float attackWaitTime;


    protected override IEnumerator co_Chase()
    {
        while (Vector3.Distance(transform.position, Target.position) >= chargeStartRange)
        {
            moveTowardTarget(Target.position);
            yield return null;
        }

        float attackTimeLeft = attackWaitTime;

        while(attackTimeLeft >= 0)
        {
            Vector2 moveVec = (Target.position - transform.position);
            moveVec = Vector2.right * moveVec.y + Vector2.up * moveVec.x * -1;

            moveToDir(moveVec);
            attackTimeLeft -= Time.deltaTime;

            yield return null;
        }
        StartCoroutine(co_Charge());
    }
}
