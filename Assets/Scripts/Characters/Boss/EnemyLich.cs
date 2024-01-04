using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLich : EnemyBoss
{
    public List<Vector3> tpVecs = new List<Vector3>();
    int curPosIdx = 3;
    public TargetedProjecitile lichSoul;
    public Vector3 groggyVec;

    int patIdx;


    public override void StartAI()
    {
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(co_SpawnRoutine());
        StartCoroutine(co_Idle(0.7f));
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
        patIdx += Random.Range(0, 2);
        patIdx %= 3;


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
        }
    }

    #region Patterns

    float pat1WaitTIme = 0.7f;
    IEnumerator co_Pat1()
    {
        teleport(3);


        Attack atk = Resources.Load<Attack>(patterns[0].prefabName);

        anim.SetBool("isAtk1Ready", true);
        yield return new WaitForSeconds(patterns[0].waitBeforeTime);

        anim.SetBool("isAtk1Ready", false);
        anim.SetTrigger("doAtk1");
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

        
            //공격 경고
            atk.ShowWarning(positions[0], positions[2], pat1WaitTIme);
            atk.ShowWarning(positions[1], positions[3], pat1WaitTIme);
            atk.ShowWarning(positions[2], positions[0], pat1WaitTIme);
            atk.ShowWarning(positions[3], positions[1], pat1WaitTIme);

            yield return new WaitForSeconds(pat1WaitTIme);
            GameMgr.Inst.MainCam.Shake(0.2f, 10, 0.1f, 0f);
            SoundMgr.Inst.Play("Throw");
            Instantiate<Attack>(atk).Shoot(positions[0], positions[2]);
            Instantiate<Attack>(atk).Shoot(positions[2], positions[0]);
            Instantiate<Attack>(atk).Shoot(positions[1], positions[3]);
            Instantiate<Attack>(atk).Shoot(positions[3], positions[1]);

            for(int j = 0; j < 4; j++)
            {
                GameObject LichParticle = DictionaryPool.Inst.Pop("Prefabs/Particle/LichParticle");
                LichParticle.transform.position = positions[j];
                LichParticle.GetComponent<ParticleSystem>().Play();
            }
            yield return new WaitForSeconds(patterns[0].intervalTime);
        }

        yield return new WaitForSeconds(patterns[0].waitAfterTime);
        selectPattern();
    }

    float skulMissileWaitTIme = 0.3f;
    IEnumerator co_Pat2()
    {
        Attack atk = Resources.Load<Attack>(patterns[1].prefabName);

        teleport();

        anim.SetBool("isAtk2Ready", true);
        yield return new WaitForSeconds(patterns[1].waitBeforeTime);
        anim.SetBool("isAtk1Ready", false);

        for (int i = 0; i < patterns[1].repeatTIme; i++)
        {
            anim.SetTrigger("doAtk2");
            sp.flipX = !sp.flipX;
            Vector3 targetVec = Target.transform.position + Random.Range(-patterns[1].range, patterns[1].range) * Vector3.right + Random.Range(-patterns[1].range, patterns[1].range) * Vector3.up;
            atk.ShowWarning(transform.position, targetVec, skulMissileWaitTIme);
            //각 공격들의 발사 / 목표 위치 계산
            yield return new WaitForSeconds(skulMissileWaitTIme);
            GameMgr.Inst.MainCam.Shake(0.2f, 10, 0.1f, 0f);
            SoundMgr.Inst.Play("Throw");
            yield return new WaitForSeconds(0.06f);
            Instantiate<Attack>(atk).Shoot(transform.position, targetVec);

            yield return new WaitForSeconds(patterns[1].intervalTime - 0.06f);
        }

        yield return new WaitForSeconds(patterns[1].waitAfterTime);
        selectPattern();
    }


    public List<Vector3>pat3Vecs = new List<Vector3>();

    float pat3WaitTime = 0.5f;
    IEnumerator co_Pat3()
    {
        Attack atk = Resources.Load<Attack>(patterns[2].prefabName);

        int positionIdx = Random.Range(0, 3);

        teleport(3);
        anim.SetBool("isAtk3Ready", true);
        List<Vector3> startVec = new List<Vector3>();
        List<Vector3> endVec = new List<Vector3>();

        Vector3 dif = (pat3Vecs[positionIdx] - pat3Vecs[(++positionIdx) % 4])/patterns[2].repeatTIme;

        yield return new WaitForSeconds(patterns[2].waitBeforeTime);
        for (int i = 0; i < patterns[2].repeatTIme; i++)
        {
            startVec.Add(pat3Vecs[positionIdx] + dif * i);
            endVec.Add(Target.transform.position);

            atk.ShowWarning(pat3Vecs[positionIdx] + dif * i, Target.transform.position, patterns[2].intervalTime * patterns[2].repeatTIme + patterns[2].waitBeforeTime);

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

    int curEnemyCount = 0;
    IEnumerator co_SpawnRoutine()
    {
        while(true)
        {
            if(curEnemyCount < 2)
            {
                curEnemyCount++;
                EnemyMgr.Inst.SpawnEnemy(mobs[0], EnemyMgr.Inst.getRandomPos(), enemyDeadOption);
            }
            yield return null;
        }
    }

    void enemyDeadOption(Vector3 pos)
    {
        //  this.onHit(transform, 1.0f, 0.0f);
        TargetedProjecitile tp =  Instantiate(lichSoul);
        tp.Shoot(pos, this.gameObject);
        tp.onTouch += onLichSoulHit;
        curEnemyCount--;
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

    void groggy()
    {
        GameMgr.Inst.SlowTime(0.4f, 0.2f, true);

        Debug.Log("G");
        StopAllCoroutines();
        anim.SetBool("isAtk1Ready", false);
        anim.SetBool("isAtk2Ready", false);
        anim.SetBool("isAtk3Ready", false);
        anim.SetBool("isGroggy", true);
        GameMgr.Inst.removeAllNormalEnemies();

        GetComponent<Collider2D>().enabled = true;
        transform.position = groggyVec;
    }
}
