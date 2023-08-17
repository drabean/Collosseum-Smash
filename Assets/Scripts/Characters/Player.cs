using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class STATUS
{
    public int STR;
    public int VIT;
    public int SPD;
}

public class Player : CharacterBase
{
    #region 오브젝트 참조
    [Header("오브젝트 참조")]
    [SerializeField] Transform spriteGroup;
    [SerializeField] ParticleSystem particle;
    #endregion


    #region 입력 변수
    Vector3 inputVec;
    Vector3 lastVec = Vector3.right;
    #endregion

    #region 상태변수
    public bool isInAttackRange = false;
    bool isInvincible = false;
    bool commandLock = false;
    #endregion

    #region 플레이어 
    public STATUS _stat;
    //TODO: 공격범위 조절 추가 (지금은 X)

    int maxHP;
    int curHP;

    /// <summary>
    /// stat을 기반으로 실제 적용시키는 함수
    /// </summary>
    void setStatus()
    {
        moveSpeed = 2f + (_stat.SPD * 0.5f);
        maxHP = _stat.VIT + 1;
        curHP = maxHP;
        UIMgr.Inst.hp.Set(curHP);
    }
    #endregion

    #region Events ( 아이템 등에서 활용)
    public Action<int> onCombo;
    void invokeOnCombo(int combo) { onCombo?.Invoke(combo); }
    public Action onMovement;
    void invokeOnMovement() { onMovement?.Invoke(); }
    public Action onHit;
    void invokeOnHit() { onHit?.Invoke(); }
    #endregion

    [HideInInspector] public Combo combo = new Combo();

    private void Awake()
    {
        evnt.moveEffect = onMove;
        evnt.attack = doAttack;
        evnt.commandLockStart = () => commandLock = true;
        evnt.commandLockEnd = () => commandLock = false;
    }

    private void Start()
    {
        UIMgr.Inst.joystick.setTarget(GetInput);

        setStatus();
    }

    private void Update()
    {
        //
        //임시코드
        attack();

#if UNITY_EDITOR
        this.inputVec = (Vector3.right * Input.GetAxisRaw("Horizontal") + Vector3.up * Input.GetAxisRaw("Vertical")).normalized;
        if(inputVec != null) lastVec = this.inputVec;

#endif
        if (inputVec != Vector3.zero)
        {
            moveToDir(inputVec);
            return;
        }
        else
        {
            anim.SetBool("isMoving", false);
            return;
        }
    }
    void attack()
    {
        if (!isInAttackRange) return;
        if (commandLock) return;


        anim.SetTrigger("doAttack");
    }

    void doAttack()
    {
        AllyMeleeAttack atk = DictionaryPool.Inst.Pop("Prefabs/Effect/AllyMeleeAttack").GetComponent<AllyMeleeAttack>();
        atk.transform.position = aim.position;
        atk.playerTr = transform;
    }

    public void GetInput(Vector2 inputVec)
    {
        this.inputVec = inputVec;
        if (this.inputVec != Vector3.zero) lastVec = inputVec;


    }

    Vector3 minVec = new Vector3(-1, 1, 1);
    protected override void setDir(Vector3 dir)
    {
        if (commandLock) return;
        dir = dir.normalized;
        anim.SetFloat("dirX", dir.x);
        anim.SetFloat("dirY", dir.y);
        if (dir.x != 0) spriteGroup.localScale = dir.x < 0 ? minVec : Vector3.one;
        aim.transform.localPosition = dir * aimRange;
    }

    public override bool Hit(Transform attackerPos)
    {
        if (isInvincible) return false;
        invokeOnHit();
        StartCoroutine(co_Invincible());
        return true;
    }

    IEnumerator co_Invincible()
    {
        isInvincible = true;
        curHP--;

        GameMgr.Inst.Shake(0.15f, 50f, 0.12f);
        GameMgr.Inst.Zoom(0.15f, 0.98f);
        GameMgr.Inst.SlowTime(0.5f, 0.2f);

        UIMgr.Inst.hp.Set(curHP);
        hit.FlashWhite(0.3f);
        hit.Togle(1.0f);

        yield return new WaitForSeconds(1.0f);

        isInvincible = false;
    }

    void onMove()
    {
        particle.Play();
        invokeOnMovement();
    }
}
