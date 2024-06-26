using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileParabola : Attack
{
    public Impact targetAttack;// 도착점에서 소환될 공격
    public float moveTime; // 공격 도착 시점까지 걸리는 시간
    public Animator anim;
    float moveSpeed;
    public override void Shoot(Vector3 startPos, Vector3 targetPos)
    {
        transform.position = startPos;
        moveSpeed = Vector3.Distance(startPos, targetPos) / moveTime;
        anim.SetFloat("moveTime", 1/moveTime);

        SoundMgr.Inst.Play(SFXName);
        StartCoroutine(move(targetPos));
    }

    IEnumerator move(Vector3 targetPos)
    {
        while(moveTime >= 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            moveTime -= Time.deltaTime;

            yield return null;
        }
        Instantiate(targetAttack).Shoot(targetPos, targetPos);
        attackAction?.Invoke();
        Destroy(gameObject);
    }

    public override GameObject ShowWarning(Vector3 startPos, Vector3 targetPos, float time,float size = 1)
    {
        return targetAttack.ShowWarning(targetPos, targetPos, moveTime+ time);
    }
}
