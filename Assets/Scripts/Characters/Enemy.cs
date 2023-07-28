using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : CharacterBase
{
    [Header("��")]
    public Transform Target;

    public float difficulty;

    #region delegate
    public Action onDeath;
    protected void invokeOnDeath() { onDeath?.Invoke(); }
    public Action onSpawn;
    #endregion
    public virtual void StartAI() { }


    public override void Hit(Transform attackerPos)
    {
        StopAllCoroutines();
        ModuleAttack attack = GetComponentInChildren<ModuleAttack>();
        Destroy(attack);
        StartCoroutine(co_Hit(attackerPos));
    }

    IEnumerator co_Hit(Transform attackerPos)
    {
        GameManager.Inst.addScore((int)difficulty);
        //HitEffect ��ȯ - �ӽ� �ڵ�� ����
        //TODO: ������Ʈ Ǯ ����ؾ���
        Vector3 hitVec = (transform.position - attackerPos.position).normalized;

        Instantiate(Resources.Load("Prefabs/HitEffect"), transform.position + hitVec * -0.4f, Quaternion.identity);
        Transform hitBackParticle = Instantiate<Transform>(Resources.Load<Transform>("Prefabs/HitBackParticle"));
        hitBackParticle.SetParent(transform, false);

        hit.FlashWhite(0.1f);
        GameManager.Inst.Shake(0.15f, 20f, 0.12f);
        GameManager.Inst.Zoom(0.15f, 0.99f);
        GameManager.Inst.SlowTime(0.2f, 0.6f);

        yield return new WaitForSeconds(0.15f);

        rb.AddForce(hitVec * 20, ForceMode2D.Impulse);
        rb.gravityScale = 1.0f;
        invokeOnDeath();
        Destroy(GetComponent<Collider2D>());
        yield return new WaitForSeconds(3.0f);

        //TODO: ���� ������Ʈ�� Ǯ ��ȯ �ؾ���
        Destroy(gameObject);
    }

    [ContextMenu("SpawnTest")]
    public void testFunc()
    {
        StartAI();
    }
}
