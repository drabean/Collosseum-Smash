using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrainingBotHuge : EnemyBoss
{
    Attack[] attacks = new Attack[3];

    private void Awake()
    {
        attacks[0] = Resources.Load<Attack>(patterns[0].prefabName);
        evnt.attack = onAttack;
        float lastSpawnedTime = Time.time + 3; // 첫 소환 시간을 앞당기기 위한 마지막 소환 시간 조절
    }


    public override void StartAI()
    {
        StartCoroutine(co_Atk());
    }


    protected override void selectPattern()
    {
        patternCount--;
        if(patternCount > 0)
        {
            StartCoroutine(co_Atk());
        }
        else
        {
            patternCount = 2;
            StartCoroutine(co_SpawnMob());
        }
    }

    IEnumerator co_Atk()
    {
        attacks[0].ShowWarning(transform.position, transform.position, patterns[0].waitBeforeTime);
        yield return new WaitForSeconds(patterns[0].waitBeforeTime);
        anim.SetTrigger("doAttack");

        yield return new WaitForSeconds(patterns[0].waitAfterTime);
        selectPattern();
    }

    void onAttack()
    {
        GameMgr.Inst.MainCam.Shake(0.4f, 20, 0.15f, 0f);
        Instantiate(attacks[0]).Shoot(transform.position + Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f);
    }


    IEnumerator co_SpawnMob()
    {
        doSpawn();
        yield return new WaitForSeconds(3.0f);
        selectPattern();
    }

    void doSpawn()
    {
        spawnMob(0, transform.position + Vector3.right * 1.5f, deadOption);
        spawnMob(0, transform.position + Vector3.right * -1.5f, deadOption);
    }


}
