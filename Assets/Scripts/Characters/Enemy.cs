using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : CharacterBase
{
    [Header("적")]
    public Transform Target;

    public virtual void StartAI() { }

    public override void Hit(Transform attackerPos)
    {
        StartCoroutine(co_Hit(attackerPos));
    }

    IEnumerator co_Hit(Transform attackerPos)
    {
        //HitEffect 소환 - 임시 코드로 땜빵
        //TODO: 오브젝트 풀 사용해야함
        Vector3 hitVec = (transform.position - attackerPos.position).normalized;

        Instantiate(Resources.Load("Prefabs/HitEffect"), transform.position + hitVec * -0.4f, Quaternion.identity);
        Transform hitBackParticle = Instantiate<Transform>(Resources.Load<Transform>("Prefabs/HitBackParticle"));
        hitBackParticle.SetParent(transform, false);

        //Material 변경
        Material origin = sp.material;
        sp.material = Resources.Load<Material>("Materials/WhiteFlash");
        GameManager.Inst.Shake(0.15f, 20f, 0.08f);
        GameManager.Inst.Zoom(0.15f, 0.99f);
        GameManager.Inst.SlowTime(0.2f, 0.6f);

        yield return new WaitForSeconds(0.15f);

        sp.material = origin;

        rb.AddForce(hitVec * 20, ForceMode2D.Impulse);
        rb.gravityScale = 1.0f;

        yield return new WaitForSeconds(3.0f);

        //TODO: 각종 오브젝트들 풀 반환 해야함
        Destroy(gameObject);
    }
}
