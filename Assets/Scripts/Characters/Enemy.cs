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
    public bool isSuperarmor;

    #region delegate
    public Action onDeath;
    protected void invokeOnDeath() { onDeath?.Invoke(); }
    public Action onSpawn;
    #endregion

    protected GameObject curAttackWarning;
    public virtual void StartAI() { }


    public override void onHit(Transform attackerPos, float dmg, float stunTime = 0.5f)
    {
        if (isDead) return;
        curHP -= dmg;


        if (curHP <= 0)
        {
            Target.HitSuccess();
            StartCoroutine(co_Smash(attackerPos, Target.combo.GetCombo()));
        }
        else
        {
            Hit(attackerPos, dmg, stunTime);
        }
    }

    IEnumerator co_Smash(Transform attackerPos, int combo)
    {
        isDead = true;
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
        yield return new WaitForSeconds(1.5f);

        //TODO: 각종 오브젝트들 풀 반환 해야함
        hitBackParticle.GetComponent<Poolable>().Push();
        Destroy(gameObject);
    }

    float KnockBackPower = 2f;

    public void Hit(Transform attackerPos, float dmg, float stunTime = 0.5f)
    {

        if(!isSuperarmor) stopAction();
        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", false);
        if (curAttackWarning != null) DictionaryPool.Inst.Push(curAttackWarning.gameObject);

        Vector3 hitVec = (transform.position - attackerPos.position).normalized;
        hit.FlashWhite(0.1f);
        hit.HitEffect(hitVec);
        if (!isSuperarmor) hit.DmgTxt("stun");
        GameMgr.Inst.Shake(0.15f, 20f, 0.15f);

        //스턴당한 적이 도착할 위치 계산
        if (!isSuperarmor) hit.knockback(1.0f, transform.position + (transform.position - attackerPos.position).normalized * KnockBackPower);
        if (!isSuperarmor) StartCoroutine(co_Stun(stunTime));
    }
    IEnumerator co_Stun(float stunTIme)
    {
        GameObject stunEffect = DictionaryPool.Inst.Pop("Prefabs/Effect/Icon/StunEffect");
        stunEffect.GetComponent<Poolable>().Push(stunTIme);
        stunEffect.transform.parent = transform;
        stunEffect.transform.localPosition = Vector3.up * 0.8f;
        float timeLeft = stunTIme;
        yield return new WaitForSeconds(1.0f);

        StartAI();
    }
    protected virtual void stopAction()
    {
        if (curAttackWarning != null) DictionaryPool.Inst.Push(curAttackWarning.gameObject);
        anim.Play("Idle");
        StopAllCoroutines();
    }


    [ContextMenu("StartAction")]
    public void StartAction()
    {
        Target = GameObject.FindObjectOfType<Player>();
        StartAI();
    }
}
