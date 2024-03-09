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
        spawnSpikes();
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

        if (isHardMode) SpawnSubShooter();
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

        if (isRagePattern)
        {
            isRagePattern = false;
            StartCoroutine(co_Pat6());

            return;
        }    
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
        if (isHardMode && subShooter != null)
        {
            if (colorIdx == 0) subShooter.ShootCount(3); // 안전 모드로 돌아갈때 4발
            else subShooter.ShootCount(2); // 패턴 시작할때 2발
        }
    }


    protected override void rageChange()
    {
        foreach(SubBlock block in subBlocks)
        {
            block.fire1WaitTime -= 0.05f;
            block.fire2WaitTime -= 0.1f;
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

    #region 가시

    Animator[,] spikes = new Animator[10, 8];
    public Animator SpikePrefab;

    //전체 가시를 4가지 구역으로 나눈것
    List<Animator>[] area4 = new List<Animator>[4] { new List<Animator>(), new List<Animator>(), new List<Animator>(), new List<Animator>() };

    //외각 가시들
    List<Animator> edges = new List<Animator>();

    //추가 패턴들 - 1: 외각 제외했을때 남은것중 모서리 3블럭씩 총 12블럭 2: 중앙 6블럭
    List<Animator>[] additional = new List<Animator>[2] { new List<Animator>(), new List<Animator>() };
    void spawnSpikes()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Animator spike = Instantiate(SpikePrefab, (-6.75f + 1.5f * x) * Vector3.right + (-6 + 1.5f * y) * Vector3.up, Quaternion.identity);
                spikes[x, y] = spike;

                //모서리 가시들 초기화
                if (x == 0 || x == 9 || y == 0 || y == 7) edges.Add(spike);
                //4구역 가시들 초기화
                if ((x >= 0) && (x <= 4) && (y >= 0) && (y <= 3)) area4[0].Add(spike);
                if ((x >= 0) && (x <= 4) && (y >= 4) && (y <= 7)) area4[1].Add(spike);
                if ((x >= 5) && (x <= 9) && (y >= 0) && (y <= 3)) area4[2].Add(spike);
                if ((x >= 5) && (x <= 9) && (y >= 4) && (y <= 7)) area4[3].Add(spike);
            }
        }
        #region 추가가시 1 - 모서리 블럭들
        additional[0].Add(spikes[1, 1]);
        additional[0].Add(spikes[1, 2]);
        additional[0].Add(spikes[2, 1]);

        additional[0].Add(spikes[8, 1]);
        additional[0].Add(spikes[8, 2]);
        additional[0].Add(spikes[7, 1]);

        additional[0].Add(spikes[1, 6]);
        additional[0].Add(spikes[1, 5]);
        additional[0].Add(spikes[2, 6]);

        additional[0].Add(spikes[8, 6]);
        additional[0].Add(spikes[8, 5]);
        additional[0].Add(spikes[7, 6]);

        #endregion
        #region 추가가시 2 - 중앙 가시

        additional[1].Add(spikes[4, 3]);
        additional[1].Add(spikes[4, 4]);
        additional[1].Add(spikes[5, 3]);
        additional[1].Add(spikes[5, 4]);
        #endregion
    }


    void spikeWarning(List<Animator> spikeLis, float time)
    {
        foreach (Animator spike in spikeLis)
        {
            GameMgr.Inst.AttackEffectLinear(spike.transform.position + Vector3.right * -0.7f, spike.transform.position + Vector3.right * 0.7f, 1.4f, time);
        }
    }
    void showSpikes(List<Animator>  spikeLis)
    {
        foreach (Animator spike in spikeLis)
        {
            spike.SetBool("isOn", true);
        }
    }

    void hideSpikes(List<Animator> spikeLis)
    {
        foreach (Animator spike in spikeLis)
        {
            spike.SetBool("isOn", false);
        }
    }

    void hideAllSpikes()
    {
        foreach(Animator spike in spikes)
        {
            spike.SetBool("isOn", false);
        }
    }
    #endregion



    /// <summary>
    /// 번갈아가면서 찍기 공격
    /// </summary>
    /// <returns></returns>
    #region 패턴 1 - 번갈아가면서 찍기 패턴
    IEnumerator co_Pat1()
    {

        Vector3 originVec = transform.position;
        int spikeIdx = Random.Range(0, 2);

        spikeWarning(edges, patterns[0].waitBeforeTime);
        spikeWarning(additional[spikeIdx], patterns[0].waitBeforeTime);
        yield return new WaitForSeconds(patterns[0].waitBeforeTime);
        changeFaceColor(1);
        setDir();

        showSpikes(edges);
        showSpikes(additional[spikeIdx]);
        yield return new WaitForSeconds(patterns[0].intervalTime);

        yield return StartCoroutine(co_Atk1(Target.transform.position));
        StartCoroutine(co_Move(originVec));

        if(isRage)StartCoroutine(subBlocks[1].co_FireSingle(1, 0.25f));
        yield return StartCoroutine(subBlocks[0].co_Smash());

        if (isRage) StartCoroutine(subBlocks[0].co_FireSingle(1, 0.25f));
        yield return StartCoroutine(subBlocks[1].co_Smash());

        hideAllSpikes();
        changeFaceColor(0);
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
    protected override IEnumerator co_Smash(Transform attackerPos)
    {
        hideAllSpikes();
        subBlocks[0].Destroy();
        subBlocks[1].Destroy();
        if (subShooter != null) Destroy(subShooter.gameObject);
        return base.co_Smash(attackerPos);
    }

    #endregion

    #region 패턴 2 - 양각 슈팅 패턴
    //양각 공격
    IEnumerator co_Pat2()
    {
        int blockSwitch = Random.Range(0, 2);
        StartCoroutine(subBlocks[blockSwitch].co_Fire1(patterns[1].duration));
        StartCoroutine(subBlocks[++blockSwitch % 2].co_Fire2(patterns[1].duration));
        spikeWarning(edges, 1.0f);
        yield return new WaitForSeconds(1.0f);
        changeFaceColor(2);
        showSpikes(edges);

        yield return new WaitForSeconds(patterns[1].duration-1);

        hideAllSpikes();
        changeFaceColor(0);
        yield return new WaitForSeconds(patterns[1].waitAfterTime);

        selectPattern();
    }
    #endregion

    #region 패턴 3 - 레이져 발사 패턴
    IEnumerator co_Pat3()
    {
        //어느 블럭에서 레이저를 발사 할 지 정하는 변수
        int  subBlockIdx = 0;

        int spikeIdx = Random.Range(0, 2);

        spikeWarning(edges, patterns[0].waitBeforeTime);
        spikeWarning(additional[spikeIdx], patterns[0].waitBeforeTime);
        yield return new WaitForSeconds(patterns[2].waitBeforeTime);

        changeFaceColor(2);

        showSpikes(edges);
        showSpikes(additional[spikeIdx]);

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
                if(isRage) StartCoroutine(subBlocks[subBlockIdx].co_FireSingle(0.2f, 0.15f));
            }
            
            yield return new WaitForSeconds(patterns[2].intervalTime);
        }

        changeFaceColor(0);
        hideAllSpikes();
        yield return new WaitForSeconds(patterns[2].waitAfterTime);

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
        yield return StartCoroutine(co_Move(Vector3.up * 4.5f));

        changeFaceColor(2);
        faceChangeAction.Invoke();
        anim.SetBool("isShootFront", true);

        spikeWarning(edges,patterns[3].waitBeforeTime);
        spikeWarning(additional[0], patterns[3].waitBeforeTime);
        yield return new WaitForSeconds(patterns[3].waitBeforeTime);
        showSpikes(edges);
        showSpikes(additional[0]);

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

        changeFaceColor(0);
        hideAllSpikes();
        yield return new WaitForSeconds(patterns[1].waitAfterTime);

        selectPattern();
    }
    #endregion

    #region 패턴 5 - 본체 레이저 패턴
    IEnumerator co_Pat5()
    {
        #region 살릴 구역 정하기
        int safeNum = Random.Range(2, 5);

        List<Animator> spikeLis = new List<Animator>();
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (y == safeNum || y == safeNum + 1) continue;
                else spikeLis.Add(spikes[x, y]);
            }
        }

        #endregion

        spikeWarning(spikeLis, patterns[4].waitBeforeTime);
        yield return new WaitForSeconds(patterns[4].waitBeforeTime);
        changeFaceColor(2);

        showSpikes(spikeLis);
        for (int i = 0; i < patterns[4].repeatTIme; i++)
        {
            if(isRage) StartCoroutine(subBlocks[Random.Range(0, 2)].co_FireSingle(0.15f, 0.2f));
            yield return co_Laser(patterns[4].intervalTime);
        }
        hideAllSpikes();
        yield return new WaitForSeconds(patterns[4].waitAfterTime);
        selectPattern();
    }

    #endregion

    #region 강력한 패턴

    float fireInterval = 1.3f;
    IEnumerator co_Pat6()
    {

        subBlocks[0].MoveToStartpos();
        subBlocks[1].MoveToStartpos();
        yield return StartCoroutine(co_Move(Vector3.up * 4.5f));
        spikeWarning(edges, patterns[5].waitBeforeTime);
        changeFaceColor(2);
        yield return new WaitForSeconds(patterns[5].waitBeforeTime);
        showSpikes(edges);

        StartCoroutine(co_FireSingle(patterns[5].duration, fireInterval));
        yield return new WaitForSeconds(fireInterval / 3);
        StartCoroutine(subBlocks[0].co_FireSingle(patterns[5].duration, fireInterval));
        yield return new WaitForSeconds(fireInterval / 3);
        StartCoroutine(subBlocks[1].co_FireSingle(patterns[5].duration, fireInterval));
        yield return new WaitForSeconds(patterns[5].duration);

        changeFaceColor(0);
        hideAllSpikes();
        yield return new WaitForSeconds(patterns[5].waitAfterTime);
        selectPattern();

    }

    public IEnumerator co_FireSingle(float duration, float interval)
    {
        anim.SetBool("isShootLear", true);

        yield return new WaitForSeconds(0.1f);

        float shootWaitTime = 0;

        while (duration > 0)
        {
            if (shootWaitTime <= 0)
            {
                Vector3 shootTr = Target.transform.position;

                shootWaitTime = interval; 

                attacks[3].ShowWarning(firePos.position, shootTr, 0.1f);
                yield return new WaitForSeconds(0.1f);
                duration -= 0.1f;
                GameMgr.Inst.MainCam.Shake(0.2f, 10, 0.1f, 0f);
                anim.SetTrigger("doShootLear");
                Instantiate(attacks[3]).Shoot(firePos.position, shootTr);
            }

            shootWaitTime -= Time.deltaTime;
            duration -= Time.deltaTime;
            yield return null;

        }
        anim.SetBool("isShootLear", false);
    }

    #endregion
    #region 하드모드패턴
    public SubShooter SubShooterPrefab;
    SubShooter subShooter;

    void SpawnSubShooter()
    {
        subShooter = Instantiate(SubShooterPrefab, new Vector3(0, -6, 0), Quaternion.identity);
        subShooter.Init(transform, Target);
    }
    #endregion
}
