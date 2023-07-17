using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CharacterBase
{
    #region 오브젝트 참조
    [Header("오브젝트 참조")]
    [SerializeField] Transform atk;
    [SerializeField] ParticleSystem particle;
    #endregion


    #region 입력 변수
    Vector3 inputVec;
    Vector3 lastVec = Vector3.right;
    bool isSpace;
    #endregion

    #region 상태변수
    bool commandLock;
    bool attackSwitch = false;
    #endregion
    private void Awake()
    {
        evnt.commandLockStart = () => commandLock = true;
        evnt.commandLockEnd = () => commandLock = false;
        evnt.moveEffect = () => particle.Play();
    }


    private void Update()
    {
        if (commandLock) return;
        getInput();

        //임시코드

        if (isSpace)
        {
            attack();

            return;
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


    void attack()
    {
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
}
