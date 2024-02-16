using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGoblinKing : EnemyBoss
{
    Attack[] attacks = new Attack[3];

    private void Awake()
    {
        attacks[0] = Resources.Load<Attack>(patterns[0].prefabName);
        evnt.attack = doAttack;
        evnt.attack2 = doSpawn;
        float lastSpawnedTime = Time.time + 3; // 첫 소환 시간을 앞당기기 위한 마지막 소환 시간 조절
    }


    public override void StartAI()
    {
        StartCoroutine(co_Chase());
    }

    public float SpawnCoolTime;
    float lastSpawnedTime;
    IEnumerator co_Chase()
    {
        while(true)
        {
            if (isRagePattern)
            {
                isRagePattern = false;
                yield return StartCoroutine(co_SpawnBombs());
            }

            // 적 소환 가능 시간이 되었다면
            if (Time.time - lastSpawnedTime >= SpawnCoolTime)
            {
                yield return StartCoroutine(co_SpawnMob());
                lastSpawnedTime = Time.time;

            }
            moveTowardTarget(Target.transform.position);

            if(Vector3.Distance(transform.position, Target.transform.position) < patterns[0].range)
            {
                yield return StartCoroutine(co_Atk());
            }
            yield return null;
        }

    }

    IEnumerator co_SpawnBombs()
    {
        anim.SetTrigger("doSpawn");
        yield return new WaitForSeconds(1.0f);

        EnemyMgr.Inst.SpawnEnemy(mobs[2], EnemyMgr.Inst.getCornerPos()[0]);
        EnemyMgr.Inst.SpawnEnemy(mobs[2], EnemyMgr.Inst.getCornerPos()[1]);
        EnemyMgr.Inst.SpawnEnemy(mobs[2], EnemyMgr.Inst.getCornerPos()[2]);
        EnemyMgr.Inst.SpawnEnemy(mobs[2], EnemyMgr.Inst.getCornerPos()[3]);

        yield return new WaitForSeconds(patterns[1].waitAfterTime);

        float runAwayTimeLeft = patterns[1].duration;

        while (runAwayTimeLeft >= 0)
        {
            runAwayTimeLeft -= Time.deltaTime;
            moveToDir( transform.position - Target.transform.position);
            yield return null;
        }
    }
    IEnumerator co_Atk()
    {
        if (isDead) yield break;

        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", true);
        curAttackWarning = attacks[0].ShowWarning(transform.position, aim.position, patterns[0].waitBeforeTime);
        yield return new WaitForSeconds(patterns[0].waitBeforeTime);
        anim.SetBool("isReady", false);

        anim.SetTrigger("doAttack");
        yield return new WaitForSeconds(patterns[0].waitAfterTime);
    }

    IEnumerator co_SpawnMob()
    {
        if (curSpawnCount > maxSpawnCount) yield break;
        anim.SetTrigger("doSpawn");

        yield return new WaitForSeconds(3.0f);
    }
    void doAttack()
    {
        Attack temp = Instantiate(attacks[0]);
        temp.Shoot(transform.position, aim.position);

        temp.GetComponent<SpriteRenderer>().flipX = sp.flipX;
    }


    void doSpawn()
    {
        int idx = Random.Range(0, 2);

        switch(idx)
        {
            case 0:
                spawnMob(0, transform.position + Vector3.right * 1.0f, deadOption);
                spawnMob(0, transform.position + Vector3.right * -1.0f, deadOption);
                if(isRage) spawnMob(2, transform.position + Vector3.down * 0.5f, deadOption);
                break;
            case 1:
                spawnMob(0, transform.position + Vector3.down * 2.0f, deadOption);
                spawnMob(1, deadOption);
                if (isRage) spawnMob(2, transform.position + Vector3.down * 0.5f, deadOption);
                break;
        }

    }

    protected override void rageChange()
    {
        maxSpawnCount = 4;
    }
}
