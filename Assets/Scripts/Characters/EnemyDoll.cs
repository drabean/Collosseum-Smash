using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 피격 관련 액션만 필요하고 행동은 필요 없는 객체에 대한 스크립트.
/// </summary>
public class EnemyDoll : Enemy
{
    protected override IEnumerator co_Smash(Transform attackerPos)
    {
        Vector3 hitVec = (transform.position - attackerPos.position).normalized;

        hit.HitEffect(hitVec, size);

        Transform hitBackParticle = DictionaryPool.Inst.Pop("Prefabs/Particle/HitBackParticle").transform;
        hitBackParticle.SetParent(transform, false);
        hitBackParticle.transform.rotation = (hitVec * (-1)).ToQuaternion();
        var mainModule = hitBackParticle.GetComponent<ParticleSystem>().main;
        mainModule.startSize = size;

        hit.FlashWhite(0.1f);
        Destroy(GetComponent<Collider2D>());

        rb.AddForce(hitVec * 20, ForceMode2D.Impulse);
        rb.gravityScale = 1.0f;
        invokeOnDeath(Vector3.zero);
        yield return new WaitForSeconds(1.5f); 

        //TODO: 각종 오브젝트들 풀 반환 해야함
        hitBackParticle.GetComponent<Poolable>().Push();
        Destroy(gameObject);
    }

    protected override void Hit(Transform attackerPos, float dmg, float stunTime = 0)
    {
        Vector3 hitVec = (transform.position - attackerPos.position).normalized;
        hit.FlashWhite(0.2f);
        hit.HitEffect(hitVec, size);
        return;
    }

}
