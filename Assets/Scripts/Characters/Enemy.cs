using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : CharacterBase
{
    [Header("��")]
    public Transform Target;

    public virtual void StartAI() { }

    public override void Hit(Transform attackerPos)
    {
        StartCoroutine(co_Hit(attackerPos));
    }

    IEnumerator co_Hit(Transform attackerPos)
    {
        //HitEffect ��ȯ - �ӽ� �ڵ�� ����
        //TODO: ������Ʈ Ǯ ����ؾ���
        Vector3 hitVec = (transform.position - attackerPos.position).normalized;

        Instantiate(Resources.Load("Prefabs/HitEffect"), transform.position + hitVec * -0.4f, Quaternion.identity);
        Transform hitBackParticle = Instantiate<Transform>(Resources.Load<Transform>("Prefabs/HitBackParticle"));
        hitBackParticle.SetParent(transform, false);

        //Material ����
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

        //TODO: ���� ������Ʈ�� Ǯ ��ȯ �ؾ���
        Destroy(gameObject);
    }
}
