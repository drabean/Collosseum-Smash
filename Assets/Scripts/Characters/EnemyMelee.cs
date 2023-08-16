using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee: Enemy
{
    [Header("�ٰŸ�")]
    public float attackRange;//���� ����Ʈ ��ȯ����
    public float attackWidth;//���� ����Ʈ Ÿ������ �ʺ�
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
        while (Vector3.Distance(transform.position, Target.position) >= attackRange)
        {
            moveTowardTarget(Target.position);
            yield return null;
        }
        moveTowardTarget(Target.position);

        StartCoroutine(co_Attack());
    }

    IEnumerator co_Attack()
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
