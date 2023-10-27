using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LichBoneSpear : Attack
{

    [SerializeField] Attack BoneSpear;

    public override void Shoot(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] positions = new Vector3[4];
        //각 공격들의 발사 / 목표 위치 계산
        for (int j = 0; j < 4; j++)
        {
            // 각도를 라디안으로 변환
            float radians = (45 + j * 90) * Mathf.Deg2Rad;

            // 좌표 계산
            float xOffset = 3 * Mathf.Cos(radians);
            float yOffset = 3 * Mathf.Sin(radians);

            positions[j] = Vector3.right * xOffset + Vector3.up * yOffset;
        }

        for (int j = 0; j < 4; j++)
        {
            GameObject LichParticle = DictionaryPool.Inst.Pop("Prefabs/Particle/LichParticle");
            LichParticle.transform.position = targetPos + positions[j];
            LichParticle.GetComponent<ParticleSystem>().Play();
        }
        Instantiate<Attack>(BoneSpear).Shoot(targetPos + positions[0], targetPos + positions[2]);
        Instantiate<Attack>(BoneSpear).Shoot(targetPos + positions[2], targetPos + positions[0]);
        Instantiate<Attack>(BoneSpear).Shoot(targetPos + positions[1], targetPos + positions[3]);
        Instantiate<Attack>(BoneSpear).Shoot(targetPos + positions[3], targetPos + positions[1]);

        Destroy(gameObject);

    }
    public override GameObject ShowWarning(Vector3 startPos, Vector3 targetPos, float time)
    {
        Vector3[] positions = new Vector3[4];
        //각 공격들의 발사 / 목표 위치 계산
        for (int j = 0; j < 4; j++)
        {
            // 각도를 라디안으로 변환
            float radians = (45 + j * 90) * Mathf.Deg2Rad;

            // 좌표 계산
            float xOffset = 3 * Mathf.Cos(radians);
            float yOffset = 3 * Mathf.Sin(radians);

            positions[j] = Vector3.right * xOffset + Vector3.up * yOffset;
        }

        GameMgr.Inst.AttackEffectLinear(targetPos + positions[0], targetPos + positions[2], 0.3f , time);
        GameMgr.Inst.AttackEffectLinear(targetPos + positions[1], targetPos + positions[3], 0.3f , time);

        return null;
    }
}
