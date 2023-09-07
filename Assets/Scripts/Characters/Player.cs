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
    public IconHolder iconHolder;
    #endregion


    #region 입력 변수
    Vector3 inputVec;
    Vector3 lastVec = Vector3.right;

    bool inputAttack;
    #endregion

    #region 상태변수
    public bool isInAttackRange = false;
    bool isInvincible = false;
    bool commandLock = false;
    #endregion

    #region 플레이어 
    public STATUS _stat;
    //TODO: 공격범위 조절 추가 (지금은 X)
    public float findRange;        
    public float attackRange;

    Transform target;
    /// <summary>
    /// stat을 기반으로 실제 적용시키는 함수
    /// </summary>
    void setStatus()
    {
        moveSpeed = 2f + (_stat.SPD * 0.5f);
        maxHP = _stat.VIT + 1;
        curHP = maxHP;
        UIMgr.Inst.hp.Set((int)curHP);
    }
    #endregion

    #region Events 
    public Action actionSmash;
    void invokeOnSmash() { actionSmash?.Invoke(); }
    public Action onMovement;
    void invokeOnMovement() { onMovement?.Invoke(); }

    public Func<bool, bool> actionHit;
    bool invokeOnHit(bool resisted)
    {
        if (actionHit == null)
            return false;
        else
            return actionHit(resisted);
    }


    public Action onAttack;
    void invokeOnAttack() { onAttack?.Invoke(); }

    #endregion

    [HideInInspector] public Combo combo = new Combo();


    public bool testMode;
    private void Awake()
    {
        evnt.moveEffect = onMove;
        evnt.attack = doAttack;
        evnt.commandLockStart = () => commandLock = true;
        evnt.commandLockEnd = () => commandLock = false;
    }

    private void Start()
    {
        if (!testMode)
        {
            UIMgr.Inst.joystick.setTarget(GetInput);
            //UIMgr.Inst.atkBtn.setTarget(attack);
        }
        
        setStatus();
    }

    private void Update()
    {

        if (testMode)
        {
            this.inputVec = (Vector3.right * Input.GetAxisRaw("Horizontal") + Vector3.up * Input.GetAxisRaw("Vertical")).normalized;
            if (inputVec != null) lastVec = this.inputVec;
        }

        //수동조작모드
        if (inputVec != Vector3.zero && !commandLock)
        {
            moveToDir(inputVec);
            return;
        }//자동조작모드
        else
        {
            findTarget();

            if(target == null) anim.SetBool("isMoving", false); // 범위 내에 타겟이 없다면 Idle상태로
            else
            {
                if(Vector2.Distance(transform.position, target.position) > attackRange)
                {
                    if(!commandLock) moveTowardTarget(target.position);
                }
                else
                {
                    setDir(target.position - transform.position); //Target을 바라본 뒤 공격
                    attack();
                }
            }


            return;
        }
    }
    [SerializeField] LayerMask layer;
    void findTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 10.0f, Vector3.forward, 0f,layer);

        if (hits.Length == 0) //추적범위 내에 Target이 존재하지 않음
        {
            target = null;
            return;
        }
        else
        {
            target = hits[0].transform;

            float minLength = Vector3.Distance(transform.position, hits[0].transform.position);
            target = hits[0].transform;
            //가장 멀리있는 Enemy 찾기
            for (int i = 1; i < hits.Length; i++)
            {
                //TODO: 더 빠르고 효율적인 코드 찾아보기
                float dist = Vector3.Distance(transform.position, hits[i].transform.position);
                if (dist < minLength)
                {
                    target = hits[i].transform;
                    minLength = dist;
                }
            }
        }
    }



    void attack()
    {
        if (commandLock) return;

        anim.SetTrigger("doAttack");
    }

    void doAttack()
    {
        //ModuleAttack atk = DictionaryPool.Inst.Pop("Prefabs/Attack/AllyMeleeAttack").GetComponent<ModuleAttack>();
        //atk.transform.position = aim.position;
        //atk.transform.rotation = (aim.position - transform.position).ToQuaternion();
        //atk.ownerTr = transform;
        if ( target.TryGetComponent<CharacterBase>(out CharacterBase cb))
        {
            //Status 적용
            cb.onHit(transform, 1.0f);
        }


        invokeOnAttack();
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

    public override void onHit(Transform attackerPos, float dmg, float stunTime = 0.5f)
    {
        if (isInvincible) return;
        StartCoroutine(co_Invincible(invokeOnHit(false)));
    }

    /// <summary>
    /// 데미지를 입히고, 무적시간을 부여하며 연출을 해주는 코루틴
    /// </summary>
    /// <param name="resisted">아이템 등을 통한 저항에 성공했다면 false</param>
    /// <returns></returns>
    IEnumerator co_Invincible(bool resisted)
    {
        isInvincible = true;
        if(!resisted) curHP--;

        GameMgr.Inst.Shake(0.15f, 50f, 0.12f);
        GameMgr.Inst.Zoom(0.15f, 0.98f);
        GameMgr.Inst.SlowTime(0.5f, 0.2f);

        UIMgr.Inst.hp.Set((int)curHP);
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

    /// <summary>
    /// 적을 타격 성공 했을 때, 호출
    /// </summary>
    /// <param name="isMelee"></param>
    public void HitSuccess()
    {
        int curCombo = combo.increaseCombo();
        invokeOnSmash();
    }
}
