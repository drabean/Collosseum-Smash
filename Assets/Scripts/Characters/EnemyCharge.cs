using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharge : Enemy
{
    [Header("돌진형")]
    public float chargeRange;//돌진거리
    public float chargeStartRange;//돌진 시작 거리
    public float chargeTime;//돌진시간
    public float waitBeforeTime;//돌진 전 대기시간
    public float waitAfterTime;//돌진후대기시간
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
        while (Vector3.Distance(transform.position, Target.position) >= chargeStartRange)
        {
            moveTowardTarget(Target.position);
            yield return null;
        }

        StartCoroutine(co_Charge());
    }
    IEnumerator co_Charge()
    {
        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", true);
        Vector3 chargeDir = (Target.position - transform.position).normalized;
        Vector3 chargeDestination = transform.position + chargeDir * chargeRange;
        setDir(chargeDir);

        yield return new WaitForSeconds(waitBeforeTime);

        anim.SetBool("isReady", false);

        float chargeTimeLeft = chargeTime;
        anim.SetTrigger("doCharge");

        while(chargeTimeLeft >= 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, chargeDestination, chargeRange * (1/chargeTime) * Time.deltaTime);
            chargeTimeLeft -= Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(waitAfterTime);

        StartCoroutine(co_Chase());
    }
}
