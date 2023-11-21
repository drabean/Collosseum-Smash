using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : CharacterBase
{

    [Header("적")]
    public Player Target;

    public float difficulty;
    public bool isSuperarmor;
    #region delegate
    public Action<Vector3> onDeath;
    protected void invokeOnDeath(Vector3 pos) { onDeath?.Invoke(pos); }
    public Action onSpawn;
    #endregion

    protected GameObject curAttackWarning;
    public virtual void StartAI() { }


    public override void onHit(Transform attackerPos, float dmg, float stunTime = 0.3f)
    {
        if (isDead) return;
        curHP -= dmg;


        if (curHP <= 0)
        {
            if(Target != null) Target.InvokeOnSmash();
            isDead = true;
            smash(attackerPos);
        }
        else
        {
            Hit(attackerPos, dmg, stunTime);
        }
    }

    protected virtual void smash(Transform attackerPos)
    {
        isDead = true;
        stopAction();
        SoundMgr.Inst.Play("Smash");
        StartCoroutine(co_Smash(attackerPos));
    }

    /// <summary>
    /// 체력이 0이 되었을 때, 사망 연출
    /// </summary>
    /// <param name="attackerPos"></param>
    /// <returns></returns>
    protected virtual IEnumerator co_Smash(Transform attackerPos)
    {
        GameMgr.Inst.addScore((int)difficulty);
        Vector3 hitVec = (transform.position - attackerPos.position).normalized;

        hit.HitEffect(hitVec, size);
        hit.DmgTxt("Smash!");

        Transform hitBackParticle = DictionaryPool.Inst.Pop("Prefabs/Particle/HitBackParticle").transform;
        hitBackParticle.SetParent(transform, false);
        hitBackParticle.transform.rotation = (hitVec * (-1)).ToQuaternion();
        var mainModule = hitBackParticle.GetComponent<ParticleSystem>().main;
        mainModule.startSize = size;

        hit.FlashWhite(0.1f);
        GameMgr.Inst.MainCam.Shake(0.2f, 20f, 0.2f, 0);
        GameMgr.Inst.SlowTime(0.1f, 0.2f);
        Destroy(GetComponent<Collider2D>());

        yield return new WaitForSecondsRealtime(0.15f);

        rb.AddForce(hitVec * 20, ForceMode2D.Impulse);
        rb.gravityScale = 1.0f;
        invokeOnDeath(transform.position);
        yield return new WaitForSeconds(1.5f);

        //TODO: 각종 오브젝트들 풀 반환 해야함
        hitBackParticle.GetComponent<Poolable>().Push();
        Destroy(gameObject);
    }

    float KnockBackPower = 0.3f;

    protected void Hit(Transform attackerPos, float dmg, float stunTime = 0.0f)
    {
        SoundMgr.Inst.Play("Hit");

        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", false);

        Vector3 hitVec = (transform.position - attackerPos.position).normalized;
        hit.FlashWhite(0.2f);
        hit.HitEffect(hitVec, size);
        hit.DmgTxt(((int)dmg).ToString());
        GameMgr.Inst.MainCam.Shake(0.15f, 20f, 0.15f, 0f);
        if (!isSuperarmor)
        {
            hit.knockback(0.3f, transform.position + hitVec * KnockBackPower);
            if (stunTime >= 0.1f) stun(stunTime);
        }
    }

    void stun(float stunTime)
    {
        StopAllCoroutines();
        StartCoroutine(co_Stun(stunTime));
    }
    IEnumerator co_Stun(float stunTime)
    {
        GameObject stunEffect = DictionaryPool.Inst.Pop("Prefabs/Effect/Icon/StunEffect");
        stunEffect.GetComponent<Poolable>().Push(stunTime);
        stunEffect.transform.parent = transform;
        stunEffect.transform.localPosition = Vector3.up * 0.8f;
        yield return new WaitForSeconds(stunTime);

        StartAI();
    }
    protected virtual void stopAction()
    {
        if (curAttackWarning != null) DictionaryPool.Inst.Push(curAttackWarning.gameObject);
        //애니메이션 상태 Idle로 초기화
        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", false);
        anim.Play("Idle");

        StopAllCoroutines();
    }
    protected virtual void setDir()
    {
        setDir((Target.transform.position - transform.position).normalized);
    }


    /// <summary>
    /// 사망 연출 없이, 그냥 삭제
    /// </summary>
    public void Despawn()
    {
        StartCoroutine(co_Despawn());
    }

    IEnumerator co_Despawn()
    {
        isDead = true;

        hit.FlashWhite(0.3f);
        Destroy(GetComponent<Collider2D>());

        yield return new WaitForSecondsRealtime(0.15f);
        Destroy(gameObject);
    }

    [ContextMenu("StartAction")]
    public void StartAction()
    {
        Target = GameObject.FindObjectOfType<Player>();
        StartAI();
    }
}
