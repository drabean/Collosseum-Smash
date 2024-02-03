using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee: Enemy
{
    [Header("�ٰŸ�")]
    public float attackRange;//���� ����Ʈ ��ȯ����
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
        isChasing = false;
        selectState();
    }

    void selectState()
    {
        if (!checkOtherMeleeEnemy()) StartCoroutine(co_Chase(Random.Range(1.0f, 2.0f)));
        else StartCoroutine(co_Wander(Random.Range(0.2f, 0.8f))); // �ֺ��� �̹� Chase������ ���� �ִٸ�, 0.5�ʵ��� ��ġ�� �� ���� �Ŀ� �߰�.
    }
    IEnumerator co_Chase(float chaseTime)
    {
        isChasing = true;
        while (chaseTime >= 0)
        {
            if (isDead) yield break;
            chaseTime -= Time.deltaTime;

            moveTowardTarget(Target.transform.position);

            if (Vector3.Distance(transform.position, Target.transform.position) < attackRange)
            {
                yield return StartCoroutine(co_Atk());
            }
            yield return null;
        }
        isChasing = false;
        selectState();
    }

    /// <summary>
    /// Target�� �߽����� ���� �̵�
    /// </summary>
    /// <param name="nextMove"></param>
    /// <returns></returns>
    IEnumerator co_Wander(float wanderTime)
    {
        bool isReversed = Random.Range(0, 2) == 0;//ȸ�� ������ ����

        while (wanderTime >= 0)
        {
            if (isDead) yield break;
            wanderTime -= Time.deltaTime;

            Vector2 moveVec = (Target.transform.position - transform.position);
            moveVec = Vector2.right * moveVec.y * (-1) + Vector2.up * moveVec.x;
            if (isReversed) moveVec *= -1;
            moveToDir(moveVec);

            if (Vector3.Distance(transform.position, Target.transform.position) < attackRange)
            {
                yield return StartCoroutine(co_Atk());
            }
            yield return null;
        }

        anim.SetBool("isMoving", false);
        yield return new WaitForSeconds(0.5f);
        selectState();
    }

    IEnumerator co_Atk()
    {
        if (isDead) yield break;
        //���� ����
        moveTowardTarget(Target.transform.position);

        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", true);
        curAttackWarning = attack.ShowWarning(transform.position, aim.position, attackWaitTime);
        yield return new WaitForSeconds(attackWaitTime);
        anim.SetBool("isReady", false);

        anim.SetTrigger("doAttack");
        yield return new WaitForSeconds(attackAfterWaitTime);
    }

    void doAttack()
    {
        Attack temp = Instantiate(attack);
        temp.Shoot(transform.position, aim.position);
        if(doAttackSpin) temp.transform.rotation = (aim.position - transform.position).ToQuaternion();
        else temp.GetComponent<SpriteRenderer>().flipX = sp.flipX;
    }


    [SerializeField] LayerMask layer;
    /// <summary>
    ///  �ֺ��� �ٸ� EnemyMelee�� �ִ��� Ȯ���ϰ�, �ش� ���� Chase ����̸�, �� ĳ���ʹ�  ���� �߰��� �ƴ�, �ֺ� ���ƴٴϴ� ���������� ��ȯ��.
    /// </summary>
    /// <returns>Chase ����� �ٸ� ���� �ִٸ� true, �ƴϸ� false</returns>
    bool checkOtherMeleeEnemy()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 1, Vector3.forward, 0f, layer);

        if (hits.Length == 0) //�������� ���� Target�� �������� ����
        {
          //  Debug.Log("������ �ƹ��� ����");
            return false;
        }
        else
        {

            for (int i = 0; i < hits.Length; i++)
            {
                if(hits[i].collider.TryGetComponent<EnemyMelee>(out EnemyMelee em))
                {
                    if(em.isChasing) return true;
                }
                    
            }
        }

        return false;
    }



}
