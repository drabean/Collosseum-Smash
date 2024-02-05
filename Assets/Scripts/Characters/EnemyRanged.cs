using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : Enemy
{
    [Header("원거리")]
    public float attackRange;
    public float attackWaitTime;
    public float attackAfterTime;
    public float runawayTime;
    public string projectileName;

    Attack curProjectile;

    private void Awake()
    {
        evnt.attack = shootProjectile;
        projectile = Resources.Load<Attack>(projectileName);
    }

    public override void StartAI()
    {
        StartCoroutine(co_Move());
    }
    IEnumerator co_Move()
    {
        while (Vector3.Distance(transform.position, Target.transform.position) >= attackRange)
        {
            moveTowardTarget(Target.transform.position);
            yield return null;
        }
        moveTowardTarget(Target.transform.position);

        StartCoroutine(co_Attack());
    }

    IEnumerator co_Attack()
    {

        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", true);

       
        curAttackWarning = projectile.ShowWarning(transform.position, Target.transform.position, attackWaitTime);
        
        yield return new WaitForSeconds(attackWaitTime);
        anim.SetBool("isReady", false);

        anim.SetTrigger("doAttack");
        SoundMgr.Inst.Play("Throw");
        yield return new WaitForSeconds(attackAfterTime);
        StartCoroutine(co_Runaway());
    }

    //공격 후 거리조절
    IEnumerator co_Runaway()
    {
        float runTimeLeft = runawayTime;

        while(runTimeLeft >= 0)
        {
            runTimeLeft -= Time.deltaTime;
            if (Vector3.Distance(transform.position, Target.transform.position) >= attackRange) 
                moveToDir(transform.position - Target.transform.position);  //공격사거리보다 거리가 멀다면 가까이
            else moveToDir(Target.transform.position - transform.position); //아니라면 멀리
            yield return null;
        }

        StartCoroutine(co_Move());
    }

    Attack projectile;

    void shootProjectile()
    {
        if (isDead) return;
        curProjectile = Instantiate<Attack>(projectile, transform.position, Quaternion.identity);
        curProjectile.Shoot(transform.position, aim.transform.position);
    }

    protected override void setDir(Vector3 dir)
    {
        dir = dir.normalized;
        anim.SetFloat("dirX", dir.x);
        anim.SetFloat("dirY", dir.y);
        if (dir.x != 0 && sp != null) sp.flipX = dir.x < 0 ? true : false;
        aim.transform.position = Target.transform.position;
    }
}
