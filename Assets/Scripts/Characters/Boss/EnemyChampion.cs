using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChampion : EnemyBoss
{
    Attack[] attacks = new Attack[4];

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
    }


    public override void StartAI()
    {
        selectPattern();
    }

    /// <summary>
    /// ���� �ൿ�� ����
    /// </summary>
    protected override void selectPattern()
    {
        if(patternCountLeft > 1)
        {

            int patIdx = Random.Range(0, 4);
            float dist = Vector3.Distance(transform.position, Target.transform.position);

            switch (patIdx)
            {
                case 0: // ��������
                    patternCountLeft--;
                    if (dist > 1.5f) StartCoroutine(co_Chase(co_Pat1())); // ������� �Ÿ��� �ִٸ� �߰� �� ����
                    else StartCoroutine(co_Runaway(co_Chase(co_Pat1()))); // �ʹ� �����ٸ�, �Ÿ��� ���� �Ŀ� �߰� �� ����
                    break;
                case 1: // ���Ÿ� ����
                    patternCountLeft--;
                    if (dist > 3) StartCoroutine(co_Wander(co_Pat2())); // ������� �Ÿ��� �ִٸ� �ڸ����� �� ����
                    else StartCoroutine(co_Runaway(co_Wander(co_Pat2()), 0.6f)); // �ʹ� �����ٸ� �Ÿ��� ���� �Ŀ� ����
                    break;
                case 2:
                    patternCountLeft--;
                    StartCoroutine(co_Runaway(co_Wander(co_Pat4()), 0.4f));
                    break;
                case 3:
                    if (dist < 1.5f) StartCoroutine(co_Runaway(co_Wander(), 0.2f)); // ȥ���� ���� ������, �Ÿ��� ��� ������ ���η� �̵�
                    else selectPattern();
                    break;
            }
            
        }
        else if(patternCountLeft == 1)
        {
            patternCountLeft--;
            StartCoroutine(co_Chase(co_Pat3()));
        }
        else // ���� ���� ���� �� �׷α�
        {
            patternCountLeft = patternCount;
            StartCoroutine(co_Fatigue());
        }
    }

    IEnumerator co_Fatigue() // �׷α� ����
    {
        anim.SetBool("isFatigue", true);
        anim.SetBool("isMoving", false);

        float timeLeft = patterns[2].waitAfterTime;
        subHP = maxHP / 5;
        while (subHP >= 0 && timeLeft >= 0)
        {
            timeLeft -= Time.deltaTime;

            yield return null;
        }
        anim.SetBool("isFatigue", false);
        selectPattern();
    }

    #endregion

    #region �̵�

    //Ÿ���� ���ؼ� �̵� (����)
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
    //Ÿ���� �ݴ�������� �̵� (����)
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
    //Target�� �߽����� �������� �̵�
    IEnumerator co_Wander(IEnumerator nextMove = null)
    {
        moveSpeed = 5f;
        float wanderTime = Random.Range(0.6f, 1.0f);//�̵��� �ð��� ����
        bool isReversed = (Random.Range(0, 2) == 0);//ȸ�� ������ ����

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
    //Patterm1 - ��������
    IEnumerator co_Pat1(IEnumerator nextMove = null)
    {
        anim.SetBool("isMoving", false);

        for(int i = 0; i < patterns[0].repeatTIme-1; i++)
        {
            aimRange = 0.8f;
            setDir();
            float waitTime = patterns[0].waitBeforeTime;
            if (i == 0) waitTime += 0.2f;

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
        anim.SetBool("isAtkReady", true);
        yield return new WaitForSeconds(0.6f);
        anim.SetBool("isAtkReady", false);

        anim.SetTrigger("doAttack");

        if(isRage)
        {
            curAttackWarning = attacks[0].ShowWarning(aim.position, aim.position, 0.6f);
            anim.SetBool("isAtkReady", true);
            yield return new WaitForSeconds(Random.Range( 0.5f, 0.8f));
            anim.SetBool("isAtkReady", false);

            anim.SetTrigger("doAttack");
        }
        yield return new WaitForSeconds(patterns[0].waitAfterTime);

        if (nextMove != null) StartCoroutine(nextMove);
        else selectPattern();
    }

    //���� ������ �ϴ� �Լ�. (Animation Event�� ���� ȣ��)
    void doPat1()
    {
        Instantiate(attacks[0]).Shoot(aim.position, aim.position);
        transform.position += (aim.position - transform.position).normalized * 0.5f; //���� �� �� ����, ���� �������� �ణ �̵�

        GameMgr.Inst.MainCam.Shake(0.1f, 40, 0.2f, 0f);
    }
    //Pattern2 - ��â����
    IEnumerator co_Pat2(IEnumerator nextMove = null)
    {
        anim.SetBool("isMoving", false);


        for (int i = 0; i < patterns[1].repeatTIme; i++)
        {
            setDir();
            anim.SetBool("isThrowReady", true);

            float waitTime = patterns[1].waitBeforeTime;
            if (i == 0) waitTime += 0.15f;
            if (isRage) waitTime -= 0.1f;

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



    //���� ������ �ϴ� �Լ�. (Animation Event�� ���� ȣ��)
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

    //���� ������ �ϴ� �Լ�. (Animation Event�� ���� ȣ��)
    void doPat3()
    {
        Instantiate(attacks[2]).Shoot(transform.position, transform.position + Vector3.up * 0.3f);
        GameMgr.Inst.MainCam.Shake(0.4f, 20, 0.15f, 0, true);

    }
    IEnumerator co_Pat4(IEnumerator nextMove = null)
    {
        anim.SetBool("isMoving", false);

        for (int i = 0; i < patterns[3].repeatTIme; i++)
        {

        
        setDir();
        anim.SetBool("isThrowStrongReady", true);

        float waitTime = patterns[3].waitBeforeTime;

        curAttackWarning = attacks[3].ShowWarning(transform.position, Target.transform.position, waitTime);

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
        Attack curProjectile = Instantiate<Attack>(attacks[3], transform.position, Quaternion.identity);
        curProjectile.Shoot(transform.position, aim.transform.position);
    }

    void spawnCompanion()
    {
        spawnMob(0, deadOption);
        spawnMob(1, deadOption);
    }

    protected override void rageChange()
    {
        patterns[3].repeatTIme++;
        patterns[3].waitBeforeTime -= 0.15f;
    }
}
