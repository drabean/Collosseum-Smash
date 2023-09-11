using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChampion : EnemyBoss
{
    [SerializeField] Transform spriteGroup;

    public float fatigueTime;

    #region Override
    private void Awake()
    {
        evnt.attack = doPat1;
        evnt.attack2 = doPat2;
        projectile = Resources.Load<Attack>(patterns[1].prefabName);

        patternCountLeft = patternCount;
    }

    Vector3 minVec = new Vector3(-1, 1, 1);


    public override void StartAI()
    {
        StartCoroutine(co_Chase(co_Pat1()));
    }


    /// <summary>
    /// ���� �ൿ�� ����
    /// </summary>
    protected override void selectPattern()
    {
        if(patternCountLeft > 0)
        {
            patternCountLeft--;

            int selecter = Random.Range(0, 4);

            switch(selecter)
            {
                case 0:
                    StartCoroutine(co_Chase(co_Pat1())); // �߰� �� ��������
                    break;
                case 1:
                    StartCoroutine(co_Runaway(co_Pat2())); // ���� �� ���Ÿ� ����
                    break;
                case 2:
                    StartCoroutine(co_Wander(co_Chase(co_Pat1()))); // ���� �̵� - �߰� �� ��������
                    break;    
                case 3:
                    StartCoroutine(co_Wander(co_Pat2())); // �����̵� �� ���Ÿ�����
                    break;
            }
        }
        else // ���� ���� ���� �� �׷α�
        {
            patternCountLeft = patternCount;
            StartCoroutine(coFatigue());
        }
    }

    IEnumerator coFatigue() // �׷α� ����
    {
        anim.SetBool("isFatigue", true);
        yield return new WaitForSeconds(fatigueTime);
        anim.SetBool("isFatigue", false);
        selectPattern();
    }

    #endregion

    #region �̵�

    //Ÿ���� ���ؼ� �̵� (����)
    IEnumerator co_Chase(IEnumerator nextMove = null)
    {
        moveSpeed = 6f;
        while (Vector3.Distance(transform.position, Target.transform.position) >= patterns[0].range)
        {
            moveTowardTarget(Target.transform.position);
            yield return null;
        }
        moveTowardTarget(Target.transform.position);

        if (nextMove != null) StartCoroutine(nextMove);
        else selectPattern();
    }
    //Ÿ���� �ݴ�������� �̵� (����)
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
        else selectPattern();
    }
    //Target�� �߽����� �������� �̵�
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
        else selectPattern();
    }
    #endregion
    //Patterm1 - ��������
    IEnumerator co_Pat1(IEnumerator nextMove = null)
    {
        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", true);
        curAttackWarning = GameMgr.Inst.AttackEffectCircle(aim.position, patterns[0].width, patterns[0].waitBeforeTime);
        yield return new WaitForSeconds(patterns[0].waitBeforeTime);
        anim.SetBool("isReady", false);


        for(int i = 0; i < patterns[0].repeatTIme; i++)
        {
            anim.SetTrigger("doAttack");
            yield return new WaitForSeconds(patterns[0].intervalTime);
        }
        yield return new WaitForSeconds(patterns[0].waitAfterTime);

        if (nextMove != null) StartCoroutine(nextMove);
        else selectPattern();
    }

    //���� ������ �ϴ� �Լ�. (Animation Event�� ���� ȣ��)
    void doPat1()
    {
        ModuleAttack atk = DictionaryPool.Inst.Pop(patterns[0].prefabName).GetComponent<ModuleAttack>();
        atk.transform.position = aim.position;
        atk.ownerTr = transform;
        transform.position += (aim.position - transform.position).normalized * 0.5f; //���� �� �� ����, ���� �������� �ణ �̵�

        GameMgr.Inst.Shake(0.1f, 40, 0.2f);
    }
    //Pattern2 - ��â����
    IEnumerator co_Pat2(IEnumerator nextMove = null)
    {
        anim.SetBool("isMoving", false);
        anim.SetBool("isReady", true);

        curAttackWarning = projectile.ShowWarning(transform.position, Target.transform.position, patterns[1].waitBeforeTime);
        setDir((Target.transform.position - transform.position).normalized);

        yield return new WaitForSeconds(patterns[1].waitBeforeTime);
        anim.SetBool("isReady", false);


        for (int i = 0; i < patterns[1].repeatTIme; i++)
        {
            anim.SetTrigger("doThrow");
            setDir((Target.transform.position - transform.position).normalized);
            yield return new WaitForSeconds(patterns[1].intervalTime);
        }
        yield return new WaitForSeconds(patterns[1].waitAfterTime);

        if (nextMove != null) StartCoroutine(nextMove);
        else selectPattern();
    }
    //
    Attack projectile;

    //���� ������ �ϴ� �Լ�. (Animation Event�� ���� ȣ��)
    void doPat2()
    {
        Attack curProjectile = Instantiate<Attack>(projectile, transform.position, Quaternion.identity);
        curProjectile.Shoot(transform.position, aim.transform.position);
    }
}
