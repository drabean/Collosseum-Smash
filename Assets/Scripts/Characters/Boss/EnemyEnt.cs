using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEnt : EnemyBoss
{
    Attack[] attacks = new Attack[3];

    float rootIntervalLength = 1.7f; // 피할 간격을 없애고 싶을 때, 공격 사이의 거리

    int spawnCount = 5; // 해당수치만큼 패턴 사용 후에 소환패턴 사용
    int spawnCountLeft = 2;
    private void Awake()
    {
        attacks[0] = Resources.Load<Attack>(patterns[0].prefabName);
        attacks[1] = Resources.Load<Attack>(patterns[1].prefabName);
        transform.position = Vector3.up * 5.4f;
        evnt.attack = onAttack;
        evnt.attack2 = onAttack2;

        if (isHardMode)
        {
            patterns[0].waitBeforeTime -= 0.1f;
            patterns[1].waitBeforeTime -= 0.1f;
            patterns[2].waitBeforeTime -= 0.1f;
            patterns[0].waitAfterTime -= 0.2f;
            patterns[1].waitAfterTime -= 0.2f;
            patterns[2].waitAfterTime -= 0.2f;
        }

    }


    public override void StartAI()
    {
        selectPattern();
    }


    protected override void selectPattern()
    {
        patternCount += Random.Range(1,3);
        patternCount %= 3;

        spawnCountLeft--;

        if(isRagePattern)
        {
            isRagePattern = false;
            StartCoroutine(co_PatRage());
            return;
        }
        if(spawnCountLeft == 0)
        {
            StartCoroutine(co_Spawn());
            spawnCountLeft = spawnCount;
            return;
        }
        switch(patternCount)
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

    protected override void rageChange()
    {

        patterns[0].waitAfterTime -= 0.2f;
        patterns[0].repeatTIme += 2;
        patterns[1].waitAfterTime -= 0.2f;
        patterns[2].waitAfterTime -= 0.2f;

        patterns[0].waitBeforeTime -= 0.3f;
        patterns[1].waitBeforeTime -= 0.3f;
        patterns[2].waitBeforeTime -= 0.3f;
    }
   


    #region Pat1 / 한손으로 땅찍기 / 3~5Way 나무뿌리 발산
    public Transform Atk1Pos;
    public List<List<Vector3>> atk1TargetVectors;

    float pat1WaitTime = 1.0f;
    int pat1Count = 9;
    WaitForSeconds waitForAttack = new WaitForSeconds(0.15f);

    IEnumerator co_Pat1() 
    {
        // attacks[0].ShowWarning(transform.position, transform.position, patterns[0].waitBeforeTime);

        atk1TargetVectors = new List<List<Vector3>>();
        float originAngle = 0f;
        float angle;

        for(int i = 0; i < pat1Count; i++)
        {
            atk1TargetVectors.Add(new List<Vector3>());
        }

        for(int i = 0; i < patterns[0].repeatTIme; i++)
        {
            if (i == 0)
            {
                Vector3 direction = Target.transform.position - Atk1Pos.position;
                originAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                // 각도가 음수로 나올 수 있으므로 360으로 나눈 나머지를 취함
                originAngle = (originAngle + 360) % 360;
                angle = originAngle;
            }
            else
            {
                angle = originAngle + Random.Range(20f, 60f) * (i % 2 == 0 ? 1 : -1);
            }

            for (int j = 0; j < pat1Count; j++)
            {
                float radians = angle * Mathf.Deg2Rad;


                Vector3 targetV = Atk1Pos.position + (Vector3.right * Mathf.Cos(radians) + Vector3.up * Mathf.Sin(radians)) * j * rootIntervalLength;



                if (EnemyMgr.Inst.checkInStage(targetV)) atk1TargetVectors[j].Add(targetV);
            }
        }

        foreach(List<Vector3> tV in atk1TargetVectors)
        {
            foreach(Vector3 tV2 in tV)  attacks[0].ShowWarning(tV2, tV2, patterns[0].waitBeforeTime + pat1WaitTime);
        }

        yield return new WaitForSeconds(patterns[0].waitBeforeTime);
        anim.SetTrigger("doAttack1");

        yield return new WaitForSeconds(patterns[0].waitAfterTime);

        transform.localScale = Vector3.right * (-1) * transform.localScale.x + Vector3.up + Vector3.forward; // 다음 공격 할 손 전환
        selectPattern();
    }

    void onAttack()
    {
        StartCoroutine(co_Atk1());
    }

    IEnumerator co_Atk1()
    {
        GameMgr.Inst.MainCam.Shake(0.4f, 20, 0.15f, 0f);

        for (int i = 0; i < atk1TargetVectors.Count; i++)
        {
            foreach (Vector3 tV in atk1TargetVectors[i]) // 보스에 가까운 가시부터 순차적으로 생성
            {
                int atkIdx = Random.Range(0, 4);

                if (atkIdx == 0 || atkIdx == 1) Instantiate(attacks[0]).Shoot(tV, tV);
                if (atkIdx == 2) Instantiate(attacks[1]).Shoot(tV, tV);
                if (atkIdx == 3)
                {
                    Attack atk = Instantiate(attacks[1]);
                    atk.Shoot(tV, tV);
                    atk.transform.localScale = Vector3.left + Vector3.up + Vector3.forward;
                }
            }
            yield return waitForAttack;
        }
    }

    #endregion

    #region Pat2 팔 땅에 찍기 / 자신 주변 범위 전부 가시로 덮기 
    int pat2Count = 8;
    float pat2WaitTime = 1.0f;
    public List<List<Vector3>> atk2TargetVectors;
    IEnumerator co_Pat2()
    {
        atk2TargetVectors = new List<List<Vector3>>();

        float angle = Random.Range(0f, 30f);

        int safeNum = Random.Range(6, 10);
        for (int i = 0; i < pat2Count; i++) atk2TargetVectors.Add(new List<Vector3>());


        for (int i = 0; i < 12; i++)
        {

           angle += 30;

            if (i == safeNum) continue;
            else
            {
                for (int j = 1; j < pat2Count; j++)
                {
                    float radians = angle * Mathf.Deg2Rad;


                    Vector3 targetV = transform.position + (Vector3.right * Mathf.Cos(radians) + Vector3.up * Mathf.Sin(radians)) * j * rootIntervalLength;



                    if (EnemyMgr.Inst.checkInStage(targetV)) atk2TargetVectors[j].Add(targetV);
                }
            }
        }

        foreach (List<Vector3> tV in atk2TargetVectors)
        {
            foreach (Vector3 tV2 in tV) attacks[0].ShowWarning(tV2, tV2, patterns[0].waitBeforeTime + pat2WaitTime);
        }

        yield return new WaitForSeconds(patterns[1].waitBeforeTime);
        anim.SetTrigger("doAttack2");

        yield return new WaitForSeconds(patterns[1].waitAfterTime);

        transform.localScale = Vector3.right * (-1) * transform.localScale.x + Vector3.up + Vector3.forward; // 다음 공격 할 손 전환
        selectPattern();
    }

    void onAttack2()
    {
        StartCoroutine(co_Atk2());
    }

    IEnumerator co_Atk2()
    {
        GameMgr.Inst.MainCam.Shake(0.4f, 20, 0.15f, 0f);

        for (int i = 0; i < atk2TargetVectors.Count; i++)
        {
            foreach (Vector3 tV in atk2TargetVectors[i]) // 보스에 가까운 가시부터 순차적으로 생성
            {
                int atkIdx = Random.Range(0, 4);
                if (atkIdx == 0 || atkIdx == 1) Instantiate(attacks[0]).Shoot(tV, tV);
                if (atkIdx == 2) Instantiate(attacks[1]).Shoot(tV, tV);
                if (atkIdx == 3)
                {
                    Attack atk = Instantiate(attacks[1]);
                    atk.Shoot(tV, tV);
                    atk.transform.localScale = Vector3.left + Vector3.up + Vector3.forward;
                }
            }
            yield return waitForAttack;
        }
    }
    #endregion

    #region Pat3 양팔 땅에 박기 / N번 반복 / 플레이어 위치에 십자가형 가시 순차적 발사
    int pat3Count = 7;
    float pat3WaitTime = 0.6f;
    public List<List<Vector3>> atk3TargetVectors;
    IEnumerator co_Pat3()
    {
        anim.SetBool("isAttack3", true);
        int repeatTime = patterns[2].repeatTIme;
        repeatTime += Random.Range(0, 3);
        for (int i = 0; i <repeatTime; i++)
        {
            float waitTime = patterns[2].waitBeforeTime;
            if (i == 0) waitTime += 0.6f;

            atk3TargetVectors = new List<List<Vector3>>();
            for (int j = 0; j < pat3Count; j++) atk3TargetVectors.Add(new List<Vector3>());

            //Target의 위치를 중심으로 십자가의 형태.
            Vector3 startPos = Target.transform.position;

            atk3TargetVectors[0].Add(startPos);
            for(int j = 1; j < pat3Count; j++)
            {
                Vector3 targetV = startPos + rootIntervalLength * Vector3.right * j;
                if (EnemyMgr.Inst.checkInStage(targetV)) atk3TargetVectors[j].Add(targetV);

                targetV = startPos + rootIntervalLength * Vector3.up * j;
                if (EnemyMgr.Inst.checkInStage(targetV)) atk3TargetVectors[j].Add(targetV);

                targetV = startPos + rootIntervalLength * Vector3.left * j;
                if (EnemyMgr.Inst.checkInStage(targetV)) atk3TargetVectors[j].Add(targetV);

                targetV = startPos + rootIntervalLength * Vector3.down * j;
                if (EnemyMgr.Inst.checkInStage(targetV)) atk3TargetVectors[j].Add(targetV);
            }



            foreach (List<Vector3> tV in atk3TargetVectors)
            {
                foreach (Vector3 tV2 in tV) attacks[0].ShowWarning(tV2, tV2, patterns[0].waitBeforeTime + pat3WaitTime);
            }

            yield return new WaitForSeconds(waitTime);
            StartCoroutine(co_Atk3());

            yield return new WaitForSeconds(patterns[2].intervalTime);
        }

        anim.SetBool("isAttack3", false);
        yield return new WaitForSeconds(patterns[2].waitAfterTime);
        selectPattern();
    }

    IEnumerator co_Atk3()
    {
        GameMgr.Inst.MainCam.Shake(0.4f, 20, 0.15f, 0f);

        for (int i = 0; i < atk3TargetVectors.Count; i++)
        {
            foreach (Vector3 tV in atk3TargetVectors[i]) // 보스에 가까운 가시부터 순차적으로 생성
            {
                int atkIdx = Random.Range(0, 4);
                if(atkIdx == 0 || atkIdx == 1) Instantiate(attacks[0]).Shoot(tV, tV);
                if(atkIdx==2) Instantiate(attacks[1]).Shoot(tV, tV);
                if (atkIdx == 3)
                {
                    Attack atk = Instantiate(attacks[1]);
                    atk.Shoot(tV, tV);
                    atk.transform.localScale = Vector3.left + Vector3.up + Vector3.forward;
                }
            }
            yield return waitForAttack;
        }
    }
    #endregion

    #region 소환패턴

    IEnumerator co_Spawn()
    {
        anim.SetBool("isAttack3", true);

        yield return new WaitForSeconds(patterns[3].waitBeforeTime);

        GameMgr.Inst.MainCam.Shake(0.4f, 20, 0.15f, 0f);
        BUF buf;
        if (!isHardMode) buf = BUF.NONE;
        else buf = BUF.MEDAL;

        spawnMob(0, Vector3.right * -2.0f, deadOption, buf);
        spawnMob(0, Vector3.right * 2.0f, deadOption, buf);
        anim.SetBool("isAttack3", false);
        yield return new WaitForSeconds(patterns[3].waitAfterTime);
        selectPattern();
    }
    #endregion

    #region 발악패턴
    int ragePatCount = 7;
    IEnumerator co_PatRage()
    {
        anim.SetBool("isAttack3", true);

        for (int i = 0; i < patterns[4].repeatTIme; i++)
        {
            float waitTime = patterns[2].waitBeforeTime;
            if (i == 0) waitTime += 0.6f;

            int safeNum = Random.Range(-2, 1);

            List<List<Vector3>> targetVectors = new List<List<Vector3>>();
            for (int j = 0; j < ragePatCount; j++) targetVectors.Add(new List<Vector3>());

            for(int j = 0; j < ragePatCount; j++)
            {
                for(int k = -7; k < 8; k+=2)
                {
                    if(k != safeNum * 2+1)targetVectors[j].Add(Vector3.right * k + Vector3.up * (-6.5f + 2 * j));
                }
            }


            foreach (List<Vector3> tV in targetVectors)
            {
                foreach (Vector3 tV2 in tV) attacks[0].ShowWarning(tV2, tV2, patterns[0].waitBeforeTime + pat3WaitTime);
            }

            yield return new WaitForSeconds(waitTime);
            StartCoroutine(co_AtkRage(targetVectors));

            yield return new WaitForSeconds(patterns[4].intervalTime);
        }

        anim.SetBool("isAttack3", false);
        yield return new WaitForSeconds(patterns[4].waitAfterTime);
        selectPattern();
    }
    IEnumerator co_AtkRage(List<List<Vector3>> TargetVectors)
    {
        GameMgr.Inst.MainCam.Shake(0.4f, 20, 0.15f, 0f);

        for (int i = 0; i < TargetVectors.Count; i++)
        {
            foreach (Vector3 tV in TargetVectors[i]) // 보스에 가까운 가시부터 순차적으로 생성
            {
                int atkIdx = Random.Range(0, 4);
                if (atkIdx == 0 || atkIdx == 1) Instantiate(attacks[0]).Shoot(tV, tV);
                if (atkIdx == 2) Instantiate(attacks[1]).Shoot(tV, tV);
                if (atkIdx == 3)
                {
                    Attack atk = Instantiate(attacks[1]);
                    atk.Shoot(tV, tV);
                    atk.transform.localScale = Vector3.left + Vector3.up + Vector3.forward;
                }
            }
            yield return waitForAttack;
        }
    }
    #endregion
}
