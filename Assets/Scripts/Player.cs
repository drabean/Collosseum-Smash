using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : CharacterBase
{
    #region 오브젝트 참조
    [Header("오브젝트 참조")]
    [SerializeField] Transform atk;
    [SerializeField] Transform spriteGroup;
    [SerializeField] ParticleSystem particle;
    #endregion


    #region 입력 변수
    Vector3 inputVec;
    Vector3 lastVec = Vector3.right;
    bool isSpace;
    #endregion

    #region 상태변수
    bool attackSwitch = false;
    public bool isInAttackRange = false;
    bool isInvincible = false;
    #endregion

    #region 플레이어 
    [Header("최대체력")] public int maxHP;
    [Header("현재체력")] public int curHP;
    #endregion


    private void Awake()
    {
        evnt.moveEffect = () => particle.Play();
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

        a.Attack();

        attackSwitch = !attackSwitch;

        anim.SetTrigger("doAttack");
        anim.SetBool("attackSwitch", attackSwitch);

        atk.position = aim.position;

        float angle = Mathf.Atan2(lastVec.y, lastVec.x) * Mathf.Rad2Deg;
        atk.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        atk.GetComponent<Animator>().SetTrigger("doAttack");
    }

    public void GetInput(Vector2 inputVec)
    {
        this.inputVec = inputVec;
        if (this.inputVec != Vector3.zero) lastVec = inputVec;
        isSpace = Input.GetKeyDown(KeyCode.Space);
    }

    Vector3 minVec = new Vector3(-1, 1, 1);
    protected override void setDir(Vector3 dir)
    {
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

        GameManager.Inst.Shake(0.15f, 50f, 0.12f);
        GameManager.Inst.Zoom(0.15f, 0.98f);
        GameManager.Inst.SlowTime(0.5f, 0.2f);

        UIMgr.Inst.hp.Set(curHP);
        hit.FlashWhite(0.3f);
        hit.Togle(1.0f);

        yield return new WaitForSeconds(1.0f);

        isInvincible = false;
    }
}
