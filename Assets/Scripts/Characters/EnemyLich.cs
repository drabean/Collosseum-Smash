using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLich : EnemyBoss
{
    public override void StartAI()
    {
        selectNextPattern();
    }


    void selectNextPattern()
    {
        //int nextPatIdx = Random.Range(0, 2);

        StartCoroutine(co_Pat2());

    }

    float boneSpearWaitTIme = 0.5f;
    IEnumerator co_Pat1()
    {
        Attack boneSpear = Resources.Load<Attack>(patterns[0].prefabName);

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

        
            //공격 경고
            boneSpear.ShowWarning(positions[0], positions[2], boneSpearWaitTIme);
            boneSpear.ShowWarning(positions[1], positions[3], boneSpearWaitTIme);
            boneSpear.ShowWarning(positions[2], positions[0], boneSpearWaitTIme);
            boneSpear.ShowWarning(positions[3], positions[1], boneSpearWaitTIme);

            yield return new WaitForSeconds(boneSpearWaitTIme);
            GameMgr.Inst.MainCam.Shake(0.2f, 10, 0.1f, 0f);
            SoundMgr.Inst.Play("Throw");
            Instantiate<Attack>(boneSpear).Shoot(positions[0], positions[2]);
            Instantiate<Attack>(boneSpear).Shoot(positions[2], positions[0]);
            Instantiate<Attack>(boneSpear).Shoot(positions[1], positions[3]);
            Instantiate<Attack>(boneSpear).Shoot(positions[3], positions[1]);

            for(int j = 0; j < 4; j++)
            {
                //공격 방향으로 피격 파티클
                GameObject LichParticle = DictionaryPool.Inst.Pop("Prefabs/Particle/LichParticle");
                LichParticle.transform.position = positions[j];
                LichParticle.GetComponent<ParticleSystem>().Play();
                LichParticle.GetComponent<Poolable>().Push(0.5f);
            }
            yield return new WaitForSeconds(patterns[0].intervalTime);
        }

        yield return new WaitForSeconds(patterns[0].waitAfterTime);
        selectNextPattern();
    }

    float skulMissileWaitTIme = 0.3f;
    IEnumerator co_Pat2()
    {
        Attack skulMissile = Resources.Load<Attack>(patterns[1].prefabName);

        yield return new WaitForSeconds(patterns[1].waitBeforeTime);
        for (int i = 0; i < patterns[1].repeatTIme; i++)
        {
            Vector3 targetVec = Target.transform.position + Random.Range(-patterns[1].range, patterns[1].range) * Vector3.right + Random.Range(-patterns[1].range, patterns[1].range) * Vector3.up;
            skulMissile.ShowWarning(transform.position, targetVec, skulMissileWaitTIme);
            //각 공격들의 발사 / 목표 위치 계산
            yield return new WaitForSeconds(skulMissileWaitTIme);
            GameMgr.Inst.MainCam.Shake(0.2f, 10, 0.1f, 0f);
            SoundMgr.Inst.Play("Throw");
            Instantiate<Attack>(skulMissile).Shoot(transform.position, targetVec);

            yield return new WaitForSeconds(patterns[1].intervalTime);
        }

        yield return new WaitForSeconds(patterns[1].waitAfterTime);
        selectNextPattern();
    }
}
