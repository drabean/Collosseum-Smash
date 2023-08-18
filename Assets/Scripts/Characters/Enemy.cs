using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ENEMYTYPE
{
    NORMAL,
    RANGED,
    SPECIAL
}
public class Enemy : CharacterBase
{
    public ENEMYTYPE type;

    [Header("적")]
    public Player Target;

    public float difficulty;

    #region delegate
    public Action onDeath;
    protected void invokeOnDeath() { onDeath?.Invoke(); }
    public Action onSpawn;
    #endregion

    protected GameObject curAttackWarning;
    public virtual void StartAI() { }


    public override bool Hit(Transform attackerPos)
    {
        StopAllCoroutines();
        if (curAttackWarning != null) DictionaryPool.Inst.Push(curAttackWarning.gameObject);

        Target.combo.increaseCombo();

        StartCoroutine(co_Hit(attackerPos, Target.combo.GetCombo()));
        return true;
    }

    IEnumerator co_Hit(Transform attackerPos, int combo)
    {
        GameMgr.Inst.addScore((int)difficulty);
        //HitEffect 소환 - 임시 코드로 땜빵
        //TODO: 오브젝트 풀 사용해야함
        Vector3 hitVec = (transform.position - attackerPos.position).normalized;

        hit.HitEffect(hitVec);
        hit.DmgTxt(combo);

        Transform hitBackParticle = DictionaryPool.Inst.Pop("Prefabs/Particle/HitBackParticle").transform;
        hitBackParticle.SetParent(transform, false);
        hitBackParticle.transform.rotation = (hitVec * (-1)).ToQuaternion();

        hit.FlashWhite(0.1f);
        GameMgr.Inst.Shake(0.2f, 20f, 0.2f);
        GameMgr.Inst.SlowTime(0.1f, 0.2f);

        yield return new WaitForSecondsRealtime(0.15f);

        rb.AddForce(hitVec * 20, ForceMode2D.Impulse);
        rb.gravityScale = 1.0f;
        invokeOnDeath();
        Destroy(GetComponent<Collider2D>());
        yield return new WaitForSeconds(3.0f);

        //TODO: 각종 오브젝트들 풀 반환 해야함
        hitBackParticle.GetComponent<Poolable>().Push();
        Destroy(gameObject);
    }

    float KnockBackPower = 0.5f;

    public override void Stun(Transform attackerPos)
    {
        StopAllCoroutines();
        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", false);
        if (curAttackWarning != null) DictionaryPool.Inst.Push(curAttackWarning.gameObject);
        hit.FlashWhite(0.1f);

        hit.DmgTxt("Stun!");

        //rb.AddForce((transform.position - attackerPos.position).normalized * 5, ForceMode2D.Impulse);
        Vector3 destination = transform.position + (transform.position - attackerPos.position).normalized * KnockBackPower;
        StartCoroutine(co_Stun(1.5f, destination));
    }
    IEnumerator co_Stun(float time, Vector3 destination)
    {
        GameObject stunEffect = DictionaryPool.Inst.Pop("Prefabs/Effect/StunEffect");
        stunEffect.GetComponent<Poolable>().Push(time);
        stunEffect.transform.parent = transform;
        stunEffect.transform.localPosition = Vector3.up * 0.8f;
        float timeLeft = time;
        while (timeLeft >= 0)
        {
            transform.position = Vector3.Lerp(transform.position, destination,  3 * Time.deltaTime);
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        StartAI();
    }

    [ContextMenu("StartAction")]
    public void StartAction()
    {
        Target = GameObject.FindObjectOfType<Player>();
        StartAI();
    }
}
