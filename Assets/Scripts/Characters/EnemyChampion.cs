using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChampion : Enemy
{
    [SerializeField] Transform spriteGroup;



    //���� ��, �÷��̾�� ���� ��ȸ�� �ִ� �ð�. (Ż�� �ð�)
    public float fatigueTime;
    //Ż������ �ʿ��� ���� Ƚ��
    public int attackCount;
    //
    int attackCountLeft;

    [Header("����1")]
    public float pat1Range;//���� ����Ʈ ��ȯ����
    public float pat1Width;//���� ����Ʈ Ÿ������ �ʺ�
    public string pat1AtkName;
    public float pat1WaitTime;
    public float pat1IntervalTime;
    public float pat1WaitAfterTime;
    public int pat1RepeatTime;

    [Header("����2")]
    public float pat2Range;
    public string pat2AtkName;
    public float pat2WaitTime;
    public float pat2IntervalTime;
    public float pat2WaitAfterTime;
    public int pat2RepeatTime;




    #region Override
    private void Awake()
    {
        evnt.attack = doPat1;
        evnt.attack2 = doPat2;
        projectile = Resources.Load<Attack>(projectileName);

        attackCountLeft = attackCount;
    }

    Vector3 minVec = new Vector3(-1, 1, 1);


    public override void StartAI()
    {
        StartCoroutine(co_Chase(co_Pat1()));
    }


    /// <summary>
    /// ���� �ൿ�� ����
    /// </summary>
    void selectNextMove()
    {
        if(attackCountLeft > 0)
        {
            attackCountLeft--;

            int selecter = Random.Range(0, 4);

            switch(selecter)
            {
                case 0:
                    StartCoroutine(co_Chase(co_Pat1()));
                    break;
                case 1:
                    StartCoroutine(co_Runaway(co_Pat2()));
                    break;
                case 2:
                    StartCoroutine(co_Wander(co_Chase(co_Pat1())));
                    break;    
                case 3:
                    StartCoroutine(co_Wander(co_Pat2()));
                    break;
            }
        }
        else // ���� �غ� �ȵǾ����Ƿ�, �׳� �̵�.
        {
            attackCountLeft = attackCount;
            StartCoroutine(cofatigue());
        }
    }

    IEnumerator cofatigue()
    {
        anim.SetBool("isFatigue", true);
        yield return new WaitForSeconds(fatigueTime);
        anim.SetBool("isFatigue", false);
        selectNextMove();
    }

    #endregion

    #region �̵�

    //Ÿ���� ���ؼ� �̵��ϴ� stste
    IEnumerator co_Chase(IEnumerator nextMove = null)
    {
        moveSpeed = 6f;
        while (Vector3.Distance(transform.position, Target.transform.position) >= pat1Range)
        {
            moveTowardTarget(Target.transform.position);
            yield return null;
        }
        moveTowardTarget(Target.transform.position);

        if (nextMove != null) StartCoroutine(nextMove);
        else selectNextMove();
    }
    //Ÿ���� �ݴ�������� �̵��ϴ� state
    IEnumerator co_Runaway(IEnumerator nextMove = null)
    {
        moveSpeed = 2f;
        float runTimeLeft = 0.4f;

        while (runTimeLeft >= 0)
        {
            runTimeLeft -= Time.deltaTime;
            moveToDir(transform.position - Target.transform.position);
            yield return null;
        }

        if (nextMove != null) StartCoroutine(nextMove);
        else selectNextMove();
    }
    //Target�� �߽����� �� �� �� ������ �������� ���Ƽ� �̵��ϴ� state
    IEnumerator co_Wander(IEnumerator nextMove = null)
    {
        moveSpeed = 4f;
        float wanderTime = Random.Range(0.6f, 1.3f);//�̵��� �ð��� ����
        bool isReversed = Random.Range(0, 2) == 0;//ȸ�� ������ ����

        while (wanderTime >= 0)
        {
            Vector2 moveVec = (Target.transform.position - transform.position);
            moveVec = Vector2.right * moveVec.y * (-1) + Vector2.up * moveVec.x;
            if (isReversed) moveVec *= -1;
            moveToDir(moveVec);
            wanderTime -= Time.deltaTime;
            yield return null;
        }


        if (nextMove != null) StartCoroutine(nextMove);
        else selectNextMove();
    }
    #endregion
    //patttern1 state
    IEnumerator co_Pat1(IEnumerator nextMove = null)
    {
        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", true);
        curAttackWarning = GameMgr.Inst.AttackEffectCircle(aim.position, pat1Width, pat1WaitTime);
        yield return new WaitForSeconds(pat1WaitTime);
        anim.SetBool("isReady", false);


        for(int i = 0; i < pat1RepeatTime; i++)
        {
            anim.SetTrigger("doAttack");
            yield return new WaitForSeconds(pat1IntervalTime);
        }
        yield return new WaitForSeconds(pat1WaitAfterTime);

        if (nextMove != null) StartCoroutine(nextMove);
        else selectNextMove();
    }

    //���� ������ �ϴ� �Լ�. (Animation Event�� ���� ȣ��)
    void doPat1()
    {
        ModuleAttack atk = DictionaryPool.Inst.Pop(pat1AtkName).GetComponent<ModuleAttack>();
        atk.transform.position = aim.position;
        atk.ownerTr = transform;
        transform.position += (aim.position - transform.position).normalized * 0.5f; //���� �� �� ����, ���� �������� �ణ �̵�

        GameMgr.Inst.Shake(0.1f, 40, 0.2f);
    }
    //Pattern2 state
    IEnumerator co_Pat2(IEnumerator nextMove = null)
    {
        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", true);

        curAttackWarning = projectile.ShowWarning(transform.position, Target.transform.position, pat2WaitTime);
        setDir((Target.transform.position - transform.position).normalized);

        yield return new WaitForSeconds(pat2WaitTime);
        anim.SetBool("isReady", false);


        for (int i = 0; i < pat1RepeatTime; i++)
        {
            anim.SetTrigger("doThrow");
            setDir((Target.transform.position - transform.position).normalized);
            yield return new WaitForSeconds(pat2IntervalTime);
        }
        yield return new WaitForSeconds(pat2WaitAfterTime);

        if (nextMove != null) StartCoroutine(nextMove);
        else selectNextMove();
    }

    Attack projectile;
    public string projectileName;

    Attack curProjectile;
    //���� ������ �ϴ� �Լ�. (Animation Event�� ���� ȣ��)
    void doPat2()
    {
        curProjectile = Instantiate<Attack>(projectile, transform.position, Quaternion.identity);
        curProjectile.Shoot(transform.position, aim.transform.position);
    }
}
