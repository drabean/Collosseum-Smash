using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKingBlock : EnemyBoss
{
    Attack[] attacks = new Attack[6];

    public SubBlock subBlockPrefab;
    public SubBlock[] subBlocks = new SubBlock[2];

    public List<SpriteRenderer> faceSprites = new List<SpriteRenderer>();
    public List<Color> faceColors = new List<Color>();

    public System.Action faceChangeAction;
    int patIdx;

    public Transform firePos;

    public List<SpikeGroup> spikes;

    private void Awake()
    {
        evnt.attack += onAttack1;

        patternCountLeft = Random.Range(2, patternCount + 1);

        attacks[0] = Resources.Load<Attack>(patterns[0].prefabName);
        attacks[2] = Resources.Load<Attack>(patterns[2].prefabName);
        attacks[3] = Resources.Load<Attack>(patterns[3].prefabName);

        faceChangeAction += () => { anim.SetTrigger("doFaceChange"); };
     
    }

    public override void StartAI()
    {
        StartCoroutine(co_Move(Vector3.up * 4.5f));
        //여기서 SubBlock들 소환
        StartCoroutine(co_Idle());

        subBlocks[0] = Instantiate(subBlockPrefab);
        subBlocks[0].Spawn();
        subBlocks[0].transform.position = Vector3.right * -5.8f;  
        subBlocks[0].Init(this, Target);

        subBlocks[1] = Instantiate(subBlockPrefab);
        subBlocks[1].Spawn();
        subBlocks[1].transform.position = Vector3.right * 5.8f;
        subBlocks[1].Init(this, Target);
        subBlocks[1].transform.localScale = Vector3.right * (-1) + Vector3.up + Vector3.forward;
    }

    IEnumerator co_Idle(float time = 1.5f)
    {
        changeFaceColor(0);
        float timeLeft = time;

        while (timeLeft >= 0)
        {
            setDir();

            timeLeft -= Time.deltaTime;
            yield return null;
        }

        selectPattern();
    }

    protected override void selectPattern()
    {
        patIdx += Random.Range(1, 3);
        patIdx %= 3;


        //test
        patIdx = 3;
        switch (patIdx)
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
            case 3:
                StartCoroutine(co_Fire1(6f));
                break;
        }
    }
    protected override void setDir(Vector3 dir)
    {
        dir = dir.normalized;
        aim.transform.localPosition = dir * aimRange;
    }

    void changeFaceColor(int colorIdx)
    {
        faceChangeAction?.Invoke();
        foreach (SpriteRenderer sp in faceSprites)
        {
            sp.color = faceColors[colorIdx];
        }
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

    /// <summary>
    /// 번갈아가면서 찍기 공격
    /// </summary>
    /// <returns></returns>
    #region Pat1
    IEnumerator co_Pat1()
    {
        changeFaceColor(1);

        Vector3 originVec = transform.position;

        setDir();
        yield return StartCoroutine(co_Atk1(Target.transform.position));
        yield return new WaitForSeconds(patterns[0].intervalTime);
        StartCoroutine(co_Move(originVec));

        yield return StartCoroutine(subBlocks[0].co_Smash());

        yield return StartCoroutine(subBlocks[1].co_Smash());

        yield return new WaitForSeconds(patterns[0].waitAfterTime);
        StartCoroutine(co_Idle());
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

    void onAttack1()
    {
        GameMgr.Inst.MainCam.Shake(0.4f, 20, 0.15f, 0f);
        Instantiate(attacks[0]).Shoot(transform.position + Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f);
    }

    #endregion

    #region pat2
    //양각 공격
    IEnumerator co_Pat2()
    {
        changeFaceColor(2);
        yield return new WaitForSeconds(patterns[1].waitBeforeTime);
        int blockSwitch = Random.Range(0, 2);

        StartCoroutine(subBlocks[blockSwitch].co_Fire1(patterns[1].duration));
        StartCoroutine(subBlocks[++blockSwitch % 2].co_Fire2(patterns[1].duration));

        yield return new WaitForSeconds(patterns[1].duration);
        yield return new WaitForSeconds(patterns[1].waitAfterTime);

        StartCoroutine(co_Idle());
    }
    #endregion

    #region pat3
    /// <summary>
    /// 번갈아가면서 레이져 쏘기 공격
    /// </summary>
    /// <returns></returns>
    IEnumerator co_Pat3()
    {
        changeFaceColor(2);
        //어느 블럭에서 레이저를 발사 할 지 정하는 변수
        int blockSwitch = Random.Range(0, 3);

        for(int i = 0; i < patterns[2].repeatTIme; i++)
        {
            switch(blockSwitch)
            {
                case 0:
                    StartCoroutine(co_Laser()); // 본체블럭 발사
                    break;
                case 1:
                    StartCoroutine(subBlocks[0].co_Laser()); // 서브블럭 1 발사
                    break;     
                case 2:
                    StartCoroutine(subBlocks[1].co_Laser()); // 서브블럭 2 발사
                    break;
            }

            blockSwitch += Random.Range(1, 3); // 다음 발사할 블록을 랜덤으로 지정
            blockSwitch %= 3;

            yield return new WaitForSeconds(patterns[2].intervalTime);
        }

        yield return new WaitForSeconds(patterns[2].waitAfterTime);

        StartCoroutine(co_Idle());
    }
    IEnumerator co_Laser()
    {
        Vector3 targetVec = Target.transform.position;

        yield return StartCoroutine(co_Move(Vector3.right * Target.transform.position.x + Vector3.up * transform.position.y));
        attacks[2].ShowWarning(transform.position, targetVec, patterns[2].waitBeforeTime);

        anim.SetBool("isLaser", true);
        yield return new WaitForSeconds(patterns[2].waitBeforeTime);

        Instantiate(attacks[2]).Shoot(transform.position,targetVec);
        GameMgr.Inst.MainCam.Shake(2.0f, 15, 0.08f, 0f);
        yield return new WaitForSeconds(patterns[2].duration);
        anim.SetBool("isLaser", false);
    }

    protected override IEnumerator co_Smash(Transform attackerPos)
    {
        subBlocks[0].Destroy();
        subBlocks[1].Destroy();
        return base.co_Smash(attackerPos);
    }
    #endregion

    public Vector3[] firePositions;
    /// <summary>
    /// 발사계열 1 - 범위 내 무작위 위치 계속 발사
    /// </summary>
    /// <returns></returns>
    public IEnumerator co_Fire1(float duration)
    {
        SpikeGroup spike = Instantiate(spikes[0], Vector3.zero, Quaternion.identity);
        yield return StartCoroutine(co_Move(Vector3.up * 4.5f));
        anim.SetBool("isShootFront", true);
        spike.ShowWarning(patterns[3].waitBeforeTime);

        yield return new WaitForSeconds(patterns[3].waitBeforeTime);

        spike.Show();
        float timeLeft = duration;
        float fireTimeLeft = patterns[3].intervalTime;
        while (timeLeft >= 0)
        {


            if (fireTimeLeft < 0)
            { 
                fireTimeLeft = patterns[3].intervalTime;
                GameMgr.Inst.MainCam.Shake(0.1f, 10, 0.05f, 0f);
                Vector3 targetVec = Vector3.right * Random.Range(firePositions[0].x, firePositions[1].x) + Vector3.up * firePositions[0].y;
                Instantiate(attacks[3]).Shoot(firePos.position, targetVec);
                anim.SetTrigger("doShootFront");
            }

            timeLeft -= Time.deltaTime;
            fireTimeLeft -= Time.deltaTime;

            yield return null;
        }
        anim.SetBool("isShootFront", false);
        spike.Hide();
        yield return new WaitForSeconds(patterns[1].waitAfterTime);
    }
}
