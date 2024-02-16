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

    SpikeGroup curSpike;
    private void Awake()
    {
        evnt.attack += onStomp;

        patternCountLeft = Random.Range(2, patternCount + 1);

        attacks[0] = Resources.Load<Attack>(patterns[0].prefabName); // 찍기 패턴
        attacks[2] = Resources.Load<Attack>(patterns[2].prefabName); // 발사 패턴 
        attacks[3] = Resources.Load<Attack>(patterns[3].prefabName); // 레이져 패턴

        faceChangeAction += () => { anim.SetTrigger("doFaceChange"); };
     
    }

    public override void StartAI()
    {
        StartCoroutine(co_Move(Vector3.up * 4.5f));
        //여기서 SubBlock들 소환
        StartCoroutine(co_Idle());

        subBlocks[0] = Instantiate(subBlockPrefab);
        subBlocks[0].Spawn();
        subBlocks[0].transform.position =  Vector3.right * -5.8f+ Vector3.up * -0.6f ;  
        subBlocks[0].Init(this, Target);

        subBlocks[1] = Instantiate(subBlockPrefab);
        subBlocks[1].Spawn();
        subBlocks[1].transform.position = Vector3.right * 5.8f+ Vector3.up * -0.6f;
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
        patIdx += Random.Range(1, 4);
        patIdx %= 5;


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
                StartCoroutine(co_Pat4());
                break;            
            case 4:
                StartCoroutine(co_Pat5());
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
        selectPattern();
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

    void onStomp()
    {
        GameMgr.Inst.MainCam.Shake(0.4f, 20, 0.15f, 0f);
        Instantiate(attacks[0]).Shoot(transform.position + Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f);
    }

    #endregion

 

    protected override IEnumerator co_Smash(Transform attackerPos)
    {
        if (curSpike != null) Destroy(curSpike.gameObject);
        subBlocks[0].Destroy();
        subBlocks[1].Destroy();
        return base.co_Smash(attackerPos);
    }



    #region 패턴 2 - 양각 슈팅 패턴
    //양각 공격
    IEnumerator co_Pat2()
    {
        SpikeGroup spike = Instantiate(spikes[2], Vector3.zero, Quaternion.identity);
        curSpike = spike;
        spike.ShowWarning(patterns[1].waitBeforeTime);
        subBlocks[0].MoveToStartpos();
        subBlocks[1].MoveToStartpos();
        yield return new WaitForSeconds(patterns[1].waitBeforeTime);
        spike.Show();
        int blockSwitch = Random.Range(0, 2);

        StartCoroutine(subBlocks[blockSwitch].co_Fire1(patterns[1].duration));
        StartCoroutine(subBlocks[++blockSwitch % 2].co_Fire2(patterns[1].duration));
        yield return new WaitForSeconds(1.0f);

        changeFaceColor(2);

        yield return new WaitForSeconds(patterns[1].duration-1);
        spike.Hide();

        yield return new WaitForSeconds(patterns[1].waitAfterTime);

        Destroy(spike.gameObject);
        selectPattern();
    }
    #endregion

    #region 패턴 3 - 레이져 발사 패턴
    IEnumerator co_Pat3()
    {
        SpikeGroup spike = Instantiate(spikes[3], Vector3.zero, Quaternion.identity);
        curSpike = spike;

        spike.ShowWarning(patterns[2].waitBeforeTime);
        yield return new WaitForSeconds(patterns[2].waitBeforeTime);
        spike.Show();
        changeFaceColor(2);
        //어느 블럭에서 레이저를 발사 할 지 정하는 변수
        int  subBlockIdx = 0;

        for (int i = 0; i < patterns[2].repeatTIme; i++)
        {
            if(i%2==0)
            {
                StartCoroutine(co_Laser(patterns[2].intervalTime - patterns[2].waitBeforeTime));
            }
            else
            {
                StartCoroutine(subBlocks[subBlockIdx].co_Laser()); // 서브블럭 1 발사
                ++subBlockIdx;
                subBlockIdx %= 2;
            }
            
            yield return new WaitForSeconds(patterns[2].intervalTime);
        }
        spike.Hide();
        yield return new WaitForSeconds(patterns[2].waitAfterTime);
        Destroy(spike.gameObject);
        selectPattern();
    }
    IEnumerator co_Laser(float waitAfterTime)
    {
        Vector3 targetVec = Target.transform.position;

        yield return StartCoroutine(co_Move(Vector3.right * Target.transform.position.x + Vector3.up * transform.position.y));
        attacks[2].ShowWarning(transform.position, targetVec, patterns[2].waitBeforeTime);

        anim.SetBool("isLaser", true);
        yield return new WaitForSeconds(patterns[2].waitBeforeTime);

        Instantiate(attacks[2]).Shoot(transform.position, targetVec);
        GameMgr.Inst.MainCam.Shake(2.0f, 15, 0.08f, 0f);
        yield return new WaitForSeconds(waitAfterTime);
        anim.SetBool("isLaser", false);
    }
    #endregion

    #region 패턴 4 - 난사패턴
    public Vector3[] firePositions;
    /// <summary>
    /// 발사계열 1 - 범위 내 무작위 위치 계속 발사
    /// </summary>
    /// <returns></returns>
    public IEnumerator co_Pat4()
    {
        SpikeGroup spike = Instantiate(spikes[Random.Range(0, 2)], Vector3.zero, Quaternion.identity);
        curSpike = spike;

        subBlocks[0].MoveToStartpos();
        subBlocks[1].MoveToStartpos();

        yield return StartCoroutine(co_Move(Vector3.up * 4.5f));
        faceChangeAction.Invoke();
        anim.SetBool("isShootFront", true);
        spike.ShowWarning(patterns[3].waitBeforeTime);

        yield return new WaitForSeconds(patterns[3].waitBeforeTime);
        spike.Show();
        float timeLeft = patterns[3].duration;
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
        Destroy(spike.gameObject);

        selectPattern();
    }
    #endregion

    #region 패턴 5 - 본체 레이저 패턴
    IEnumerator co_Pat5()
    {
        SpikeGroup spike = Instantiate(spikes[4], Vector3.zero, Quaternion.identity);
        curSpike = spike;
        spike.ShowWarning(patterns[4].waitBeforeTime);
        yield return new WaitForSeconds(patterns[4].waitBeforeTime);
        spike.Show();
        changeFaceColor(2);

        for (int i = 0; i < patterns[4].repeatTIme; i++)
        {
            yield return co_Laser(patterns[4].intervalTime);
        }
        spike.Hide();
        yield return new WaitForSeconds(patterns[4].waitAfterTime);
        Destroy(spike.gameObject);
        selectPattern();
    }

    #endregion
}
