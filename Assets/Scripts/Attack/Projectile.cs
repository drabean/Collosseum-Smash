using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Attack
{
    public float moveSpeed;
    public float lifeTime;
    public bool isRotate;

    public float warningRadius = 0.7f;
    public Vector3 moveVec;
    [SerializeField]    SpriteRenderer sp;
    public ModuleAttack moduleAttack;
    public override GameObject ShowWarning(Vector3 startPos, Vector3 targetPos, float time, float size = 1)
    {
        float tempSpd = moveSpeed == 0 ? 30 : moveSpeed; // 움직이지 않는 투사체를 위한 값 조절
        curWarning = GameMgr.Inst.AttackEffectLinear(startPos, startPos + (targetPos - startPos).normalized * (tempSpd * lifeTime), warningRadius, time);
        return curWarning;
    }

    public override void Shoot(Vector3 startPos, Vector3 targetPos)
    {
        if (SFXName != "") SoundMgr.Inst.Play(SFXName);
        transform.position = startPos;
        moveVec = (targetPos - startPos).normalized;
        if(sp != null) sp.flipX = (moveVec.x < 0);
        if(moveVec == Vector3.zero)
        {
            Debug.Log("잘못된 방향입니다.");//00으로 발사시 가만히 잇는 현상 방지.
            moveVec = Vector3.right;
            return;
        }
        if (isRotate) transform.rotation = moveVec.ToQuaternion();
        StartCoroutine(co_Shoot());
    }

    IEnumerator co_Shoot()
    {
        while(lifeTime >= 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + moveVec, moveSpeed * Time.deltaTime);

            lifeTime -= Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

}
