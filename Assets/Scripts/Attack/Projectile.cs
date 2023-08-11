using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Attack
{
    public float moveSpeed;
    public float lifeTime;

    Vector3 moveVec;

    public override GameObject ShowWarning(Vector3 startPos, Vector3 targetPos, float time)
    {
        curWarning = GameMgr.Inst.AttackEffectLinear(startPos, startPos + (targetPos - startPos).normalized * (moveSpeed * lifeTime), 0.7f, time);
        return curWarning;
    }

    public override void Shoot(Vector3 startPos, Vector3 targetPos)
    {
        transform.position = startPos;
        moveVec = (targetPos - startPos).normalized;

        StartCoroutine(co_Shoot());
    }

    IEnumerator co_Shoot()
    {
        while(lifeTime >= 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + moveVec, moveSpeed * Time.deltaTime);

            lifeTime -= Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

}
