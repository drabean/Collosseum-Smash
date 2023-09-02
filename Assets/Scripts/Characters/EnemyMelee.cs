using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee: Enemy
{
    [Header("근거리")]
    public float attackRange;//공격 이펙트 소환지점
    public float attackWidth;//공격 이펙트 타격판정 너비
    public string attackName;
    public float attackWaitTime;
    public float attackAfterWaitTime;

    private void Awake()
    {
        evnt.attack = doAttack;
    }

    public override void StartAI()
    {
        StartCoroutine(co_Chase());
    }
    IEnumerator co_Chase()
    {
        while (Vector3.Distance(transform.position, Target.transform.position) >= attackRange)
        {
            moveTowardTarget(Target.transform.position);
            yield return null;
        }
        moveTowardTarget(Target.transform.position);

        StartCoroutine(co_Atk());
    }

    IEnumerator co_Atk()
    {

        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", true);
        curAttackWarning = GameMgr.Inst.AttackEffectCircle(aim.position, attackWidth, 1.0f);
        yield return new WaitForSeconds(attackWaitTime);
        anim.SetBool("isReady", false);

        anim.SetTrigger("doAttack");
        yield return new WaitForSeconds(attackAfterWaitTime);
        StartCoroutine(co_Chase());
    }

    void doAttack()
    {
        GameObject attackEffect = DictionaryPool.Inst.Pop(attackName);
        attackEffect.transform.position = aim.position;
        attackEffect.GetComponent<SpriteRenderer>().flipX = sp.flipX;
        
    }


}
