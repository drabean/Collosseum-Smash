using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee: Enemy
{
    [Header("�ٰŸ�")]
    public float attackRange;//���� ����Ʈ ��ȯ����
    public float attackWidth;//���� ����Ʈ Ÿ������ �ʺ�
    public string attackName;
    Attack attack;
    public float attackWaitTime;
    public float attackAfterWaitTime;

    public bool doAttackSpin;

    private void Awake()
    {
        evnt.attack = doAttack;
        attack = Resources.Load<Attack>(attackName);
    }

    public override void StartAI()
    {
        StartCoroutine(co_Chase());
    }
    IEnumerator co_Chase()
    {
        if (isDead) yield break;
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
        if (isDead) yield break;

        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", true);
        curAttackWarning = attack.ShowWarning(transform.position, aim.position, attackWaitTime);
        yield return new WaitForSeconds(attackWaitTime);
        anim.SetBool("isReady", false);

        anim.SetTrigger("doAttack");
        yield return new WaitForSeconds(attackAfterWaitTime);
        StartCoroutine(co_Chase());
    }

    void doAttack()
    {
        Attack temp = Instantiate(attack);
        temp.Shoot(transform.position, aim.position);
        if(doAttackSpin) temp.transform.rotation = (aim.position - transform.position).ToQuaternion();
        else temp.GetComponent<SpriteRenderer>().flipX = sp.flipX;
    }


}
