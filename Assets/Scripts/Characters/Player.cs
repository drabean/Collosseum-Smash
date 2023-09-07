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
    #region ������Ʈ ����
    [Header("������Ʈ ����")]
    [SerializeField] Transform spriteGroup;
    [SerializeField] ParticleSystem particle;
    public IconHolder iconHolder;
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
    #endregion

    #region �÷��̾� 
    public STATUS _stat;
    //TODO: ���ݹ��� ���� �߰� (������ X)
    public float findRange;        
    public float attackRange;

    Transform target;
    /// <summary>
    /// stat�� ������� ���� �����Ű�� �Լ�
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

        //�������۸��
        if (inputVec != Vector3.zero && !commandLock)
        {
            moveToDir(inputVec);
            return;
        }//�ڵ����۸��
        else
        {
            findTarget();

            if(target == null) anim.SetBool("isMoving", false); // ���� ���� Ÿ���� ���ٸ� Idle���·�
            else
            {
                if(Vector2.Distance(transform.position, target.position) > attackRange)
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
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 10.0f, Vector3.forward, 0f,layer);

        if (hits.Length == 0) //�������� ���� Target�� �������� ����
        {
            target = null;
            return;
        }
        else
        {
            target = hits[0].transform;

            float minLength = Vector3.Distance(transform.position, hits[0].transform.position);
            target = hits[0].transform;
            //���� �ָ��ִ� Enemy ã��
            for (int i = 1; i < hits.Length; i++)
            {
                //TODO: �� ������ ȿ������ �ڵ� ã�ƺ���
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
            //Status ����
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
    /// �������� ������, �����ð��� �ο��ϸ� ������ ���ִ� �ڷ�ƾ
    /// </summary>
    /// <param name="resisted">������ ���� ���� ���׿� �����ߴٸ� false</param>
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
    /// ���� Ÿ�� ���� ���� ��, ȣ��
    /// </summary>
    /// <param name="isMelee"></param>
    public void HitSuccess()
    {
        int curCombo = combo.increaseCombo();
        invokeOnSmash();
    }
}
