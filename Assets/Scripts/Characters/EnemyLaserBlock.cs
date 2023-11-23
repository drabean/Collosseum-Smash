using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserBlock : Enemy
{
    [Header("∑π¿Ã¡Æ")]
    public float attackRange;
    public float attackWaitTime;
    public float attackWaitAfterTime;
    public float runawayTime;
    public string projectileName;

    Attack curProjectile;

    float topPos = 5.0f;
    private void Awake()
    {
        evnt.attack = shootProjectile;
        projectile = Resources.Load<Attack>(projectileName);
    }

    public override void StartAI()
    {
        StartCoroutine(co_Move(Vector3.up * topPos));
        StartCoroutine(co_Attack());
    }
    IEnumerator co_Move(Vector3 destination)
    {
        yield return new WaitForSeconds(0.5f);
        SoundMgr.Inst.Play("Jump");
        anim.SetTrigger("doMove");
        float timeLeft = 0.5f;

        moveSpeed = Vector3.Distance(destination, transform.position) / 0.5f;

        while (timeLeft >= 0)
        {
            moveTowardTarget(destination);
            setDir();
            timeLeft -= Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator co_Attack()
    {
        yield return co_Move(Target.transform.position.x * Vector3.right + Vector3.up * topPos);
        anim.SetBool("isReady", true);
        Vector3 targetVec = transform.position.x * Vector3.right + transform.position.y * (-1) * Vector3.up;

        curAttackWarning = projectile.ShowWarning(transform.position, targetVec, attackWaitTime);

        yield return new WaitForSeconds(attackWaitTime);
        anim.SetBool("isReady", false);

        Instantiate(projectile).Shoot(transform.position, targetVec);
        
        yield return new WaitForSeconds(attackWaitAfterTime);


        StartCoroutine(co_Attack());
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
        aim.transform.position = Target.transform.position;
    }
}
