using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlimeKing : Enemy
{
    bool isImmune;
    [Header("컴포넌트 참조")]
    [SerializeField] SpriteRenderer eye;
    [SerializeField] Transform eyeTr;

    [Header("패턴1")]
    public float pat1Width;//공격 이펙트 타격판정 너비
    public string pat1AtkName;
    public float pat1WaitBeforeTIme;
    public float pat1WaitAfterTime;

    public float pat1IntervalTime;
    public int pat1RepeatTime;

    [Header("패턴2")]
    public float pat2Range;
    public string pat2AtkName;
    public float pat2Duration;
    public float pat2WaitAfterTime;

    [Header("패턴3")]
    public float pat3WaitAfterTIme;

    int patternCount = 3;
    int patternCountLeft = 3;


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

        switch(patternCountLeft)
        {
            case > 1:
                patternCountLeft--;
                StartCoroutine(co_Pat1());
                break;
            case 1:
                patternCountLeft--;
                StartCoroutine(co_Pat2());
                break;
            case 0:
                patternCountLeft = Random.Range(2, patternCount+1);
                StartCoroutine(co_Pat3());
                break;
        }
    }

    IEnumerator co_Pat1()
    {
        for(int i = 0; i < pat1RepeatTime; i++)
        {
            setDir();
            yield return StartCoroutine(co_Move(Target.transform.position));
            yield return new WaitForSeconds(pat1IntervalTime);
        }

        yield return co_Wait(pat1WaitAfterTime);
        StartCoroutine(co_Idle());
    }

    IEnumerator co_Move(Vector3 destination)
    {
        GameMgr.Inst.AttackEffectCircle(destination + Vector3.up * 0.5f, 2.3f, pat1WaitBeforeTIme + 0.5f);

        yield return new WaitForSeconds(pat1WaitBeforeTIme);
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
        GameMgr.Inst.Shake(0.4f, 20, 0.15f, 0, true);
        GameObject attackEffect = DictionaryPool.Inst.Pop(pat1AtkName);
        attackEffect.transform.position = transform.position + Vector3.up * 0.5f;
    }

    IEnumerator co_Pat2()
    {
        eyeTr.transform.localPosition = Vector3.zero;
        eye.sortingOrder = 1;
        GameMgr.Inst.AttackEffectCircle(transform.position, 2.3f, 1.0f);
        yield return co_Wait(1.0f);
        onAttack1();

        float timeLeft = pat2Duration;
        anim.SetBool("isShaking", true);
        GameMgr.Inst.Shake(pat2Duration, 20, 0.08f, 0, true);
        while(timeLeft >= 0)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        anim.SetBool("isShaking", false);

        yield return co_Wait(pat2WaitAfterTime);

        StartCoroutine(co_Idle());
    }

    int attack2Count = 0;
    void onAttack2()
    {
        ++attack2Count;
        attack2Count %= 2;
        StartCoroutine(co_attack2("Prefabs/Attack/SlimeExplosion", "Prefabs/Attack/SlimeExplosionBall"));
        StartCoroutine(co_attack2("Prefabs/Attack/SlimeExplosionGreen", "Prefabs/Attack/SlimeExplosionBallGreen"));
    }

    IEnumerator co_attack2(string attackName, string effectName)
    {
        Vector3 attackPos = EnemyMgr.Inst.getRandomPos();
        Projectile tempObj = DictionaryPool.Inst.Pop(effectName).GetComponent<Projectile>();
        tempObj.transform.position = transform.position;
        tempObj.moveSpeed = Vector3.Distance(transform.position, attackPos) / 0.75f;

        tempObj.Shoot(transform.position, attackPos);


        GameMgr.Inst.AttackEffectCircle(attackPos, 1.125f, 0.75f);
        yield return new WaitForSeconds(0.75f);
        GameObject attackEffect = DictionaryPool.Inst.Pop(attackName);
        attackEffect.transform.position = attackPos;
    }


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
            EnemyMgr.Inst.SpawnEnemy(EnemyPrefabs[Random.Range(0, EnemyPrefabs.Count)], cornerPoints[i], enemyDeadOption);
            yield return null;
        }
        while(spawnedEnemyCount > 0)
        {
            yield return co_Wait(1.0f);
        }

        anim.SetBool("isImmune", false);
        isImmune = false;
        yield return new WaitForSeconds(pat3WaitAfterTIme);

        StartCoroutine(co_Idle());
    }

    void enemyDeadOption()
    {
        spawnedEnemyCount--;
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
}
