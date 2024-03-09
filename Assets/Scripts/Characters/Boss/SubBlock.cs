using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 독립된 적이 아닌, KingBlock 보스에 종속되는 몬스터
/// </summary>
public class SubBlock : Enemy
{
    /// <summary>
    /// 패턴 정보
    /// </summary>
    public PatternInfo[] patterns;
    public EnemyKingBlock Owner;

    Attack[] attacks = new Attack[6];
    [SerializeField] SpriteRenderer face;
    [SerializeField] Transform LaserPos;
    float xPos;
    float[] yRange = new float[] { 2.8f, -5.6f };


    private void Awake()
    {
        evnt.attack += onAttack1;

        attacks[0] = Resources.Load<Attack>(patterns[0].prefabName);
        attacks[1] = Resources.Load<Attack>(patterns[1].prefabName);
        attacks[2] = Resources.Load<Attack>(patterns[3].prefabName);

    }
    //소환 연출 (본체에서 호출)
    public void Spawn()
    {
        hit.FlashWhite(0.1f);
        anim.SetTrigger("doSpawn");
    }
    //제거 연출 (본체에서 호출)
    public void Destroy()
    {
        StartCoroutine(co_Destroy());
    }

    IEnumerator co_Destroy()
    {
        GetComponent<Collider2D>().enabled = false;
        hit.FlashWhite(3.0f);
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);
    }



    public void Init(EnemyKingBlock Owner, Player target)
    {
        this.Owner = Owner;
        Target = target;
        xPos = transform.position.x;
        Owner.faceSprites.Add(face);
        Owner.faceChangeAction += () => { anim.SetTrigger("doFaceChange"); };
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

    /// <summary>
    /// 
    /// </summary>
    public void MoveToStartpos()
    {
        StartCoroutine(co_Move(Vector3.right * xPos + Vector3.up * -0.6f));
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
        anim.SetTrigger("doStomp");
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
   public float fire1WaitTime = 0.7f;
    public Transform firePos;

    /// <summary>
    /// 발사계열 1 - 플레이어를 짧게 조준 - 발사 반복
    /// </summary>
    /// <returns></returns>
    public IEnumerator co_Fire1(float duration)
    {
        yield return new WaitForSeconds(1.0f);

        anim.SetBool("isShootLear", true);

        yield return new WaitForSeconds(patterns[1].waitBeforeTime);
        float timeLeft = duration-1;
        float fireTimeLeft = 0.0f;

        while (timeLeft > 0)
        {
            //각 공격들의 발사 / 목표 위치 계산
            if(fireTimeLeft < 0)
            {
                GameMgr.Inst.MainCam.Shake(0.2f, 10, 0.1f, 0f);

                GameMgr.Inst.MainCam.Shake(0.1f, 10, 0.05f, 0f);
                Instantiate(attacks[1]).Shoot(firePos.position, Target.transform.position);
                anim.SetTrigger("doShootLear");
                fireTimeLeft = fire1WaitTime;
            }

            timeLeft -= Time.deltaTime;
            fireTimeLeft -= Time.deltaTime;
            yield return null;
        }
        anim.SetBool("isShootLear", false);
    }

    public float fire2WaitTime = 0.9f;
    float rapidWaitTime = 0.1f;

    /// <summary>
    /// 발사 계열 2 - 플레이어를 길게 조준 - 3발 점사 반복 *정화갛게
    /// </summary>
    /// <returns></returns>
    public IEnumerator co_Fire2(float duration)
    {
        yield return new WaitForSeconds(1.0f);

        anim.SetBool("isShootLear", true);

        yield return new WaitForSeconds(patterns[1].waitBeforeTime);

        float timeLeft = duration-1; // 이동모션 위해  대기한만큼 뺴줌
        float fireTimeLeft = 0.0f;

        while (timeLeft > 0)
        { 
            Vector3 targetVec = Target.transform.position;

            if (fireTimeLeft < 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    GameMgr.Inst.MainCam.Shake(0.2f, 10, 0.1f, 0f);
                    Instantiate<Attack>(attacks[1]).Shoot(firePos.position, targetVec);
                    anim.SetTrigger("doShootLear");

                    timeLeft -= rapidWaitTime;
                    yield return new WaitForSeconds(rapidWaitTime);
                }
                fireTimeLeft = fire2WaitTime;
            }

            timeLeft -= Time.deltaTime;
            fireTimeLeft -= Time.deltaTime;

            yield return new WaitForSeconds(patterns[1].intervalTime - fire1WaitTime);
        }

        anim.SetBool("isShootLear", false);
    }

    #endregion

    #region 레이져패턴

    public IEnumerator co_Laser()
    {
        Vector3 targetVec = Target.transform.position;

        yield return StartCoroutine(co_Move(Vector3.right * xPos + Vector3.up * Target.transform.position.y));
        attacks[2].ShowWarning(LaserPos.position, LaserPos.position.x * (-1) * Vector3.right + LaserPos.position.y * Vector3.up, patterns[3].waitBeforeTime);

        anim.SetBool("isLaser", true);
        yield return new WaitForSeconds(patterns[3].waitBeforeTime);

        Instantiate(attacks[2]).Shoot(LaserPos.position, LaserPos.position.x * (-1) * Vector3.right + LaserPos.position.y * Vector3.up);
        GameMgr.Inst.MainCam.Shake(2.0f, 15, 0.08f, 0f);
        yield return new WaitForSeconds(patterns[3].duration);
        anim.SetBool("isLaser", false);
    }

    #endregion

    #region 단발 발사 패턴

    public IEnumerator co_FireSingle(float duration, float interval)
    {  
        anim.SetBool("isShootLear", true);

        yield return new WaitForSeconds(0.1f);

        float shootWaitTime = 0;

        while (duration > 0)
        {
            Vector3 shootTr = Target.transform.position;

            if(shootWaitTime <= 0)
            {
                shootWaitTime = interval;
                attacks[1].ShowWarning(firePos.position, shootTr, 0.1f);
                yield return new WaitForSeconds(0.1f);
                duration -= 0.1f;

                GameMgr.Inst.MainCam.Shake(0.2f, 10, 0.1f, 0f);
                anim.SetTrigger("doShootLear");
                Instantiate(attacks[1]).Shoot(firePos.position, shootTr);
            }

            shootWaitTime -= Time.deltaTime;
            duration -= Time.deltaTime;
            yield return null;
        }
        anim.SetBool("isShootLear", false);
    }

    #endregion
}
