using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChampion : Enemy
{
    [SerializeField] Transform spriteGroup;



    //공격 후, 플레이어에게 공격 기회를 주는 시간. (탈진 시간)
    public float fatigueTime;
    //탈진까지 필요한 공격 횟수
    public int attackCount;
    //
    int attackCountLeft;

    [Header("패턴1")]
    public float pat1Range;//공격 이펙트 소환지점
    public float pat1Width;//공격 이펙트 타격판정 너비
    public string pat1AtkName;
    public float pat1WaitTime;
    public float pat1IntervalTime;
    public float pat1WaitAfterTime;
    public int pat1RepeatTime;

    [Header("패턴2")]
    public float pat2Range;
    public string pat2AtkName;
    public float pat2WaitTime;
    public float pat2IntervalTime;
    public float pat2WaitAfterTime;
    public int pat2RepeatTime;




    #region Override
    private void Awake()
    {
        evnt.attack = doPat1;
        evnt.attack2 = doPat2;
        projectile = Resources.Load<Attack>(projectileName);

        attackCountLeft = attackCount;
    }

    Vector3 minVec = new Vector3(-1, 1, 1);


    public override void StartAI()
    {
        StartCoroutine(co_Chase(co_Pat1()));
    }


    /// <summary>
    /// 다음 행동을 결정
    /// </summary>
    void selectNextMove()
    {
        if(attackCountLeft > 0)
        {
            attackCountLeft--;

            int selecter = Random.Range(0, 4);

            switch(selecter)
            {
                case 0:
                    StartCoroutine(co_Chase(co_Pat1()));
                    break;
                case 1:
                    StartCoroutine(co_Runaway(co_Pat2()));
                    break;
                case 2:
                    StartCoroutine(co_Wander(co_Chase(co_Pat1())));
                    break;    
                case 3:
                    StartCoroutine(co_Wander(co_Pat2()));
                    break;
            }
        }
        else // 패턴 준비가 안되었으므로, 그냥 이동.
        {
            attackCountLeft = attackCount;
            StartCoroutine(cofatigue());
        }
    }

    IEnumerator cofatigue()
    {
        anim.SetBool("isFatigue", true);
        yield return new WaitForSeconds(fatigueTime);
        anim.SetBool("isFatigue", false);
        selectNextMove();
    }

    #endregion

    #region 이동

    //타겟을 향해서 이동하는 stste
    IEnumerator co_Chase(IEnumerator nextMove = null)
    {
        moveSpeed = 6f;
        while (Vector3.Distance(transform.position, Target.transform.position) >= pat1Range)
        {
            moveTowardTarget(Target.transform.position);
            yield return null;
        }
        moveTowardTarget(Target.transform.position);

        if (nextMove != null) StartCoroutine(nextMove);
        else selectNextMove();
    }
    //타겟의 반대방향으로 이동하는 state
    IEnumerator co_Runaway(IEnumerator nextMove = null)
    {
        moveSpeed = 2f;
        float runTimeLeft = 0.4f;

        while (runTimeLeft >= 0)
        {
            runTimeLeft -= Time.deltaTime;
            moveToDir(transform.position - Target.transform.position);
            yield return null;
        }

        if (nextMove != null) StartCoroutine(nextMove);
        else selectNextMove();
    }
    //Target을 중심으로 좌 우 중 랜덤한 방향으로 돌아서 이동하는 state
    IEnumerator co_Wander(IEnumerator nextMove = null)
    {
        moveSpeed = 4f;
        float wanderTime = Random.Range(0.6f, 1.3f);//이동할 시간을 정함
        bool isReversed = Random.Range(0, 2) == 0;//회전 방향을 정함

        while (wanderTime >= 0)
        {
            Vector2 moveVec = (Target.transform.position - transform.position);
            moveVec = Vector2.right * moveVec.y * (-1) + Vector2.up * moveVec.x;
            if (isReversed) moveVec *= -1;
            moveToDir(moveVec);
            wanderTime -= Time.deltaTime;
            yield return null;
        }


        if (nextMove != null) StartCoroutine(nextMove);
        else selectNextMove();
    }
    #endregion
    //patttern1 state
    IEnumerator co_Pat1(IEnumerator nextMove = null)
    {
        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", true);
        curAttackWarning = GameMgr.Inst.AttackEffectCircle(aim.position, pat1Width, pat1WaitTime);
        yield return new WaitForSeconds(pat1WaitTime);
        anim.SetBool("isReady", false);


        for(int i = 0; i < pat1RepeatTime; i++)
        {
            anim.SetTrigger("doAttack");
            yield return new WaitForSeconds(pat1IntervalTime);
        }
        yield return new WaitForSeconds(pat1WaitAfterTime);

        if (nextMove != null) StartCoroutine(nextMove);
        else selectNextMove();
    }

    //실제 공격을 하는 함수. (Animation Event를 통해 호출)
    void doPat1()
    {
        ModuleAttack atk = DictionaryPool.Inst.Pop(pat1AtkName).GetComponent<ModuleAttack>();
        atk.transform.position = aim.position;
        atk.ownerTr = transform;
        transform.position += (aim.position - transform.position).normalized * 0.5f; //공격 할 때 마다, 공격 방향으로 약간 이동

        GameMgr.Inst.Shake(0.1f, 40, 0.2f);
    }
    //Pattern2 state
    IEnumerator co_Pat2(IEnumerator nextMove = null)
    {
        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", true);

        curAttackWarning = projectile.ShowWarning(transform.position, Target.transform.position, pat2WaitTime);
        setDir((Target.transform.position - transform.position).normalized);

        yield return new WaitForSeconds(pat2WaitTime);
        anim.SetBool("isReady", false);


        for (int i = 0; i < pat1RepeatTime; i++)
        {
            anim.SetTrigger("doThrow");
            setDir((Target.transform.position - transform.position).normalized);
            yield return new WaitForSeconds(pat2IntervalTime);
        }
        yield return new WaitForSeconds(pat2WaitAfterTime);

        if (nextMove != null) StartCoroutine(nextMove);
        else selectNextMove();
    }

    Attack projectile;
    public string projectileName;

    Attack curProjectile;
    //실제 공격을 하는 함수. (Animation Event를 통해 호출)
    void doPat2()
    {
        curProjectile = Instantiate<Attack>(projectile, transform.position, Quaternion.identity);
        curProjectile.Shoot(transform.position, aim.transform.position);
    }
}
