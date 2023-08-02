using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : Enemy
{
    [Header("¿ø°Å¸®")]
    public float attackRange;
    public float attackWaitTime;
    public float runawayTime;
    public Projectile projectile;
    public float projectileSpeed;

    private void Awake()
    {
        evnt.attack = shootProjectile;
    }

    public override void StartAI()
    {
        StartCoroutine(co_Chase());
    }
    IEnumerator co_Chase()
    {
        while (Vector3.Distance(transform.position, Target.position) >= attackRange)
        {
            moveTowardTarget(Target.position);
            yield return null;
        }

        StartCoroutine(co_Attack());
    }

    IEnumerator co_Attack()
    {

        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", true);
        yield return new WaitForSeconds(attackWaitTime);
        anim.SetBool("isReady", false);

        anim.SetTrigger("doAttack");
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(co_Runaway());
    }

    IEnumerator co_Runaway()
    {
        float runTimeLeft = runawayTime;

        while(runTimeLeft >= 0)
        {
            runTimeLeft -= Time.deltaTime;
            moveToDir(transform.position - Target.position);
            yield return null;
        }

        StartCoroutine(co_Chase());
    }

    void shootProjectile()
    {
        Projectile temp = Instantiate<Projectile>(projectile, transform.position, Quaternion.identity);

        temp.moveVec = (aim.position - transform.position);
        temp.moveSpeed = projectileSpeed;
        Destroy(temp.gameObject, 4.0f);
    }
}
