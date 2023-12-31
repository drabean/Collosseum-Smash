using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    #region 컴포넌트 참조
    [Header("컴포넌트 참조")]
    [SerializeField] protected SpriteRenderer sp;
    [SerializeField] protected Animator anim;
    [SerializeField] protected AnimationEventReciever evnt;
    [SerializeField] protected Rigidbody2D rb;

    public Transform aim;
    public ModuleHit hit;
    #endregion

    #region 상태변수
    [Header("상태변수")]
    public bool isDead;

    #endregion

    #region 스테이터스
    [Header("Status")]
    public float maxHP;
    public float curHP;
    public float moveSpeed;
    public float moveSpeedCo = 1;
    public float aimRange;
    public float size;
    #endregion

    /// <summary>
    /// 피격시 공격자 측에서 호출시키는 함수.
    /// </summary>
    /// <param name="attackerPos"></param>
    /// <param name="dmg"></param>
    /// <param name="isStun"></param>
    /// <param name="stunTime"></param>
    public virtual void onHit(Transform attackerPos, float dmg, float stunTime = 0.0f)
    {

    }
    /// <summary>
    /// 목표 지점으로 이동시키는 함수
    /// </summary>
    /// <param name="target"></param>
    protected virtual void moveTowardTarget(Vector3 target, bool useCoeffient = true)
    {
        Vector3 moveVec = (target - transform.position).normalized;

        moveToDir(moveVec, useCoeffient);
    }


    /// <summary>
    /// 정규화된 벡터를 넣어 해당 방향으로 이동시키는 함수.
    /// </summary>
    /// <param name="dir"></param>
    protected virtual void moveToDir(Vector3 dir, bool useCoeffient = true)
    {
        if (dir.magnitude > 1) dir = dir.normalized;

        anim.SetBool("isMoving", true);

        setDir(dir);
        // transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, moveSpeed * Time.deltaTime);

        transform.position += dir * moveSpeed * (useCoeffient ? moveSpeedCo : 1) * Time.deltaTime;
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
