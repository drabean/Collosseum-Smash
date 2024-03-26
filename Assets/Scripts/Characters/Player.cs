using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class STATUS
{
    public STATUS(int STR, int VIT, int SPD, int ACC)
    {
        this.STR = STR;
        this.VIT = VIT;
        this.SPD = SPD;
        this.ACC = ACC;
    }
    public int STR;
    public int VIT;
    public int SPD;
    public int ACC;
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

   public STATUS Stat;



    /// <summary>
    /// ���� �ν��� �� �ִ� �ִ����
    /// </summary>
    public float findRange;        
    /// <summary>
    /// ���� ��Ÿ�
    /// </summary>
    public float attackRange;


    /// <summary>
    /// ��ô ��Ÿ�
    /// </summary>
    public float throwRange;

    Transform target;
    float targetSize;

    #region ���º���
    public bool isHolding;
    #endregion

    /// <summary>
    /// stat�� ������� ���� �����Ű�� �Լ�
    /// </summary>
    public void SetStatus()
    {
        moveSpeed = 2.2f + (Stat.SPD * 0.4f);
        anim.SetFloat("moveSpeed", moveSpeed / 3);
        maxHP = Stat.VIT + 1;
        if(curHP > maxHP)curHP = maxHP;
        UIMgr.Inst.hp.SetMaxHP((int)maxHP);
    }

    #endregion

    #region Events 
    /// <summary>
    /// �� óġ��
    /// </summary>
    public Action actionSmash;
    public void InvokeOnSmash() { actionSmash?.Invoke(); }

    /// <summary>
    /// �̵� �� ��
    /// </summary>
    public Action onMovement;
    public void InvokeOnMovement() { onMovement?.Invoke(); }


    /// <summary>
    /// �ǰ� ��
    /// </summary>
    public ResistanceChain resistenceChain = new ResistanceChain();

    /// <summary>
    /// ���� ��
    /// </summary>
    public Action onAttack;
    public void InvokeOnAttack() { onAttack?.Invoke(); }

    /// <summary>
    /// ��ô ��
    /// </summary>
    public Action<Projectile> onThrow;
    public void InvokeOnThrow(Projectile projectile) { onThrow?.Invoke(projectile); }
    /// <summary>
    /// �̵� ���� ��
    /// </summary>
    public Action onMovementStop;
    public void InvokeOnMovementStop() { onMovementStop?.Invoke(); }
    #endregion

    [HideInInspector] public Combo combo = new Combo();


    private void Awake()
    {
        evnt.moveEffect = onMove;
        evnt.attack = doAttack;
        evnt.attack2 = throwItem;
        evnt.commandLockStart = () => commandLock = true;
        evnt.commandLockEnd = () => commandLock = false;
    }

    private void Start()
    {
        targetIcon = GameObject.FindObjectOfType<TargetIcon>();
        if (targetIcon == null) targetIcon = Instantiate(Resources.Load<TargetIcon>("Prefabs/targetIcon"));
        targetIcon.Owner = transform;
        targetIcon.gameObject.SetActive(false);
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
            InvokeOnMovementStop();
            if(target == null) anim.SetBool("isMoving", false); // ���� ���� Ÿ���� ���ٸ� Idle���·�
            else
            {
                if (commandLock) return;

                if (!isHolding)
                {
                    if (Vector2.Distance(transform.position, target.position) > attackRange + targetSize)
                    {
                        moveTowardTarget(target.position);
                    }
                    else
                    {
                        setDir(target.position - transform.position); //Target�� �ٶ� �� ����
                        attack();
                    }
                }
                else
                {
                    if (Vector2.Distance(transform.position, target.position) > throwRange + targetSize)
                    {
                        moveTowardTarget(target.position);
                    }
                    else
                    {
                        setDir(target.position - transform.position); //Target�� �ٶ� �� ����
                        doThrow();
                    }
                }
            }


            return;
        }
    }

    public void AttachUI()
    {
        UIMgr.Inst.hp.Set((int)curHP);
        UIMgr.Inst.hp.SetMaxHP((int)maxHP);
        UIMgr.Inst.joystick.setTarget(GetInput);
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
        if (isDead) return;

        if(target == null)
        {
            targetIcon.Target = null;
            targetIcon.gameObject.SetActive(false);
        }
        else
        {
            targetIcon.Target = target;
            targetIcon.gameObject.SetActive(true);
        }
    }

    #region ����
    void attack()
    {
        anim.SetBool("isMoving", false);
        SoundMgr.Inst.Play("Attack");
        GlobalEvent.Inst.AttackEvent();
        anim.SetTrigger("doAttack");
    }

    void doAttack()
    {
        if (target != null)
        {
            if (target.TryGetComponent<CharacterBase>(out CharacterBase cb))
            {
                //Status ����
                cb.onHit(transform, Stat.STR);
            }
        }

        InvokeOnAttack();
    }
    #endregion
    #region ��ô
    public Transform holdPos;
    public HoldingItem curHoldingItem;
    public void HoldItem(HoldingItem holdItem)
    {
        curHoldingItem = Instantiate(holdItem);
        curHoldingItem.transform.parent = holdPos;
        curHoldingItem.transform.localPosition = Vector3.zero;

        holdStart();
    }
    void holdStart()
    {
        isHolding = true;
        anim.SetBool("isHolding", true);
    }

    void doThrow()
    {
        if (!curHoldingItem.canThrow) return;

        SoundMgr.Inst.Play("Throw");
        anim.SetBool("isHolding", false);
        anim.SetTrigger("doThrow");
    }
    void throwItem()
    {
        isHolding = false;
        if (curHoldingItem == null) return;
        if (target != null)
        {
            Projectile projectile = Instantiate(curHoldingItem.projectile);
            projectile.gameObject.layer = LayerMask.NameToLayer("AllyAttack");
            projectile.Shoot(transform.position, target.position);
            projectile.moduleAttack.dmg = Stat.STR + Stat.ACC;
            InvokeOnThrow(projectile);
        }

        Destroy(curHoldingItem.gameObject);
        curHoldingItem = null;

    }
    #endregion
    public void AutoMove(Vector3 destination)
    {
        StartCoroutine(co_AutoMove(destination));
    }
    IEnumerator co_AutoMove(Vector3 destination)
    {
        isAutoMove = true;
        float originMoveSpd = moveSpeed;
        moveSpeed = 5.0f;
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

    #region ü�� ����
    public void Heal(int amount)
    {
        curHP += amount;
        if (curHP > maxHP) curHP = maxHP;

        UIMgr.Inst.hp.SetMaxHP((int)maxHP);
        UIMgr.Inst.hp.Set((int)curHP);
    }
    public override void onHit(Transform attackerPos, float dmg, float stunTime = 0.5f)
    {
        if (isInvincible) return;

        Vector3 hitVec = (transform.position - attackerPos.position).normalized;

        SoundMgr.Inst.Play("PlayerHit");
        bool resisted = resistenceChain.ApplyResistance(false);

        hit.FlashWhite(0.2f);
        hit.HitEffect(hitVec, size);

        StartCoroutine(co_Invincible(resisted, hitVec));

    }


    /// <summary>
    /// �������� ������, �����ð��� �ο��ϸ� ������ ���ִ� �ڷ�ƾ
    /// </summary>
    /// <param name="resisted">������ ���� ���� ���׿� �����ߴٸ� false</param>
    /// <returns></returns>
    IEnumerator co_Invincible(bool resisted, Vector3 hitVec)
    {
        isInvincible = true;
        if(!resisted) curHP--;

        GameMgr.Inst.MainCam.Shake(0.15f, 50f, 0.12f, 0f);
        GameMgr.Inst.MainCam.Zoom(0.15f, 0.98f);
        GameMgr.Inst.SlowTime(0.6f, 0.3f, true);
        if (curHP <= 0)
        {
            StartCoroutine(co_Smash((hitVec)));
            yield break;
        }

        UIMgr.Inst.hp.Set((int)curHP);
        hit.FlashWhite(0.3f);
        hit.Togle(2.0f);

        yield return new WaitForSeconds(2.0f);

        isInvincible = false;
    }
    IEnumerator co_Smash(Vector3 hitVec)
    {
        UIMgr.Inst.hp.Set(0);

        isDead = true;
        Destroy(targetIcon.gameObject);

        GameObject camTarget = new GameObject();
        camTarget.name = "CamTarget";
        camTarget.transform.position = transform.position;
        GameMgr.Inst.MainCam.changeTarget(camTarget.transform);
        anim.SetBool("isMoving", false);
        SoundMgr.Inst.BGMFadeout();

        GameMgr.Inst.SlowTime(1.0f, 0.3f, true);
        GameMgr.Inst.MainCam.Shake(1.0f, 15f, 0.12f, 0f);
        GameMgr.Inst.MainCam.Zoom(1.0f, 0.96f);

        hit.FlashWhite(0.3f);
        hit.Togle(1.0f);

        bool isGameOver = GameMgr.Inst.SaveCurRunData();

        SoundMgr.Inst.Play("PlayerHit");
        yield return new WaitForSeconds(0.3f);

        Destroy(GetComponent<Collider2D>());
        rb.AddForce(hitVec * 20, ForceMode2D.Impulse);
        rb.gravityScale = 1.0f;

        yield return new WaitForSeconds(1.2f);
        if (isGameOver) GameMgr.Inst.ShowDefeated();
        else LoadSceneMgr.LoadSceneAsync("GameOver");
    }
    void onMove()
    {
        particle.Play();
        SoundMgr.Inst.Play("Step");
        InvokeOnMovement();
    }
    #endregion

    #region ���� ���� �� ����� �Լ�
    public void StartDeadMotion()
    {
        anim.SetBool("isDead", true);
    }
    public void EndDeadMotion()
    {
        anim.SetBool("isDead", false);
    }
    #endregion

#if UNITY_EDITOR
    [ContextMenu("KILL")]
    void deathTest()
    {
        StartCoroutine(co_Smash(Vector3.right));
    }

    [ContextMenu("GODMOD")]
    void godMode()
    {
        findRange = 20;
        attackRange = 20;
        Stat.STR = 100;
    }
#endif
}
