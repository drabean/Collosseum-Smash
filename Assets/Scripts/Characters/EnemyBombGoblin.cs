using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBombGoblin : Enemy
{
    public float attackWaitTime;
    public float explosionRange;

    bool isExploded;

    public GameObject Explosion;
    public override void StartAI()
    {
        StartCoroutine(co_Chase());
    }

    IEnumerator co_Chase()
    {
        while(!isDead)
        {
            moveTowardTarget(Target.transform.position);
            if(Vector3.Distance(transform.position, Target.transform.position) <= explosionRange && !isExploded)
            {
                isExploded = true;
                StartCoroutine(co_Explode());
            }
            yield return null;
        }
    }

    IEnumerator co_Explode()
    {
        hit.FlashWhite(1.0f);
        yield return new WaitForSeconds(attackWaitTime);
        Instantiate(Explosion, transform.position, Quaternion.identity);
        Despawn();
    }



}
