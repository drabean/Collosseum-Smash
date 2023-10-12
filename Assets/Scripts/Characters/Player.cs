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
    #region ������Ʈ ����
    [Header("������Ʈ ����")]
    [SerializeField] Transform spriteGroup;
    [SerializeField] ParticleSystem particle;
    public IconHolder iconHolder;
    TargetIcon targetIcon;
    #endregion


    #region �Է� ����
    Vector3 inputVec;
    Vector3 lastVec = Vector3.right;

    bool inputAttack;
    #endregion

    #region ���º���
    public bool isInAttackRange = false;
    bool isInvincible = false;
    bool commandLock = false;
    bool isAutoMove = false;
    #endregion

    #region �÷��̾� 
    /// <summary>
    /// ���� ����
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
    /// ���� �ν��� �� �ִ� �ִ����
    /// </summary>
    public float findRange;        
    /// <summary>
    /// ���� ��Ÿ�
    /// </summary>
    public float attackRange;

    Transform target;
    float targetSize;


    /// <summary>
    /// stat�� ������� ���� �����Ű�� �Լ�
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
        if (isDead) return; // ��� �����϶� �ƹ��͵� ���ϵ���
        if (isAutoMove) return; // ���� �ܰ迡�� �÷��̾��� ���� ���� �̵��Ҷ�.

        findTarget();
        updateTargetIcon();

        //�������۸��
        if (inputVec != Vector3.zero && !commandLock)
        {
            moveToDir(inputVec);
            return;
        }//�ڵ����۸��
        else
        {
            invokeOnMovementStop();
            if(target == null) anim.SetBool("isMoving", false); // ���� ���� Ÿ���� ���ٸ� Idle���·�
            else
            {
                if(Vector2.Distance(transform.position, target.position) > attackRange + targetSize)
                {
                    if(!commandLock) moveTowardTarget(target.position);
                }
                else
                {
                    setDir(target.position - transform.position); //Target�� �ٶ� �� ����
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

        if (hits.Length == 0) //�������� ���� Target�� �������� ����
        {
            target = null;
            return;
        }
        else
        {
            target = hits[0].transform;

            float minLength = Vector3.Distance(transform.position, hits[0].point);
            target = hits[0].transform;
            //���� ������ �ִ� �� ã�ƺ���
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
            //Status ����
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
    /// �������� ������, �����ð��� �ο��ϸ� ������ ���ִ� �ڷ�ƾ
    /// </summary>
    /// <param name="resisted">������ ���� ���� ���׿� �����ߴٸ� false</param>
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
    /// ���� Ÿ�� ���� ���� ��, ȣ��
    /// </summary>
    /// <param name="isMelee"></param>
    public void HitSuccess()
    {
        invokeOnSmash();
    }
}
