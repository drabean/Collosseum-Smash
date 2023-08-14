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
        GameMgr.Inst.addScore((int)difficulty);
        //HitEffect ��ȯ - �ӽ� �ڵ�� ����
        //TODO: ������Ʈ Ǯ ����ؾ���
        Vector3 hitVec = (transform.position - attackerPos.position).normalized;

        hit.HitEffect(hitVec);
        hit.DmgTxt(ComboMgr.Inst.checkCombo());

        Transform hitBackParticle = DictionaryPool.Inst.Pop("Prefabs/Particle/HitBackParticle").transform;
        hitBackParticle.SetParent(transform, false);
        hitBackParticle.transform.rotation = (hitVec * (-1)).ToQuaternion();

        hit.FlashWhite(0.1f);
        GameMgr.Inst.Shake(0.15f, 40f, 0.2f);
        GameMgr.Inst.SlowTime(0.15f, 0.2f);

        yield return new WaitForSecondsRealtime(0.15f);

        rb.AddForce(hitVec * 20, ForceMode2D.Impulse);
        rb.gravityScale = 1.0f;
        invokeOnDeath();
        Destroy(GetComponent<Collider2D>());
        yield return new WaitForSeconds(3.0f);

        //TODO: ���� ������Ʈ�� Ǯ ��ȯ �ؾ���
        hitBackParticle.GetComponent<Poolable>().Push();
        Destroy(gameObject);
    }

    [ContextMenu("StartAction")]
    public void StartAction()
    {
        Target = GameObject.FindObjectOfType<Player>().transform;
        StartAI();
    }
}
