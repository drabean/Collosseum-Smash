using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChampion : EnemyBoss
{
    Attack[] attacks = new Attack[5];
    Attack GroundBlock;
    #region Override
    private void Awake()
    {
        evnt.attack = doPat1;
        evnt.attack2 = doPat2;
        evnt.attack3 = doPat3;
        evnt.attack4 = doPat4;
        

        patternCountLeft = patternCount;
        attacks[0]= Resources.Load<Attack>(patterns[0].prefabName);
        attacks[1] = Resources.Load<Attack>(patterns[1].prefabName);
        attacks[2] = Resources.Load<Attack>(patterns[2].prefabName);
        attacks[3] = Resources.Load<Attack>(patterns[3].prefabName);
        attacks[4] = Resources.Load<Attack>("Prefabs/Attack/MultiSpear2");

        GroundBlock = Resources.Load<Attack>("Prefabs/Attack/GroundBlock");
    }


    public override void StartAI()
    {
        if(isHardMode)
        {
            patterns[3].repeatTIme++;
            patterns[3].intervalTime -= 0.3f;
        }
        selectPattern();
    }

    /// <summary>
    /// 다음 행동을 결정
    /// </summary>
    protected override void selectPattern()
    {
        if(patternCountLeft > 1)
        {

            int patIdx = Random.Range(0, 4);
            float dist = Vector3.Distance(transform.position, Target.transform.position);

            switch (patIdx)
            {
                case 0: // 근접공격
                    patternCountLeft--;
                    if (dist > 1.5f) StartCoroutine(co_Chase(co_Pat1())); // 어느정도 거리가 있다면 추격 후 공격
                    else StartCoroutine(co_Runaway(co_Chase(co_Pat1()))); // 너무 가깝다면, 거리를 벌린 후에 추격 후 공격
                    break;
                case 1: // 원거리 공격
                    patternCountLeft--;
                    if (dist > 3) StartCoroutine(co_Wander(co_Pat2())); // 어느정도 거리가 있다면 자리변경 후 공격
                    else StartCoroutine(co_Runaway(co_Wander(co_Pat2()), 0.6f)); // 너무 가깝다면 거리를 벌린 후에 공격
                    break;
                case 2:
                    patternCountLeft--;
                    StartCoroutine(co_Runaway(co_Wander(co_Pat4()), 0.4f));
                    break;
                case 3:
                    if (dist < 1.5f) StartCoroutine(co_Runaway(co_Wander(), 0.2f)); // 혼란을 위한 움직임, 거리를 잠시 벌리고 가로로 이동
                    else selectPattern();
                    break;
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
        anim.SetBool("isMoving", false);

        float timeLeft = patterns[2].waitAfterTime;
        subHP = 25;
        while (subHP >= 0 && timeLeft >= 0)
        {
            timeLeft -= Time.deltaTime;

            yield return null;
        }
        anim.SetBool("isFatigue", false);
        selectPattern();
    }

    #endregion

    #region 이동

    //타겟을 향해서 이동 (추적)
    IEnumerator co_Chase(IEnumerator nextMove = null)
    {
        moveSpeed = Target.moveSpeed + 1f;
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
    IEnumerator co_Runaway(IEnumerator nextMove = null, float time = 1)
    {
        moveSpeed = 3f;

        while (time >= 0)
        {
            time -= Time.deltaTime;
            moveToDir(transform.position - Target.transform.position);
            yield return null;
        }

        if (nextMove != null) StartCoroutine(nextMove);
        else selectPattern();
    }
    //Target을 중심으로 측면으로 이동
    IEnumerator co_Wander(IEnumerator nextMove = null)
    {
        moveSpeed = 5f;
        float wanderTime = Random.Range(0.6f, 1.0f);//이동할 시간을 정함
        bool isReversed = (Random.Range(0, 2) == 0);//회전 방향을 정함

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
    bool isSmashGround;
    //Patterm1 - 근접공격
    IEnumerator co_Pat1(IEnumerator nextMove = null)
    {
        anim.SetBool("isMoving", false);

        for(int i = 0; i < patterns[0].repeatTIme-1; i++)
        {
            aimRange = 0.8f;
            setDir();
            float waitTime = patterns[0].waitBeforeTime;
            if (i != 0) waitTime -= 0.15f;

            curAttackWarning = attacks[0].ShowWarning(aim.position, aim.position, waitTime);
            anim.SetBool("isAtkReady", true);
            yield return new WaitForSeconds(waitTime);
            anim.SetBool("isAtkReady", false);

            anim.SetTrigger("doAttack");
            yield return new WaitForSeconds(patterns[0].intervalTime);
        }


        aimRange = 1.4f;
        setDir();
        curAttackWarning = attacks[0].ShowWarning(aim.position, aim.position, 0.6f);
        if (isHardMode) isSmashGround = true;
        anim.SetBool("isAtkReady", true);
        yield return new WaitForSeconds(0.6f);
        anim.SetBool("isAtkReady", false);

        anim.SetTrigger("doAttack");

        if(isRage)
        {
            curAttackWarning = attacks[0].ShowWarning(aim.position, aim.position, 0.6f);
            if (isHardMode) isSmashGround = true;
            anim.SetBool("isAtkReady", true);
            yield return new WaitForSeconds(Random.Range( 0.5f, 0.8f));
            anim.SetBool("isAtkReady", false);

            anim.SetTrigger("doAttack");
        }
        yield return new WaitForSeconds(patterns[0].waitAfterTime);

        if (nextMove != null) StartCoroutine(nextMove);
        else selectPattern();
    }

    //실제 공격을 하는 함수. (Animation Event를 통해 호출)
    void doPat1()
    {
        if (isSmashGround)
        {
            isSmashGround = false;
            ShootGroundBlock();
        }
        Instantiate(attacks[0]).Shoot(aim.position, aim.position);
        transform.position += (aim.position - transform.position).normalized * 0.5f; //공격 할 때 마다, 공격 방향으로 약간 이동

        GameMgr.Inst.MainCam.Shake(0.1f, 40, 0.2f, 0f);
    }
    //Pattern2 - 투창공격
    IEnumerator co_Pat2(IEnumerator nextMove = null)
    {
        anim.SetBool("isMoving", false);


        for (int i = 0; i < patterns[1].repeatTIme; i++)
        {
            setDir();
            anim.SetBool("isThrowReady", true);

            float waitTime = patterns[1].waitBeforeTime;
            if (i == 0) waitTime += 0.15f;
            if (isRage) waitTime -= 0.05f;

            curAttackWarning = attacks[1].ShowWarning(transform.position, Target.transform.position, waitTime);

            yield return new WaitForSeconds(waitTime);
            anim.SetBool("isThrowReady", false);

            anim.SetTrigger("doThrow");
            setDir((Target.transform.position - transform.position).normalized);
            yield return new WaitForSeconds(patterns[1].intervalTime);
        }
        yield return new WaitForSeconds(patterns[1].waitAfterTime);

        if (nextMove != null) StartCoroutine(nextMove);
        else selectPattern();
    }



    //실제 공격을 하는 함수. (Animation Event를 통해 호출)
    void doPat2()
    {
        Attack curProjectile = Instantiate(attacks[1], transform.position, Quaternion.identity);
        curProjectile.Shoot(transform.position, aim.transform.position);
    }

    IEnumerator co_Pat3()
    {
        anim.SetBool("isMoving", false);

        curAttackWarning = GameMgr.Inst.AttackEffectCircle(transform.position + Vector3.up * 0.3f, 1.5f, patterns[2].waitBeforeTime);
        yield return new WaitForSeconds(patterns[2].waitBeforeTime);
        anim.SetTrigger("doShout");
        SoundMgr.Inst.Play("Impact");
        yield return new WaitForSeconds(0.5f);
        spawnCompanion();
        selectPattern();
    }

    //실제 공격을 하는 함수. (Animation Event를 통해 호출)
    void doPat3()
    {
        Instantiate(attacks[2]).Shoot(transform.position, transform.position + Vector3.up * 0.3f);
        GameMgr.Inst.MainCam.Shake(0.4f, 20, 0.15f, 0, true);

    }

    Attack curAttack3;
    IEnumerator co_Pat4(IEnumerator nextMove = null)
    {
        anim.SetBool("isMoving", false);

        for (int i = 0; i < patterns[3].repeatTIme; i++)
        {
            curAttack3 = attacks[3];
            if(isHardMode)
            {
                if (Random.Range(0, 2) == 0) curAttack3 = attacks[4];
            }
            setDir();
            anim.SetBool("isThrowStrongReady", true);

            float waitTime = patterns[3].waitBeforeTime;

            curAttack3.ShowWarning(transform.position, Target.transform.position, waitTime);

            yield return new WaitForSeconds(patterns[3].waitBeforeTime);
            anim.SetBool("isThrowStrongReady", false);
            anim.SetTrigger("doThrowStrong");
            yield return new WaitForSeconds(patterns[3].waitAfterTime);
        }

        if (nextMove != null) StartCoroutine(nextMove);
        else selectPattern();
    }

    void doPat4()
    {
        Attack curProjectile = Instantiate<Attack>(curAttack3, transform.position, Quaternion.identity);
        curProjectile.Shoot(transform.position, aim.transform.position);
    }

    void spawnCompanion()
    {
        if (!isHardMode)
        {
            spawnMob(0, transform.position + Vector3.down, deadOption);
            spawnMob(1, deadOption);
        }
        else
        {
            spawnMob(0, transform.position + Vector3.down, deadOption, BUF.MEDAL);
            spawnMob(1, deadOption, BUF.TELEPORT);
        }
    }

    protected override void rageChange()
    {
        patterns[0].waitBeforeTime-= 0.1f;
        patterns[3].repeatTIme++;
        patterns[3].waitBeforeTime -= 0.15f;
    }

    #region 하드모드 추가패턴 
    public void ShootGroundBlock()
    {
        StartCoroutine(co_ShootGroundBlock());
    }
    float groundBlockWaitTime = 0.1f;
    IEnumerator co_ShootGroundBlock()
    {
        Vector3[] targetPositions = new Vector3[2] { transform.position.Randomize(3.0f), transform.position.Randomize(4.0f) };
        for (int i = 0; i < 2; i++) GroundBlock.ShowWarning(transform.position, targetPositions[i], groundBlockWaitTime);
        yield return new WaitForSeconds(groundBlockWaitTime);
        for (int i = 0; i < 2; i++) Instantiate(GroundBlock).Shoot(aim.transform.position, targetPositions[i]);

    }
    #endregion

}
