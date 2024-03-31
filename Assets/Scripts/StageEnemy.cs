using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEnemy : StageMechanic
{

    public Attack attack;

    public float attackIntervalTime;
    public float attackWaitTime;

    public float attackRange;

    public Animator anim;
    public AnimationEventReciever evnt;

    bool isActionActive = false;
    public Transform aim;



    public override void Init(Transform target)
    {
        base.Init(target);
        evnt.attack += () => { Instantiate(attack).Shoot(transform.position,aim.position); };

        StartCoroutine(co_ActionCoroutine());
    }
    public override void StartAction()
    {
        isActionActive = true;
    }

    public override void endAction()
    {
        isActionActive = false;

    }


    IEnumerator co_ActionCoroutine()
    {
        while(true)
        {
            if (isActionActive)
            {
                setDir();

                if (Vector3.Distance(transform.position, target.position) <= attackRange)
                {
                    yield return StartCoroutine(co_Shoot());
                    yield return new WaitForSeconds(attackIntervalTime + Random.Range(0, 2.5f));
                }
                else yield return null;
            }
            else
            {
                setDir();
                yield return null;
            }
        }
    }
    void setDir()
    {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        anim.SetFloat("dirX", dir.x);
        anim.SetFloat("dirY", dir.y);

    }
    IEnumerator co_Shoot()
    {
        anim.SetBool("isReady", true);
        aim.position = target.transform.position;
        setDir();

        attack.ShowWarning(transform.position, aim.position, attackWaitTime);
        yield return new WaitForSeconds(attackWaitTime);
        anim.SetBool("isReady", false);

        anim.SetTrigger("doAttack");
    }
}
