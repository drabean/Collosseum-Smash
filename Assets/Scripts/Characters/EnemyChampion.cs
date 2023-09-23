using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChampion : EnemyBoss
{
    [SerializeField] Transform spriteGroup;

    public float fatigueTime;

    #region Override
    private void Awake()
    {
        evnt.attack = doPat1;
        evnt.attack2 = doPat2;
        evnt.attack3 = doPat3;
        projectile = Resources.Load<Attack>(patterns[1].prefabName);

        patternCountLeft = patternCount;
    }

    Vector3 minVec = new Vector3(-1, 1, 1);


    public override void StartAI()
    {
        selectPattern();
    }

    bool toglePat;

    /// <summary>
    /// 다음 행동을 결정
    /// </summary>
    protected override void selectPattern()
    {
        if(patternCountLeft > 1)
        {
            toglePat = !toglePat;
            patternCountLeft--;

            int selecter = Random.Range(0, 2);
            if (toglePat)
            {
                switch (selecter)
                {
                    case 0:
                        StartCoroutine(co_Chase(co_Pat1())); // 추격 후 근접공격
                        break;
                    case 1:
                        StartCoroutine(co_Wander(co_Chase(co_Pat1()))); // 측면 이동 - 추격 후 근접공격
                        break;

                }
            }
            else
            {
                switch (selecter)
                {
                    case 0:
                        StartCoroutine(co_Runaway(co_Pat2())); // 도주 후 원거리 공격
                        break;
                    case 1:
                        StartCoroutine(co_Wander(co_Pat2())); // 측면이동 후 원거리공격
                        break;
                }
            }
            
        }
        else if(patternCountLeft == 1)
        {
            patternCountLeft--;
            StartCoroutine(co_Pat3());
        }
        else // 연속 공격 끝난 후 그로기
        {
            patternCountLeft = patternCount;
            StartCoroutine(co_Fatigue());
        }
    }

    IEnumerator co_Fatigue() // 그로기 상태
    {
        anim.SetBool("isFatigue", true);
        yield return new WaitForSeconds(fatigueTime);
        anim.SetBool("isFatigue", false);
        selectPattern();
    }

    #endregion

    #region 이동

    //타겟을 향해서 이동 (추적)
    IEnumerator co_Chase(IEnumerator nextMove = null)
    {
        moveSpeed = 6f;
        while (Vector3.Distance(transform.position, Target.transform.position) >= patterns[0].range)
        {
            moveTowardTarget(Target.transform.position);
            yield return null;
        }
        moveTowardTarget(Target.transform.position);

        if (nextMove != null) StartCoroutine(nextMove);
        else selectPattern();
    }
    //타겟의 반대방향으로 이동 (도주)
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
        else selectPattern();
    }
    //Target을 중심으로 측면으로 이동
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
        else selectPattern();
    }
    #endregion
    //Patterm1 - 근접공격
    IEnumerator co_Pat1(IEnumerator nextMove = null)
    {
        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", true);
        curAttackWarning = GameMgr.Inst.AttackEffectCircle(aim.position, patterns[0].width, patterns[0].waitBeforeTime);
        yield return new WaitForSeconds(patterns[0].waitBeforeTime);
        anim.SetBool("isReady", false);


        for(int i = 0; i < patterns[0].repeatTIme; i++)
        {
            anim.SetTrigger("doAttack");
            yield return new WaitForSeconds(patterns[0].intervalTime);
        }
        yield return new WaitForSeconds(patterns[0].waitAfterTime);

        if (nextMove != null) StartCoroutine(nextMove);
        else selectPattern();
    }

    //실제 공격을 하는 함수. (Animation Event를 통해 호출)
    void doPat1()
    {
        ModuleAttack atk = DictionaryPool.Inst.Pop(patterns[0].prefabName).GetComponent<ModuleAttack>();
        atk.transform.position = aim.position;
        atk.ownerTr = transform;
        transform.position += (aim.position - transform.position).normalized * 0.5f; //공격 할 때 마다, 공격 방향으로 약간 이동

        GameMgr.Inst.MainCam.Shake(0.1f, 40, 0.2f, 0f);
    }
    //Pattern2 - 투창공격
    IEnumerator co_Pat2(IEnumerator nextMove = null)
    {
        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", true);

        curAttackWarning = projectile.ShowWarning(transform.position, Target.transform.position, patterns[1].waitBeforeTime);
        setDir((Target.transform.position - transform.position).normalized);

        yield return new WaitForSeconds(patterns[1].waitBeforeTime);
        anim.SetBool("isReady", false);


        for (int i = 0; i < patterns[1].repeatTIme; i++)
        {
            anim.SetTrigger("doThrow");
            setDir((Target.transform.position - transform.position).normalized);
            yield return new WaitForSeconds(patterns[1].intervalTime);
        }
        yield return new WaitForSeconds(patterns[1].waitAfterTime);

        if (nextMove != null) StartCoroutine(nextMove);
        else selectPattern();
    }
    //
    Attack projectile;

    //실제 공격을 하는 함수. (Animation Event를 통해 호출)
    void doPat2()
    {
        Attack curProjectile = Instantiate<Attack>(projectile, transform.position, Quaternion.identity);
        curProjectile.Shoot(transform.position, aim.transform.position);
    }

    IEnumerator co_Pat3()
    {
        anim.SetTrigger("doShout");
        yield return new WaitForSeconds(patterns[2].waitBeforeTime);
        curAttackWarning = GameMgr.Inst.AttackEffectCircle(transform.position + Vector3.up * 0.3f, 1.5f, 1.0f);
        yield return new WaitForSeconds(0.5f);
        spawnCompanion();
        yield return new WaitForSeconds(patterns[2].waitAfterTime);

        selectPattern();
    }

    //실제 공격을 하는 함수. (Animation Event를 통해 호출)
    void doPat3()
    {
        DictionaryPool.Inst.Pop("Prefabs/Attack/ShoutEffect").transform.position = transform.position + Vector3.up * 0.3f;
        GameMgr.Inst.MainCam.Shake(0.4f, 20, 0.15f, 0, true);

    }

    void spawnCompanion()
    {
        Enemy enemyPrefab = Resources.Load<Enemy>(patterns[2].prefabName);
        EnemyMgr.Inst.SpawnEnemy(enemyPrefab, EnemyMgr.Inst.getRandomPos());
        EnemyMgr.Inst.SpawnEnemy(enemyPrefab, EnemyMgr.Inst.getRandomPos());
    }

}
