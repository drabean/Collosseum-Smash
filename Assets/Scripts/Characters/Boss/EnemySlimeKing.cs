using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlimeKing : EnemyBoss
{
    bool isImmune;
    [Header("컴포넌트 참조")]
    [SerializeField] SpriteRenderer eye;
    [SerializeField] Transform eyeTr;

    Attack[] attacks = new Attack[4];
    int patIdx = 0;

    float slimeballMoveTime = 0.85f;
    protected override void setDir(Vector3 dir)
    {
        eye.sortingOrder = dir.y < 0 ? 1 : -1;
        eyeTr.localPosition = Vector3.right * 0.4f * dir.x + Vector3.up * (0.4f * dir.y + 0.6f);
        aim.transform.localPosition = dir * aimRange;
    }

    private void Awake()
    {
        evnt.attack += onAttack1;
        evnt.attack2 += onAttack2;
        evnt.attack4 += onAttack4;

        patternCountLeft = Random.Range(2, patternCount + 1);

        attacks[0] = Resources.Load<Attack>(patterns[0].prefabName);
        attacks[1] = Resources.Load<Attack>(patterns[2].prefabName);
        attacks[2] = Resources.Load<Attack>("Prefabs/Attack/SlimeExplosionBall");
        attacks[3] = Resources.Load<Attack>("Prefabs/Attack/SlimeExplosionBallGreen");

    }

    public override void  StartAI()
    {
        StartCoroutine(co_Idle());
    }

    protected override void selectPattern()
    {
        
        patIdx += Random.Range(1, 3);
        patIdx %= 3;

        if(isRagePattern)
        {
            isRagePattern = false;
            StartCoroutine(co_Pat4());
            return;
        }
        switch(patIdx)
        {
            case 0:
                StartCoroutine(co_Pat1());
                break;
            case 1:
                StartCoroutine(co_Pat2());
                break;
            case 2:
                StartCoroutine(co_Pat3());
                break;
        }
    }

    protected override void rageChange()
    {
        maxSpawnCount++;
        slimeballMoveTime -= 0.2f;
        patterns[0].waitBeforeTime -= 0.15f;
        patterns[0].intervalTime = -0.35f;
        patterns[2].repeatTIme += 2;
        patterns[2].waitBeforeTime -= 0.15f;
    }
    IEnumerator co_Wait(float timeleft)
    {
        while(timeleft >= 0)
        {
            timeleft -= Time.deltaTime;
            setDir();
            yield return null;
        }
    }

    IEnumerator co_Idle(float time = 1.5f)
    {
        float timeLeft = time;

        while(timeLeft >= 0)
        {
            setDir();

            timeLeft -= Time.deltaTime;
            yield return null;
        }

        selectPattern();
    }

    IEnumerator co_Pat1()
    {
        for(int i = 0; i < patterns[0].repeatTIme; i++)
        {
            setDir();
            yield return StartCoroutine(co_Move(Target.transform.position));
            if (isHardMode) ShootParabolaSlime();
            yield return new WaitForSeconds(patterns[0].intervalTime);
        }

        yield return co_Wait(patterns[0].waitAfterTime);
        spawnMob(0, deadOption);
        StartCoroutine(co_Idle());
    }

    IEnumerator co_Move(Vector3 destination, bool isFirstJump = false)
    {
        float waitTime = patterns[0].waitBeforeTime;
        if (isFirstJump) waitTime += 0.5f;
        attacks[0].ShowWarning(destination + Vector3.up * 0.5f, destination + Vector3.up * 0.5f, waitTime + 0.5f);

        yield return new WaitForSeconds(waitTime);
        SoundMgr.Inst.Play("Jump");
        anim.SetBool("isMoving", true);
        float timeLeft = 0.5f;

        moveSpeed = Vector3.Distance(destination, transform.position) / 0.5f;

        while (timeLeft >= 0)
        {
            moveTowardTarget(destination);
            setDir();
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        anim.SetBool("isMoving", false);
    }

    void onAttack1()
    {
        GameMgr.Inst.MainCam.Shake(0.4f, 20, 0.15f, 0f);
        Instantiate(attacks[0]).Shoot(transform.position + Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f);
    }

    IEnumerator co_Pat2()
    {
        eyeTr.transform.localPosition = Vector3.zero;
        eye.sortingOrder = 1;
        float waitTime = 1.0f;
        if (isRage) waitTime -= 0.3f;
        yield return co_Wait(waitTime);


        float timeLeft = patterns[1].duration;
        anim.SetBool("isShakingWeak", true);
        if (isHardMode) ShootParabolaSlime();
        GameMgr.Inst.MainCam.Shake(patterns[1].duration, 20, 0.08f, 0, true);
        isImmune = true;
        while(timeLeft >= 0)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        isImmune = false;
        anim.SetBool("isShakingWeak", false);

        yield return co_Wait(patterns[1].waitAfterTime);


        StartCoroutine(co_Idle());
    }

    void onAttack2()
    {
        SoundMgr.Inst.Play("Throw");

        float randomRange = 2.5f;

        StartCoroutine(co_attack2(attacks[2], Target.transform.position.Randomize(randomRange)));
        StartCoroutine(co_attack2(attacks[2], Target.transform.position.Randomize(randomRange)));
        StartCoroutine(co_attack2(attacks[3], Target.transform.position.Randomize(randomRange)));
    }

    IEnumerator co_attack2(Attack slimeBall, Vector3 attackPos)
    {
        slimeBall.ShowWarning(attackPos, attackPos, slimeballMoveTime + 0.15f);
        yield return new WaitForSeconds(0.15f);
        //날아가는 슬라임 볼 생성
        ProjectileParabola tempObj = (ProjectileParabola)Instantiate(slimeBall);
        tempObj.moveTime = slimeballMoveTime;
        tempObj.Shoot(transform.position, attackPos);
    }

    IEnumerator co_Pat3()
    {
        List<Vector3> targetPositions = new List<Vector3>();
        yield return new WaitForSeconds(patterns[2].waitBeforeTime);

        targetPositions.Add(Target.transform.position);
        for (int i = 0; i < patterns[2].repeatTIme; i++)
        {
            Vector3 targetPos = transform.position + Vector3.right * Random.Range(-1.0f, 1.0f) + Vector3.up * Random.Range(-1.0f, 1.0f);
            targetPositions.Add(targetPos);
        }

        for (int i = 0; i < targetPositions.Count; i++)
        {
            attacks[1].ShowWarning(transform.position, targetPositions[i], patterns[2].waitBeforeTime);
        }
        yield return co_Move(transform.position);
        if (isHardMode) ShootParabolaSlime();
        foreach (Vector3 targetPos in targetPositions)
        {
            Instantiate(attacks[1], transform.position, Quaternion.identity).Shoot(transform.position, targetPos);
        }

        spawnMob(1, deadOption);
        StartCoroutine(co_Idle(patterns[2].waitAfterTime));
    }

    public override void onHit(Transform attackerPos, float dmg, float stunTime = 0.3F)
    {
        if(isImmune)
        {
            base.onHit(attackerPos, 1, stunTime);
            hit.FlashWhite(0.1f);
            return;
        }
        else
        {
            base.onHit(attackerPos, dmg, stunTime);
        }
    }
    protected override void stopAction()
    {
        base.stopAction();
        anim.SetBool("isShaking", false);
    }

    #region 어려운패턴

    IEnumerator co_Pat4()
    {
        yield return co_Move(new Vector3(0, 3.5f, 0));

        eyeTr.transform.localPosition = Vector3.zero;
        eye.sortingOrder = 1;
        GameMgr.Inst.AttackEffectCircle(transform.position, 2.3f, 1.0f);
        yield return co_Wait(1.0f);
        onAttack1();

        float timeLeft = patterns[3].duration;
        anim.SetBool("isShaking", true);
        GameMgr.Inst.MainCam.Shake(patterns[3].duration, 20, 0.08f, 0, true);
        isImmune = true;
        while (timeLeft >= 0)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        isImmune = false;
        anim.SetBool("isShaking", false);

        yield return co_Wait(patterns[3].waitAfterTime);


        StartCoroutine(co_Idle());
    }

    int atk4Idx = 0;
    void onAttack4()
    {
        SoundMgr.Inst.Play("Throw");
        if (atk4Idx >= 2)
        {
            atk4Idx = 0;
            StartCoroutine(co_attack2(attacks[2], Target.transform.position.Randomize(2.0f)));
            StartCoroutine(co_attack2(attacks[3], Target.transform.position.Randomize(2.0f)));
        }
        else
        {

            StartCoroutine(co_attack2(attacks[2], EnemyMgr.Inst.getRandomPos()));
            StartCoroutine(co_attack2(attacks[2], EnemyMgr.Inst.getRandomPos()));
            StartCoroutine(co_attack2(attacks[3], EnemyMgr.Inst.getRandomPos()));
        }
        atk4Idx++;
    }

    #endregion

    public void ShootParabolaSlime()
    {
        StartCoroutine(co_ShootParabolaSlime());
    }

    float slimeBallWaitTIme = 0.1f;
    IEnumerator co_ShootParabolaSlime()
    {
        Vector3[] targetPositions = new Vector3[3] { transform.position.Randomize(3.0f), transform.position.Randomize(3.0f), transform.position.Randomize(3.0f) };
        for (int i = 0; i < 3; i++) attacks[2].ShowWarning(transform.position, targetPositions[i], slimeBallWaitTIme);
        yield return new WaitForSeconds(slimeBallWaitTIme);
        for (int i = 0; i < 3; i++) Instantiate(attacks[2]).Shoot(transform.position, targetPositions[i]);

    }
}

/*LEGACY
 * 
 * 
    public List<Enemy> EnemyPrefabs;
    int spawnedEnemyCount = 0;
    Vector3 returnVec = new Vector3(0, 3.5f, 0);
    IEnumerator co_Pat3()
    {
        yield return co_Move(returnVec);

        List<Vector2> cornerPoints = EnemyMgr.Inst.getCornerPos();

        yield return new WaitForSeconds(0.5f);

        eyeTr.transform.localPosition = Vector3.zero;
        eye.sortingOrder = 1;

        anim.SetBool("isImmune", true);
        isImmune = true;
        for (int i = 0; i < 4; i++)
        {
            spawnedEnemyCount++;
            EnemyMgr.Inst.SpawnEnemy(EnemyPrefabs[i % 3], cornerPoints[i], enemyDeadOption);
            yield return null;
        }
        while(spawnedEnemyCount > 0)
        {
            yield return co_Wait(1.0f);
        }

        anim.SetBool("isImmune", false);
        isImmune = false;
        yield return new WaitForSeconds(patterns[2].waitAfterTime);

        StartCoroutine(co_Idle());
    }

    void enemyDeadOption()
    {
        spawnedEnemyCount--;
    }
 * */