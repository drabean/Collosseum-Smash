using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlimeKing : EnemyBoss
{
    bool isImmune;
    [Header("컴포넌트 참조")]
    [SerializeField] SpriteRenderer eye;
    [SerializeField] Transform eyeTr;

    public List<Enemy> mobs = new List<Enemy>();
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

        patternCountLeft = Random.Range(2, patternCount + 1);
    }

    public override void  StartAI()
    {
        StartCoroutine(co_Idle());
    }

    protected override void selectPattern()
    {
        switch (patternCountLeft)
        {
            case > 0:
                patternCountLeft--;
                if (Random.Range(0, 2) == 0) StartCoroutine(co_Pat1());
                else StartCoroutine(co_Pat3());
                break;
            case 0:
                patternCountLeft = Random.Range(2, patternCount + 1);
                StartCoroutine(co_Pat2());
                break;
        }
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
            yield return new WaitForSeconds(patterns[0].intervalTime);
        }

        yield return co_Wait(patterns[0].waitAfterTime);
        EnemyMgr.Inst.SpawnEnemy(mobs[0], transform.position);
        StartCoroutine(co_Idle());
    }

    IEnumerator co_Move(Vector3 destination)
    {
        GameMgr.Inst.AttackEffectCircle(destination + Vector3.up * 0.5f, 2.3f, patterns[0].waitBeforeTime + 0.5f);

        yield return new WaitForSeconds(patterns[0].waitBeforeTime);
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
        GameObject attackEffect = DictionaryPool.Inst.Pop(patterns[0].prefabName);
        attackEffect.transform.position = transform.position + Vector3.up * 0.5f;
    }

    IEnumerator co_Pat2()
    {
        yield return co_Move(new Vector3(0, 3.5f, 0));

        eyeTr.transform.localPosition = Vector3.zero;
        eye.sortingOrder = 1;
        GameMgr.Inst.AttackEffectCircle(transform.position, 2.3f, 1.0f);
        yield return co_Wait(1.0f);
        onAttack1();

        float timeLeft = patterns[1].duration;
        anim.SetBool("isShaking", true);
        GameMgr.Inst.MainCam.Shake(patterns[1].duration, 20, 0.08f, 0, true);
        while(timeLeft >= 0)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        anim.SetBool("isShaking", false);

        yield return co_Wait(patterns[1].waitAfterTime);


        StartCoroutine(co_Idle());
    }

    void onAttack2()
    {
        SoundMgr.Inst.Play("Throw");

      
        StartCoroutine(co_attack2("Prefabs/Attack/SlimeExplosion", "Prefabs/Attack/SlimeExplosionBall"));
        StartCoroutine(co_attack2("Prefabs/Attack/SlimeExplosionGreen", "Prefabs/Attack/SlimeExplosionBallGreen"));
    }

    IEnumerator co_attack2(string attackName, string effectName)
    {
        //날아가는 슬라임 볼 생성
        Vector3 attackPos = EnemyMgr.Inst.getRandomPos();
        Projectile tempObj = DictionaryPool.Inst.Pop(effectName).GetComponent<Projectile>();
        tempObj.transform.position = transform.position;
        tempObj.moveSpeed = Vector3.Distance(transform.position, attackPos) / 0.75f;

        tempObj.Shoot(transform.position, attackPos);


        GameMgr.Inst.AttackEffectCircle(attackPos, 1.125f, 0.75f);
        yield return new WaitForSeconds(0.75f);
        //실제 공격 (폭팔) 생성
        GameObject attackEffect = DictionaryPool.Inst.Pop(attackName);
        attackEffect.transform.position = attackPos;
    }

    public Attack pat3Atk;
    IEnumerator co_Pat3()
    {
        List<Vector3> targetPositions = new List<Vector3>();
        yield return new WaitForSeconds(patterns[2].waitBeforeTime);

        for (int i = 0; i < patterns[2].repeatTIme; i++)
        {
            Vector3 targetPos = transform.position + Vector3.right * Random.Range(-1.0f, 1.0f) + Vector3.up * Random.Range(-1.0f, 1.0f);
            targetPositions.Add(targetPos);
            pat3Atk.ShowWarning(transform.position, targetPos, patterns[2].waitBeforeTime);
        }

        yield return co_Move(transform.position);
        foreach(Vector3 targetPos in targetPositions)
        {
            Instantiate<Attack>(pat3Atk, transform.position, Quaternion.identity).Shoot(transform.position, targetPos);
        }

        EnemyMgr.Inst.SpawnEnemy(mobs[1], transform.position);
        StartCoroutine(co_Idle(patterns[2].waitAfterTime));
    }

    public override void onHit(Transform attackerPos, float dmg, float stunTime = 0.3F)
    {
        if(isImmune)
        {
            hit.DmgTxt("IMMUNE!");
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