using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Title화면에서 쓰기 위한, 행동 없이 피격모션만 보여주는 물체
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

}
