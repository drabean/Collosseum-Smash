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
    public float attackColRange;

    public string attackName;
    public string SFXName;

    Attack attack;

    public override void StartAI()
    {
        StartCoroutine(co_Chase());
    }

    private void Awake()
    {
        evnt.attack += onAttack;
        attack = Resources.Load<Attack>(attackName);
    }

    protected virtual IEnumerator co_Chase()
    {
        while (Vector3.Distance(transform.position, Target.transform.position) >= chargeStartRange)
        {
            moveTowardTarget(Target.transform.position);
            yield return null;
        }

        StartCoroutine(co_Charge());
    }
   protected IEnumerator co_Charge()
    {
        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", true);
        Vector3 chargeDir = (Target.transform.position - transform.position).normalized;
        Vector3 chargeDestination = transform.position + chargeDir * chargeRange;

        setDir(chargeDir);
        //curAttackWarning = GameMgr.Inst.AttackEffectLinear(transform.position, chargeDestination, 0.5f, waitBeforeTime);
        curAttackWarning = attack.ShowWarning(transform.position, aim.position, waitBeforeTime);


        yield return new WaitForSeconds(waitBeforeTime);

        anim.SetBool("isReady", false);

        float chargeTimeLeft = chargeTime;
        anim.SetTrigger("doCharge");
        SoundMgr.Inst.Play(SFXName);
        while(chargeTimeLeft >= 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, chargeDestination, chargeRange * (1/chargeTime) * Time.deltaTime);
            chargeTimeLeft -= Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(waitAfterTime);

        StartCoroutine(co_Chase());
    }

    void onAttack()
    {
        Instantiate(attack).Shoot(transform.position, transform.position);
    }
}
