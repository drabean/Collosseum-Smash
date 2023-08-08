using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    [Header("최대체력")] public int maxHP;
    [Header("현재체력")] public int curHP;
    #endregion


    private void Awake()
    {
        evnt.moveEffect = () => particle.Play();
        evnt.attack = doAttack;
        evnt.commandLockStart = () => commandLock = true;
        evnt.commandLockEnd = () => commandLock = false;
    }

    private void Start()
    {
        UIMgr.Inst.joystick.setTarget(GetInput);
        UIMgr.Inst.hp.Set(curHP);
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
    //테스트코드
    [SerializeField] AttackCol a;
    float attackTimeLeft = 0;
    [SerializeField] float attackCooltime;
    void attack()
    {
        attackTimeLeft -= Time.deltaTime;
        if (!isInAttackRange) return;
        if (attackTimeLeft >= 0) return;

        attackTimeLeft = attackCooltime;

        anim.SetTrigger("doAttack");
    }

    void doAttack()
    {
        DictionaryPool.Inst.Pop("Prefabs/AttackEffect").transform.position = aim.position;
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

    public override void Hit(Transform attackerPos)
    {
        if (isInvincible) return;

        StartCoroutine(co_Invincible());
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
}
