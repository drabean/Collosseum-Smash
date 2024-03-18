using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMushroomKing : EnemyBoss
{

    public Transform EyeTr;

    public Attack SporeSmall;
    public Attack SporeBig;
    public Attack MushroomParabola;

    public ParticleSystem PatParticle;

    int patIdx;
    public override void StartAI()
    {
        StartCoroutine(co_Idle(1.5f));
    }

    protected override void selectPattern()
    {
        patIdx += Random.Range(1, 3);
        patIdx %= 3;
        if(isRagePattern)
        {
            isRagePattern = false;
            StartCoroutine(co_PatRage());
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
        }

    }

    protected override void rageChange()
    {
        patterns[0].waitAfterTime -= 0.5f;
        patterns[1].waitAfterTime -= 0.5f;
        patterns[2].waitAfterTime -= 0.5f;
        maxSpawnCount++;
    }
    IEnumerator co_Pat1()
    {
        anim.SetBool("isAttackReady", true);

        SporeSmall.ShowWarning(transform.position, transform.position + Vector3.right * patterns[0].range, patterns[0].waitBeforeTime);
        SporeSmall.ShowWarning(transform.position, transform.position + Vector3.left * patterns[0].range, patterns[0].waitBeforeTime);
        SporeSmall.ShowWarning(transform.position, transform.position + Vector3.up * patterns[0].range, patterns[0].waitBeforeTime);
        SporeSmall.ShowWarning(transform.position, transform.position + Vector3.down * patterns[0].range, patterns[0].waitBeforeTime);
        yield return new WaitForSeconds(patterns[0].waitBeforeTime);
        PatParticle.Play();
        
        anim.SetBool("isAttackReady", false);
        anim.SetTrigger("doAttack");

        Instantiate(SporeSmall).Shoot(transform.position, transform.position + Vector3.right * patterns[0].range);
        Instantiate(SporeSmall).Shoot(transform.position, transform.position + Vector3.left * patterns[0].range);
        Instantiate(SporeSmall).Shoot(transform.position, transform.position + Vector3.up * patterns[0].range);
        Instantiate(SporeSmall).Shoot(transform.position, transform.position + Vector3.down * patterns[0].range);

        GameMgr.Inst.MainCam.Shake(1.0f, 15f, 0.12f, 0f);
        StartCoroutine(co_Idle(patterns[0].waitAfterTime));

    }

    IEnumerator co_Pat2()
    {
        anim.SetBool("isAttackReady", true);

        Vector3[] targetPositions = new Vector3[patterns[1].repeatTIme];

        for(int i = 0; i < patterns[1].repeatTIme; i++)
        {
            if (i != 0) targetPositions[i] = transform.position.Randomize(patterns[1].range);
            else targetPositions[i] = Target.transform.position;
            MushroomParabola.ShowWarning(transform.position, targetPositions[i], patterns[1].waitBeforeTime);
        }
        yield return new WaitForSeconds(patterns[1].waitBeforeTime);
        PatParticle.Play();

        anim.SetBool("isAttackReady", false);
        anim.SetTrigger("doAttack");

        for (int i = 0; i < patterns[1].repeatTIme; i++)
        {
            Attack atk = Instantiate(MushroomParabola);
            atk.Shoot(transform.position, targetPositions[i]);
            if(i == 0)atk.attackAction = () => { spawnMob(0, targetPositions[0], deadOption); };
            if(i == 1)atk.attackAction = () => { spawnMob(0, targetPositions[1], deadOption); };
        }

        GameMgr.Inst.MainCam.Shake(1.0f, 15f, 0.12f, 0f);

        StartCoroutine(co_Idle(patterns[1].waitAfterTime));
    }
    IEnumerator co_Pat3()
    {
        anim.SetBool("isAttackReady", true);

        setDir();
        Vector3[] targetPositions = new Vector3[patterns[2].repeatTIme];

        for(int i = 0; i < patterns[2].repeatTIme; i++)
        {
            targetPositions[i] = (aim.position - transform.position) * patterns[2].range * (i + 1);
            SporeSmall.ShowWarning(transform.position, targetPositions[i], patterns[2].waitBeforeTime);
        }

        yield return new WaitForSeconds(patterns[2].waitBeforeTime);
        PatParticle.Play();

        anim.SetBool("isAttackReady", false);

        anim.SetTrigger("doAttack");
        for (int j = 0; j < patterns[2].repeatTIme; j++)
        {
            Instantiate(SporeSmall).Shoot(transform.position, targetPositions[j]);
        }


        GameMgr.Inst.MainCam.Shake(1.0f, 15f, 0.12f, 0f);
        StartCoroutine(co_Idle(patterns[2].waitAfterTime));
    }
    IEnumerator co_Idle(float time = 1.5f)
    {
        float timeLeft = time;

        while (timeLeft >= 0)
        {
            setDir();

            timeLeft -= Time.deltaTime;
            yield return null;
        }

        selectPattern();
    }

    IEnumerator co_PatRage()
    {
        anim.SetBool("isAttackReady", true);
        SporeBig.ShowWarning(transform.position, transform.position, patterns[3].waitBeforeTime);

        yield return new WaitForSeconds(patterns[3].waitBeforeTime);

        anim.SetTrigger("doAttack");

        Instantiate(SporeBig).Shoot(transform.position, transform.position);

        for (int i = 0; i < patterns[3].repeatTIme; i++)
        {
            anim.SetBool("isAttackReady", true);


            Vector3[] targetPositions = new Vector3[patterns[1].repeatTIme];

            for (int j = 0; j < 5; j++)
            {
                if (j != 0) targetPositions[j] = transform.position.Randomize(patterns[1].range);
                else targetPositions[j] = Target.transform.position;
                MushroomParabola.ShowWarning(transform.position, targetPositions[j], 0.5f);
            }
            yield return new WaitForSeconds(0.5f);


            anim.SetBool("isAttackReady", false);
            anim.SetTrigger("doAttack");

            PatParticle.Play();
            GameMgr.Inst.MainCam.Shake(0.4f, 15f, 0.09f, 0f);
            for (int j = 0; j < 7; j++)
            {
                Instantiate(MushroomParabola).Shoot(transform.position, targetPositions[j]);
            }
            yield return new WaitForSeconds(patterns[3].intervalTime);
        }

        StartCoroutine(co_Idle(patterns[3].waitAfterTime));
    }

    protected override void setDir(Vector3 dir)
    {
        EyeTr.localPosition = Vector3.right * 0.3f * dir.x;
        aim.transform.localPosition = dir * aimRange;
    }

}
