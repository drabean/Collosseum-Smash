using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLich : EnemyBoss
{
    public List<Vector3> tpVecs = new List<Vector3>();
    public Vector3 magicCirclePos;

    int curPosIdx = 3;
    public TargetedProjecitile lichSoul;
    public Vector3 groggyVec;
    public ParticleSystem patternParticle;

    int patIdx;

    public List<Vector3> patternVector = new List<Vector3>();

    public SubShooter subSkul;
    List<SubShooter> subSkuls = new List<SubShooter>();
    List<SubShooter> hardSubSkuls = new List<SubShooter>();
    public override void StartAI()
    {
        GetComponent<Collider2D>().enabled = false;
        spawnPool.Add(mobs[0]);
        StartCoroutine(co_SpawnRoutine());
        StartCoroutine(co_Idle(0.7f));

        spawnSkul();
        if(isHardMode) StartCoroutine(spawnSubSkulHard());
    }

    void spawnSkul()
    {
        SubShooter skul = Instantiate(subSkul);
        skul.Init(transform, (Player)Target);
        skul.StartAutoShoot();
        subSkuls.Add(skul);
    }
    IEnumerator co_Idle(float time = 1.5f)
    {
        float timeLeft = time;

        while (timeLeft >= 0)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        selectPattern();
    }

    protected override void selectPattern()
    {
        patIdx += Random.Range(1, 3);
        patIdx %= 4;


        if(isRagePattern)
        {
            isRagePattern = false;
            StartCoroutine(co_Pat5());
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
            case 3:
                StartCoroutine(co_Pat4());
                break;
        }
    }

    #region Patterns

    float pat1WaitTIme = 0.55f;
    //4방위 뼈창 공격
    IEnumerator co_Pat1()
    {
        teleport(3);


        Attack atk = Resources.Load<Attack>(patterns[0].prefabName);

        anim.SetBool("isAtk1Ready", true);
        yield return new WaitForSeconds(patterns[0].waitBeforeTime);

        for (int i = 0; i < patterns[0].repeatTIme; i++)
        {
            Vector3[] positions = new Vector3[4];
            float angle = Random.Range(0, 45);

            //각 공격들의 발사 / 목표 위치 계산
            for (int j = 0; j < 4; j++)
            {
                // 각도를 라디안으로 변환
                float radians = (angle + j * 90) * Mathf.Deg2Rad;

                // 좌표 계산
                float xOffset = patterns[0].range * Mathf.Cos(radians);
                float yOffset = patterns[0].range * Mathf.Sin(radians);


                positions[j] = Target.transform.position + Vector3.right * xOffset + Vector3.up * yOffset;
            }
            float waitTime = pat1WaitTIme;
            if (i == 0) waitTime += 0.35f;
        
            //공격 경고
            atk.ShowWarning(positions[0], positions[2], waitTime);
            atk.ShowWarning(positions[1], positions[3], waitTime);
            atk.ShowWarning(positions[2], positions[0], waitTime);
            atk.ShowWarning(positions[3], positions[1], waitTime);

            yield return new WaitForSeconds(waitTime);
            GameMgr.Inst.MainCam.Shake(0.15f, 20, 0.15f, 0f);
            anim.SetTrigger("doAtk1");
            Instantiate<Attack>(atk).Shoot(positions[0], positions[2]);
            Instantiate<Attack>(atk).Shoot(positions[2], positions[0]);
            Instantiate<Attack>(atk).Shoot(positions[1], positions[3]);
            Instantiate<Attack>(atk).Shoot(positions[3], positions[1]);
            patternParticle.Play();
            for (int j = 0; j < 4; j++)
            {
                GameObject LichParticle = DictionaryPool.Inst.Pop("Prefabs/Particle/LichParticle");
                LichParticle.transform.position = positions[j];
                LichParticle.GetComponent<ParticleSystem>().Play();
            }
            yield return new WaitForSeconds(patterns[0].intervalTime);
        }

        anim.SetBool("isAtk1Ready", false);

        yield return new WaitForSeconds(patterns[0].waitAfterTime);
        selectPattern();
    }

    //해골미사일 연속투척
    float skulMissileWaitTIme = 0.2f;
    IEnumerator co_Pat2()
    {
        Attack atk = Resources.Load<Attack>(patterns[1].prefabName);

        teleport();

        anim.SetBool("isAtk2Ready", true);
        yield return new WaitForSeconds(patterns[1].waitBeforeTime);
        anim.SetBool("isAtk2Ready", false);

        for (int i = 0; i < patterns[1].repeatTIme; i++)
        {
            anim.SetTrigger("doAtk2");
            sp.flipX = !sp.flipX;
            Vector3 targetVec = Target.transform.position.Randomize(patterns[1].range);
            atk.ShowWarning(transform.position, targetVec, skulMissileWaitTIme);
            //각 공격들의 발사 / 목표 위치 계산
            yield return new WaitForSeconds(skulMissileWaitTIme);
            GameMgr.Inst.MainCam.Shake(0.2f, 10, 0.1f, 0f);
            yield return new WaitForSeconds(0.06f);
            Instantiate<Attack>(atk).Shoot(transform.position, targetVec);

            yield return new WaitForSeconds(patterns[1].intervalTime - 0.06f);

            if (i % 4 == 3)
            {
                teleport();
                yield return new WaitForSeconds(0.7f);
            }
        }

        yield return new WaitForSeconds(patterns[1].waitAfterTime);
        selectPattern();
    }


    //뼈창 직선 투척 패턴
    float pat3WaitTime = 0.5f;
    IEnumerator co_Pat3()
    {
        Attack atk = Resources.Load<Attack>(patterns[2].prefabName);

        bool isLeft = Random.Range(0, 2) == 0; // 패턴 출발점이 왼쪽 / 오른쪽 고름
        Vector3 startPos, endPos;


        if (isLeft)
        {
            startPos = patternVector[0];
            endPos = patternVector[1];
        }
        else
        {
            startPos = patternVector[2];
            endPos = patternVector[3];
        }

        teleport(3);
        anim.SetBool("isAtk3Ready", true);
        List<Vector3> startVec = new List<Vector3>();
        List<Vector3> endVec = new List<Vector3>();

        Vector3 dif = (endPos - startPos)/patterns[2].repeatTIme;

        yield return new WaitForSeconds(patterns[2].waitBeforeTime);
        for (int i = 0; i < patterns[2].repeatTIme; i++)
        {
            startVec.Add(startPos + dif * i);
            endVec.Add(Target.transform.position);

            atk.ShowWarning(startPos + dif * i, Target.transform.position, patterns[2].intervalTime + patterns[2].waitBeforeTime);

            GameObject LichParticle = DictionaryPool.Inst.Pop("Prefabs/Particle/LichParticle");
            LichParticle.transform.position = startVec[i];
            LichParticle.GetComponent<ParticleSystem>().Play();

            yield return new WaitForSeconds(patterns[2].intervalTime);
        }
        anim.SetBool("isAtk3Ready", false);
        anim.SetTrigger("doAtk3");
        yield return new WaitForSeconds(pat3WaitTime);

        for (int i = 0; i < patterns[2].repeatTIme; i++)
        {
            Instantiate(atk).Shoot(startVec[i], endVec[i]);
            GameObject LichParticle = DictionaryPool.Inst.Pop("Prefabs/Particle/LichParticle");
            LichParticle.transform.position = startVec[i];
            LichParticle.GetComponent<ParticleSystem>().Play();
            
            yield return new WaitForSeconds(patterns[2].intervalTime);
        }
        yield return new WaitForSeconds(patterns[2].waitAfterTime);
        selectPattern();
    }

    //가로 똥뿌리기 공격
    IEnumerator co_Pat4()
    {
        teleport(3);
        bool isLeft = Random.Range(0, 2) == 0; // 패턴 출발점이 왼쪽 / 오른쪽 고름
        Vector3 startPos, endPos;

        Attack atk = Resources.Load<Attack>(patterns[3].prefabName);

        if (isLeft)
        {
            startPos = patternVector[0];
            endPos = patternVector[1];
        }
        else
        {
            startPos = patternVector[2];
            endPos = patternVector[3];
        }
        float timeLeft = patterns[3].duration;
        float attackCooltimeLeft = 0;


        anim.SetBool("isAtk2Ready", true);
        GameMgr.Inst.AttackEffectLinear(startPos, endPos, 4f,  patterns[3].waitBeforeTime);
        yield return new WaitForSeconds(patterns[3].waitBeforeTime);

        while(timeLeft >= 0)
        {
            timeLeft -= Time.deltaTime;
            attackCooltimeLeft -= Time.deltaTime;
            if(attackCooltimeLeft < 0)
            {
                attackCooltimeLeft = patterns[3].intervalTime;
                Vector3 atkStartVec = Vector3.right * startPos.x + Vector3.up * Random.Range(startPos.y, endPos.y);
                Vector3 atkEndVec = Vector3.right * startPos.x * (-1) + Vector3.up * atkStartVec.y;

                GameObject LichParticle = DictionaryPool.Inst.Pop("Prefabs/Particle/LichParticle");
                LichParticle.transform.position = atkStartVec;
                LichParticle.GetComponent<ParticleSystem>().Play();


                Instantiate(atk).Shoot(atkStartVec, atkEndVec);

            }
            yield return null;
        }
        anim.SetBool("isAtk2Ready", false);
        yield return new WaitForSeconds(patterns[3].waitAfterTime);

        selectPattern();

    }

    #endregion

    #region 강력한 공격 

    public AnimationCurve rotationSpeedCurve;
    float spinSpeed = 0;
    IEnumerator co_Pat5()
    {
        Attack atk = Resources.Load<Attack>(patterns[4].prefabName);
        StopCoroutine(co_SpawnRoutine());

        GameMgr.Inst.removeAllNormalEnemies();

        float timeLeft = patterns[4].duration;
        float magicCircleFireTime = 0;
        float lichFireTIme = 0;
        float angle = 0;

        teleport(3);
        for (int j = 0; j < 4; j++)
        {
            // 각도를 라디안으로 변환
            float radians = (angle + j * 90) * Mathf.Deg2Rad;

            // 좌표 계산
            float xOffset = patterns[0].range * Mathf.Cos(radians);
            float yOffset = patterns[0].range * Mathf.Sin(radians);

            atk.ShowWarning(magicCirclePos, magicCirclePos + Vector3.right * xOffset + Vector3.up * yOffset, patterns[4].waitBeforeTime);
        }

        yield return new WaitForSeconds(patterns[4].waitBeforeTime);

        while(timeLeft > 0)
        {
            spinSpeed = rotationSpeedCurve.Evaluate(patterns[4].duration - timeLeft) * 40f;
            timeLeft -= Time.deltaTime;
            magicCircleFireTime -= Time.deltaTime;
            lichFireTIme -= Time.deltaTime;

            angle += Time.deltaTime * spinSpeed;

            if(magicCircleFireTime <= 0)
            {
                magicCircleFireTime = patterns[4].intervalTime;

                for (int j = 0; j < 3; j++)
                {
                    // 각도를 라디안으로 변환
                    float radians = (angle + j * 120) * Mathf.Deg2Rad;

                    // 좌표 계산
                    float xOffset = patterns[0].range * Mathf.Cos(radians);
                    float yOffset = patterns[0].range * Mathf.Sin(radians);

                    Instantiate(atk).Shoot(magicCirclePos, magicCirclePos + Vector3.right * xOffset + Vector3.up * yOffset);
                }

            }

            if(lichFireTIme <= 0)
            {
                lichFireTIme = 1.5f;
                StartCoroutine(co_Pat5Subpattern());
            }
            yield return null;
        }
        yield return new WaitForSeconds(patterns[4].waitAfterTime);


        StartCoroutine(co_SpawnRoutine());
        curSpawnCount = 0;

        selectPattern();
    }

    IEnumerator co_Pat5Subpattern()
    {
        Attack atk = Resources.Load<Attack>(patterns[2].prefabName);
        patternParticle.Play();

        anim.SetTrigger("doAtk2");
        sp.flipX = !sp.flipX;
        Vector3 targetVec = Target.transform.position;
        atk.ShowWarning(transform.position, targetVec, skulMissileWaitTIme);
        //각 공격들의 발사 / 목표 위치 계산
        yield return new WaitForSeconds(skulMissileWaitTIme);
        GameMgr.Inst.MainCam.Shake(0.2f, 10, 0.1f, 0f);
        yield return new WaitForSeconds(0.06f);
        Instantiate<Attack>(atk).Shoot(transform.position, targetVec);
    }
    #endregion
    void teleport(int idx = -1)
    {
        if(idx == -1)
        {
            curPosIdx += Random.Range(1, tpVecs.Count);
            curPosIdx %= tpVecs.Count;
        }
        else
        {
            curPosIdx = idx;
        }
        anim.SetTrigger("doTeleport");
        GameObject LichParticle = DictionaryPool.Inst.Pop("Prefabs/Particle/LichParticle");
        LichParticle.transform.position = transform.position;
        LichParticle.GetComponent<ParticleSystem>().Play();

        LichParticle.transform.position = transform.position;
        //tp effect
        transform.position = tpVecs[curPosIdx];
    }

    List<Enemy> spawnPool = new List<Enemy>();
    WaitForSeconds wait05 = new WaitForSeconds(0.5f);

    IEnumerator co_SpawnRoutine()
    {
        while(true)
        {
            spawnMob(spawnPool[Random.Range(0, spawnPool.Count)], EnemyMgr.Inst.getRandomPos(), deadOption);
            yield return wait05;
        }
    }

    protected override void deadOption(Vector3 pos)
    {
        //  this.onHit(transform, 1.0f, 0.0f);
        TargetedProjecitile tp =  Instantiate(lichSoul);
        tp.Shoot(pos, this.gameObject);
        tp.onTouch += onLichSoulHit;
        curSpawnCount--;
    }

    void onLichSoulHit()
    {
        onHit(transform, 1.0f, 0.0f);
    }

    public override void onHit(Transform attackerPos, float dmg, float stunTime = 0.0f)
    {
        if (curHP == 3) groggy();

        base.onHit(attackerPos, dmg, stunTime);
    }
    protected override void rageChange()
    {
        maxSpawnCount = 2;
        StartCoroutine(co_ResetSubSkul());

        spawnPool.Add(mobs[0]);
        spawnPool.Add(mobs[1]); // 원거리형 잡몹을 더 적게 소환하기 위해, 근거리형 잡몹을 2마리 추가로 리스트에 추가.
    }
    IEnumerator co_ResetSubSkul() // SubSkul을 하나 더 생성하고, 발사주기를 재조정하는 함수.
    {
        subSkuls[0].StopAutoShoot();
        subSkuls[0].StartAutoShoot();

        if (!isHardMode)
        {
            yield return new WaitForSeconds(subSkuls[0].shootInterval / 3);

            SubShooter skul = Instantiate(subSkul);
            skul.Init(transform, (Player)Target);
            skul.StartAutoShoot();
            subSkuls.Add(skul);
        }
        else
        {
            hardSubSkuls[0].StopAutoShoot();
            hardSubSkuls[1].StopAutoShoot();

            yield return new WaitForSeconds(subSkuls[0].shootInterval / 5);
            SubShooter skul = Instantiate(subSkul);
            skul.Init(transform, (Player)Target);
            skul.StartAutoShoot();
            subSkuls.Add(skul);

            yield return new WaitForSeconds(subSkuls[0].shootInterval / 5);
            hardSubSkuls[0].StartAutoShoot();         
            
            yield return new WaitForSeconds(subSkuls[0].shootInterval / 5);
            hardSubSkuls[1].StartAutoShoot();

        }
    }

    void groggy()
    {
        patternParticle.Play();
        GameMgr.Inst.SlowTime(0.7f, 0.2f, true);

        StopAllCoroutines();
        anim.SetBool("isAtk1Ready", false);
        anim.SetBool("isAtk2Ready", false);
        anim.SetBool("isAtk3Ready", false);
        anim.SetBool("isGroggy", true);
        GameMgr.Inst.removeAllNormalEnemies();
        foreach(SubShooter subSkul in subSkuls) { Destroy(subSkul.gameObject); }
        if(hardSubSkuls.Count != 0)
        {
            foreach(SubShooter subSkul in hardSubSkuls) { Destroy(subSkul.gameObject); };
        }
        GetComponent<Collider2D>().enabled = true;
        transform.position = groggyVec;
    }

    IEnumerator spawnSubSkulHard()
    {
        for (int i = 0; i < 2; i++)
        {
            SubShooter skul = Instantiate(subSkul, Vector3.right * (i ==1 ? -5 : 5) + Vector3.up * 5.5f, Quaternion.identity);
            skul.Init(transform, (Player)Target);
            Destroy(skul.GetComponent<ModuleFollow>());
            hardSubSkuls.Add(skul);
        }
        yield return new WaitForSeconds(subSkuls[0].shootInterval / 2);
        hardSubSkuls[0].StartAutoShoot();
        hardSubSkuls[1].StartAutoShoot();
    }
}
