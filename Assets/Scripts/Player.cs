using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : CharacterBase
{
    #region ������Ʈ ����
    [Header("������Ʈ ����")]
    [SerializeField] Transform atk;
    [SerializeField] Transform spriteGroup;
    [SerializeField] ParticleSystem particle;
    #endregion


    #region �Է� ����
    Vector3 inputVec;
    Vector3 lastVec = Vector3.right;
    bool isSpace;
    #endregion

    #region ���º���
    bool attackSwitch = false;
    #endregion
    private void Awake()
    {
        evnt.moveEffect = () => particle.Play();
    }


    private void Update()
    {
        getInput();

        //�ӽ��ڵ�

        if (isSpace)
        {
            attack();
        }

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
    //�׽�Ʈ�ڵ�
    [SerializeField] AttackCol a;

    void attack()
    {
        a.Attack();

        attackSwitch = !attackSwitch;

        anim.SetTrigger("doAttack");
        anim.SetBool("attackSwitch", attackSwitch);

        atk.position = aim.position;

        float angle = Mathf.Atan2(lastVec.y, lastVec.x) * Mathf.Rad2Deg;
        atk.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        atk.GetComponent<Animator>().SetTrigger("doAttack");
    }

    void getInput()
    {
        inputVec = (Vector2.right * Input.GetAxisRaw("Horizontal") + Vector2.up * Input.GetAxisRaw("Vertical")).normalized;
        if (inputVec != Vector3.zero) lastVec = inputVec;
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

}
