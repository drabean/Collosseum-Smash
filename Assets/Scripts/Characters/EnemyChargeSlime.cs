using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChargeSlime : Enemy
{
    [Header("������")]
    public float chargeRange;//�����Ÿ�
    public float chargeStartRange;//���� ���� �Ÿ�
    public float chargeTime;//�����ð�
    public float waitBeforeTime;//���� �� ���ð�
    public float waitAfterTime;//�����Ĵ��ð�
    public string attackName;

    public override void StartAI()
    {
        StartCoroutine(co_Chase());
    }

    private void Awake()
    {
        evnt.attack += onAttack;
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
        curAttackWarning = GameMgr.Inst.AttackEffectCircle(chargeDestination, 1f, waitBeforeTime);


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

    void onAttack()
    {
        GameObject attackEffect = DictionaryPool.Inst.Pop(attackName);
        attackEffect.transform.position = transform.position;
    }
}
