using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee: Enemy
{
    [Header("근거리")]
    public float attackRange;//공격 이펙트 소환지점
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
        else StartCoroutine(co_Wander(Random.Range(0.2f, 0.8f))); // 주변에 이미 Chase상태인 적이 있다면, 0.5초동안 위치를 재 조정 후에 추격.
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
    /// Target을 중심으로 측면 이동
    /// </summary>
    /// <param name="nextMove"></param>
    /// <returns></returns>
    IEnumerator co_Wander(float wanderTime)
    {
        bool isReversed = Random.Range(0, 2) == 0;//회전 방향을 정함

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
        //방향 조절
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
    ///  주변에 다른 EnemyMelee가 있는지 확인하고, 해당 적이 Chase 모드이면, 이 캐릭터는  직선 추격이 아닌, 주변 돌아다니는 움직임으로 전환함.
    /// </summary>
    /// <returns>Chase 모드인 다른 적이 있다면 true, 아니면 false</returns>
    bool checkOtherMeleeEnemy()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 1, Vector3.forward, 0f, layer);

        if (hits.Length == 0) //추적범위 내에 Target이 존재하지 않음
        {
          //  Debug.Log("주위에 아무도 없음");
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
