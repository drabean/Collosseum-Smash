using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class STATUS
{
    public STATUS(int STR, int VIT, int SPD)
    {
        this.STR = STR;
        this.VIT = VIT;
        this.SPD = SPD;
    }
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
    TargetIcon targetIcon;
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
    bool isAutoMove = false;
    #endregion

    #region 플레이어 
    /// <summary>
    /// 현재 스탯
    /// </summary>
    public STATUS Stat
    {
        get { return _stat; }
        set
        {
            _stat = value;

            SetStatus();
        }
    }
   [SerializeField] STATUS _stat;



    /// <summary>
    /// 적을 인식할 수 있는 최대범위
    /// </summary>
    public float findRange;        
    /// <summary>
    /// 공격 사거리
    /// </summary>
    public float attackRange;

    Transform target;
    float targetSize;


    /// <summary>
    /// stat을 기반으로 실제 적용시키는 함수
    /// </summary>
    public void SetStatus()
    {
        moveSpeed = 2f + (_stat.SPD * 0.5f); 
        maxHP = _stat.VIT + 1;
        if(curHP > maxHP)curHP = maxHP;
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

    public Action onMovementStop;
    void invokeOnMovementStop() { onMovementStop?.Invoke(); }
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
        SetStatus();
        if (targetIcon == null) targetIcon = Instantiate(Resources.Load<TargetIcon>("Prefabs/targetIcon"));
        targetIcon.Owner = transform;
        targetIcon.gameObject.SetActive(false);
        foreach (Equip e in  GameData.Inst.equips)
        {
            Instantiate<Equip>(e).onEquip(this);
        }
    }

    private void Update()
    {
        if (isDead) return; // 사망 상태일때 아무것도 못하도록
        if (isAutoMove) return; // 연출 단계에서 플레이어의 조작 없이 이동할때.

        findTarget();
        updateTargetIcon();

        //수동조작모드
        if (inputVec != Vector3.zero && !commandLock)
        {
            moveToDir(inputVec);
            return;
        }//자동조작모드
        else
        {
            invokeOnMovementStop();
            if(target == null) anim.SetBool("isMoving", false); // 범위 내에 타겟이 없다면 Idle상태로
            else
            {
                if(Vector2.Distance(transform.position, target.position) > attackRange + targetSize)
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
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, findRange, Vector3.forward, 0f,layer);

        if (hits.Length == 0) //추적범위 내에 Target이 존재하지 않음
        {
            target = null;
            return;
        }
        else
        {
            target = hits[0].transform;

            float minLength = Vector3.Distance(transform.position, hits[0].point);
            target = hits[0].transform;
            //가장 가까이 있는 적 찾아보기
            for (int i = 1; i < hits.Length; i++)
            {
                float dist = Vector3.Distance(transform.position, hits[i].point);
                if (dist < minLength)
                {
                    target = hits[i].transform;
                    minLength = dist;
                }
            }
        }

        targetSize = target.GetComponent<CharacterBase>().size;
    }


    void updateTargetIcon()
    {
        if(target == null)
        {
            targetIcon.Target = null;
            targetIcon.curTargetingPosition = transform.position;
            targetIcon.gameObject.SetActive(false);
        }
        else
        {
            targetIcon.gameObject.SetActive(true);
            targetIcon.Target = target;
        }
    }
    void attack()
    {
        if (commandLock) return;

        SoundMgr.Inst.Play("Attack");
        GlobalEvent.Inst.AttackEvent();
        anim.SetTrigger("doAttack");
    }

    void doAttack()
    {
        if ( target.TryGetComponent<CharacterBase>(out CharacterBase cb))
        {
            //Status 적용
            cb.onHit(transform, Stat.STR);
        }


        invokeOnAttack();
    }
    
    public void AutoMove(Vector3 destination)
    {
        StartCoroutine(co_AutoMove(destination));
    }
    IEnumerator co_AutoMove(Vector3 destination)
    {
        isAutoMove = true;
        float originMoveSpd = moveSpeed;
        moveSpeed = 3.0f;
        while(Vector3.Distance(transform.position, destination) >= 0.1f)
        {
            moveTowardTarget(destination);
            yield return null;
        }
        isAutoMove = false;
        moveSpeed = originMoveSpd;
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

        SoundMgr.Inst.Play("PlayerHit");
        if (curHP <= 1)
        {
            die();
        }
        StartCoroutine(co_Invincible(invokeOnHit(false)));
    }
    void die()
    {
        SoundMgr.Inst.StopBGM();
        isDead = true;
        anim.SetBool("isMoving", false);
        UIMgr.Inst.progress.Die();
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

        GameMgr.Inst.MainCam.Shake(0.15f, 50f, 0.12f, 0f);
        GameMgr.Inst.MainCam.Zoom(0.15f, 0.98f);
        GameMgr.Inst.SlowTime(0.3f, 0.3f, true);

        UIMgr.Inst.hp.Set((int)curHP);
        hit.FlashWhite(0.3f);
        hit.Togle(1.0f);

        yield return new WaitForSeconds(1.0f);

        isInvincible = false;
    }

    void onMove()
    {
        particle.Play();
        SoundMgr.Inst.Play("Step");
        invokeOnMovement();
    }

    /// <summary>
    /// 적을 타격 성공 했을 때, 호출
    /// </summary>
    /// <param name="isMelee"></param>
    public void HitSuccess()
    {
        invokeOnSmash();
    }
}
