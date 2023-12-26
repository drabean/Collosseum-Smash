using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHairballHuge : EnemyBoss
{
    Attack[] attacks = new Attack[2];

    /// <summary>
    /// 패턴1: 근접공격
    /// 패턴2: 점프
    /// 패턴3: 돌진
    /// </summary>

    private void Awake()
    {
        attacks[0] = Resources.Load<Attack>(patterns[0].prefabName);
        attacks[1] = Resources.Load<Attack>(patterns[1].prefabName);

        evnt.attack += doAttack;
        evnt.attack2 += doJumpAttack;
    }
    public override void StartAI()
    {
        SpawnMob(Random.Range(0, 3));
        SpawnMob(Random.Range(0, 3));
        selectPattern();
    }

    int lastPatIdx = 0;

    protected override void selectPattern()
    {
        anim.SetBool("isMoving", false);

        //연속공격
        if (patternCount >= 1)
        {
            patternCount--;
            //같은 패턴이 연속으로 나오지 않도록 조절.
            lastPatIdx++;
            lastPatIdx %= 2;

            switch(lastPatIdx)
            {
                case 0:
                    StartCoroutine(co_Pat1());
                    break;
                case 1:
                    StartCoroutine(co_Pat2());
                    break;
            }
        }
        else
        {
            patternCount = 4 + Random.Range(0, 3);

            StartCoroutine(co_Fatigue(2.0f));
            //TODO: FATIQUEMOTION
        }

    }


    #region Pat1

    IEnumerator co_Pat1(float chaseTime = 3.0f)
    {
        while (chaseTime >= 0)
        {
            if (isDead) yield break;
            chaseTime -= Time.deltaTime;

            moveTowardTarget(Target.transform.position);

            if (Vector3.Distance(transform.position, Target.transform.position) < patterns[0].range)
            {
                chaseTime -= 2f;
                yield return StartCoroutine(co_Atk());
            }
            yield return null;
        }

        selectPattern();
    }

    IEnumerator co_Atk()
    {
        if (isDead) yield break;
        //방향 조절
        moveTowardTarget(Target.transform.position);

        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", true);
        curAttackWarning = attacks[0].ShowWarning(aim.position, aim.position, patterns[0].waitBeforeTime);
        yield return new WaitForSeconds(patterns[0].waitBeforeTime);
        anim.SetBool("isReady", false);

        anim.SetTrigger("doAttack");
        yield return new WaitForSeconds(patterns[0].waitAfterTime);

    }
    void doAttack()
    {
        Attack temp = Instantiate(attacks[0]);
        temp.Shoot(aim.position, aim.position);
        transform.position += (aim.transform.position - transform.position).normalized * aimRange;
        temp.GetComponent<SpriteRenderer>().flipX = sp.flipX;
    }

    #endregion
    #region Pat2
    IEnumerator co_Pat2()
    {
        //플레이어 주변 랜덤 범위 내에서 스테이지 내부의 위치.
        Vector3 destination = Target.transform.position + Random.Range(patterns[1].range, -patterns[1].range) * Vector3.right + Random.Range(patterns[1].range, -patterns[1].range) * Vector3.up;
        destination = EnemyMgr.Inst.getClampedVec(destination);

        setDir(destination - transform.position);
        yield return StartCoroutine(co_Jump(destination));

        anim.SetBool("isMoving", false);
        yield return new WaitForSeconds(patterns[1].waitAfterTime);
        selectPattern();
    }
    IEnumerator co_Jump(Vector3 destination)
    {
        attacks[1].ShowWarning(destination + Vector3.up * 0.5f, destination + Vector3.up * 0.5f, patterns[0].waitBeforeTime + 0.5f);

        yield return new WaitForSeconds(patterns[1].waitBeforeTime);
        SoundMgr.Inst.Play("Jump");
        anim.SetTrigger("doJump");
        float timeLeft = 0.5f;

        float jumpSpeed = Vector3.Distance(destination, transform.position) / 0.5f;

        while (timeLeft >= 0)
        {
            // moveTowardTarget(destination);
            transform.position = Vector3.MoveTowards(transform.position, destination, jumpSpeed  * Time.deltaTime);
            timeLeft -= Time.deltaTime;
            yield return null;
        }
    }

    void doJumpAttack()
    {
        GameMgr.Inst.MainCam.Shake(0.4f, 20, 0.15f, 0f);

        Attack temp = Instantiate(attacks[1]);
        temp.Shoot(transform.position, aim.position);
    }

    #endregion
    #region Pat3
    protected IEnumerator co_Pat3()
    {
        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", true);
        Vector3 chargeDir = (Target.transform.position - transform.position).normalized;
        Vector3 chargeDestination = transform.position + chargeDir * patterns[2].range;

        setDir(chargeDir);
        curAttackWarning = GameMgr.Inst.AttackEffectLinear(transform.position, chargeDestination, size * 2, patterns[2].waitBeforeTime);

        yield return new WaitForSeconds(patterns[2].waitBeforeTime);

        anim.SetBool("isReady", false);

        float chargeTimeLeft = patterns[2].duration;
        anim.SetTrigger("doCharge");
        while (chargeTimeLeft >= 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, chargeDestination, patterns[2].range * (1 / patterns[2].duration) * Time.deltaTime);
            chargeTimeLeft -= Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(patterns[2].waitAfterTime);

        selectPattern();
    }

    #endregion

    #region Fatique
    IEnumerator co_Fatigue(float fatigueTime = 3.0f)
    {
        SpawnMob(Random.Range(0, 3));
        SpawnMob(Random.Range(0, 3));
        anim.SetBool("isFatigue", true);

        float timeLeft = fatigueTime;

        subHP = maxHP / 4;
        while (subHP >= 0 && timeLeft >= 0)
        {
            timeLeft -= Time.deltaTime;

            yield return null;
        }
        anim.SetBool("isFatigue", false);
        selectPattern();
    }

    #endregion

    #region Mob Spawn
    int enemyCount = 0;
    public int maxEnemyCount = 3;
    public void SpawnMob(int idx)
    {
        if (enemyCount >= maxEnemyCount) return;

        enemyCount++;
        EnemyMgr.Inst.SpawnEnemy(mobs[idx], EnemyMgr.Inst.getRandomPos(), deadOption);    
    }
    void deadOption(Vector3 hitVec)
    {
        enemyCount--;
    }
    #endregion

}
