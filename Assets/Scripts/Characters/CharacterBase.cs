using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    #region ������Ʈ ����
    [Header("������Ʈ ����")]
    [SerializeField] protected SpriteRenderer sp;
    [SerializeField] protected Animator anim;
    [SerializeField] protected AnimationEventReciever evnt;
    [SerializeField] protected Rigidbody2D rb;

    [SerializeField] protected Transform aim;
    [SerializeField] protected ModuleHit hit;
    #endregion

    #region ���º���
    [Header("���º���")]
    public bool isDead;

    #endregion

    #region �������ͽ�
    [Header("Status")]
    public float moveSpeed;
    public float aimRange;
    #endregion


    /// <summary>
    ///  �ǰ� �Լ� (���������� ȣ��)
    ///  
    /// </summary>
    /// <param name="attackerPos">���ظ� �����µ� �����ߴٸ� T, ���� �� F</param>
    /// <returns></returns>
    public virtual bool Hit(Transform attackerPos)
    {
        return true;
    }

    public virtual void Stun(Transform attackerPos)
    {

    }
    /// <summary>
    /// ��ǥ �������� �̵���Ű�� �Լ�
    /// </summary>
    /// <param name="target"></param>
    protected virtual void moveTowardTarget(Vector3 target)
    {
        Vector3 moveVec = (target - transform.position).normalized;

        moveToDir(moveVec);
    }


    /// <summary>
    /// ����ȭ�� ���͸� �־� �ش� �������� �̵���Ű�� �Լ�.
    /// </summary>
    /// <param name="dir"></param>
    protected virtual void moveToDir(Vector3 dir)
    {
        if (dir.magnitude > 1) dir = dir.normalized;

        anim.SetBool("isMoving", true);

        setDir(dir);
       // transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, moveSpeed * Time.deltaTime);
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    protected virtual void setDir(Vector3 dir)
    {
        dir = dir.normalized;
        anim.SetFloat("dirX", dir.x);
        anim.SetFloat("dirY", dir.y);
        if (dir.x != 0) sp.flipX = dir.x < 0 ? true : false;
        aim.transform.localPosition = dir * aimRange;
    }
}
