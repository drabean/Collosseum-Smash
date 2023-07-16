using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region ������Ʈ ����
    [Header("������Ʈ ����")]
    [SerializeField] SpriteRenderer sp;
    [SerializeField] Animator anim;
    [SerializeField] AnimationEventReciever evnt;
    [SerializeField] ParticleSystem particle;

    #endregion

    #region ������Ʈ ����
    [Header("������Ʈ ����")]
    [SerializeField] Transform aim;
    [SerializeField] Transform atk;
    #endregion

    #region �Է� ����
    Vector3 inputVec;
    Vector3 lastVec = Vector3.right;
    bool isSpace;
    #endregion

    #region �������� ����
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

        //�ӽ��ڵ�

        if (isSpace)
        {
            attack();

            return;
        }

        if (inputVec != Vector3.zero)
        {
            setAnimState();

            anim.SetBool("isMoving", true);
            transform.position += inputVec * 2.5f * Time.deltaTime;
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

    void setAnimState()
    {
        anim.SetFloat("dirX", inputVec.x);
        anim.SetFloat("dirY", inputVec.y);

        aim.transform.localPosition = inputVec * 0.8f;
        if (inputVec.x != 0) sp.flipX = inputVec.x < 0 ? true : false;
        


    }
}
