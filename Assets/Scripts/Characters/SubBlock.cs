using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 독립된 적이 아닌, KingBlock 보스에 종속되는 몬스터 입니다.
/// </summary>
public class SubBlock : Enemy
{
    /// <summary>
    /// 패턴 정보
    /// </summary>
    public PatternInfo[] patterns;
    public EnemyKingBlock Owner;

    Attack[] attacks = new Attack[6];

    float xPos;
    float[] yRange = new float[] { 2.8f, -5.6f };

    private void Awake()
    {
        evnt.attack += onAttack1;

        attacks[0] = Resources.Load<Attack>(patterns[0].prefabName);
        attacks[1] = Resources.Load<Attack>(patterns[1].prefabName);
        attacks[2] = Resources.Load<Attack>(patterns[3].prefabName);

    }

    public void Init(EnemyKingBlock Owner, Player target)
    {
        this.Owner = Owner;
        Target = target;
        xPos = transform.position.x;
    }
    protected override void setDir(Vector3 dir)
    {
        dir = dir.normalized;
        aim.transform.localPosition = dir * aimRange;
    }

    public override void onHit(Transform attackerPos, float dmg, float stunTime = 0.3F)
    {
        Vector3 hitVec = (transform.position - attackerPos.position).normalized;

        hit.FlashWhite(0.2f);
        hit.HitEffect(hitVec, size);

        if (Owner == null) return;
        Owner.onHit(attackerPos, dmg, stunTime);
    }

    #region 이동
    IEnumerator co_Move(Vector3 destination)
    {
        yield return new WaitForSeconds(patterns[0].waitBeforeTime);
        SoundMgr.Inst.Play("Jump");
        anim.SetTrigger("doMove");
        float timeLeft = 0.5f;

        moveSpeed = Vector3.Distance(destination, transform.position) / 0.5f;

        while (timeLeft >= 0)
        {
            moveTowardTarget(destination);
            setDir();
            timeLeft -= Time.deltaTime;
            yield return null;
        }
    }


    #endregion

    #region 스매쉬
    void onAttack1()
    {
        GameMgr.Inst.MainCam.Shake(0.4f, 20, 0.15f, 0f);
        Instantiate(attacks[0]).Shoot(transform.position + Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f);
    }

    public IEnumerator co_Smash()
    {
        Vector3 originVec = transform.position;

        setDir();
        yield return StartCoroutine(co_Atk1(Target.transform.position));
        yield return new WaitForSeconds(patterns[0].intervalTime);
        StartCoroutine(co_Move(originVec));
    }

    IEnumerator co_Atk1(Vector3 destination)
    {
        attacks[0].ShowWarning(destination + Vector3.up * 0.5f, destination + Vector3.up * 0.5f, patterns[0].waitBeforeTime + 0.5f);

        yield return new WaitForSeconds(patterns[0].waitBeforeTime);
        SoundMgr.Inst.Play("Jump");
        anim.SetTrigger("doPat1");
        float timeLeft = 0.5f;

        moveSpeed = Vector3.Distance(destination, transform.position) / 0.5f;

        while (timeLeft >= 0)
        {
            moveTowardTarget(destination);
            setDir();
            timeLeft -= Time.deltaTime;
            yield return null;
        }
    }

    #endregion


    #region 발사 패턴
    float missileWaitTime = 0.3f;
    public Transform firePos;

    /// <summary>
    /// 발사계열 1 - 플레이어를 짧게 조준 - 발사 반복
    /// </summary>
    /// <returns></returns>
    public IEnumerator co_Fire1(float duration)
    {
        yield return StartCoroutine(co_Move(Vector3.right * xPos + Vector3.up * Random.Range(yRange[0], yRange[1])));
        anim.SetBool("isPat2", true);

        yield return new WaitForSeconds(patterns[1].waitBeforeTime);

        float startTime = Time.time;
        while (Time.time - startTime <= duration)
        {
            Vector3 targetVec = Target.transform.position ;
            attacks[1].ShowWarning(firePos.position, targetVec, missileWaitTime);
            //각 공격들의 발사 / 목표 위치 계산
            yield return new WaitForSeconds(0.3f);
            GameMgr.Inst.MainCam.Shake(0.2f, 10, 0.1f, 0f);
            SoundMgr.Inst.Play("Throw");
            yield return new WaitForSeconds(missileWaitTime);
            GameMgr.Inst.MainCam.Shake(0.1f, 10, 0.05f, 0f);
            Instantiate(attacks[1]).Shoot(firePos.position, targetVec);
            anim.SetTrigger("doPat2");

            yield return new WaitForSeconds(patterns[1].intervalTime - missileWaitTime);
        }
        anim.SetBool("isPat2", false);
        yield return new WaitForSeconds(patterns[1].waitAfterTime);
    }

    float rapidWaitTime = 0.1f;

    /// <summary>
    /// 발사 계열 2 - 플레이어를 길게 조준 - 3발 점사 반복 *정화갛게
    /// </summary>
    /// <returns></returns>
    public IEnumerator co_Fire2(float duration)
    {
        yield return StartCoroutine(co_Move(Vector3.right * xPos + Vector3.up * Random.Range(yRange[0], yRange[1])));
        anim.SetBool("isPat2", true);

        yield return new WaitForSeconds(patterns[1].waitBeforeTime);

        float startTime = Time.time;
        while (Time.time - startTime <= duration)
        { 
            Vector3 targetVec = Target.transform.position;
            attacks[1].ShowWarning(firePos.position, targetVec, missileWaitTime);
            //각 공격들의 발사 / 목표 위치 계산
            yield return new WaitForSeconds(0.3f);
            for (int i = 0; i < patterns[2].repeatTIme; i++)
            {
                yield return new WaitForSeconds(0.1f);
                GameMgr.Inst.MainCam.Shake(0.2f, 10, 0.1f, 0f);
                SoundMgr.Inst.Play("Throw");
                Instantiate<Attack>(attacks[1]).Shoot(firePos.position, targetVec);
                anim.SetTrigger("doPat2");
            }
            yield return new WaitForSeconds(missileWaitTime + rapidWaitTime * patterns[2].repeatTIme);

            yield return new WaitForSeconds(patterns[1].intervalTime - missileWaitTime);
        }

        anim.SetBool("isPat2", false);
        yield return new WaitForSeconds(patterns[1].waitAfterTime);
    }

    #endregion

    #region 레이져패턴

    public IEnumerator co_Laser()
    {
        Vector3 targetVec = Target.transform.position;

        yield return StartCoroutine(co_Move(Vector3.right * xPos + Vector3.up * Target.transform.position.y));
        attacks[2].ShowWarning(transform.position, targetVec, patterns[3].waitBeforeTime);

        yield return new WaitForSeconds(patterns[3].waitBeforeTime);

        Instantiate(attacks[2]).Shoot(transform.position, targetVec);
    }

    #endregion
}
