using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlimeKing : Enemy
{
    [Header("컴포넌트 참조")]
    [SerializeField] SpriteRenderer eye;
    [SerializeField] Transform eyeTr;

    [Header("패턴1")]
    public float pat1Width;//공격 이펙트 타격판정 너비
    public string pat1AtkName;
    public float pat1WaitTime;


    [Header("패턴2")]
    public float pat2Range;
    public string pat2AtkName;
    public float pat2Duration;


    protected override void setDir(Vector3 dir)
    {
        eye.sortingOrder = dir.y < 0 ? 1 : -1;
        eyeTr.localPosition = Vector3.right * 0.4f * dir.x + Vector3.up * (0.4f * dir.y + 0.6f);
        aim.transform.localPosition = dir * aimRange;
    }

    private void Awake()
    {
        evnt.attack += onAttack1;
    }

    public override void  StartAI()
    {
        StartCoroutine(co_Idle());
    }

    IEnumerator co_Idle(float time = 1.5f)
    {
        float timeLeft = time;

        while(timeLeft >= 0)
        {
            setDir((Target.transform.position - transform.position).normalized);

            timeLeft -= Time.deltaTime;
            yield return null;
        }
        StartCoroutine(co_Move());
    }

    IEnumerator co_Move()
    {
        float timeLeft = 0.5f;
        anim.SetBool("isMoving", true);

        while (timeLeft >= 0)
        {
            moveTowardTarget(aim.transform.position);
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        anim.SetBool("isMoving", false);

        StartCoroutine(co_Idle());
    }

    void onAttack1()
    {
        GameMgr.Inst.Shake(0.4f, 20, 0.15f, 0, true);
    }

    IEnumerator co_Pat2()
    {
        eyeTr.transform.localPosition = Vector3.zero;
        eye.sortingOrder = 1;

        float timeLeft = pat2Duration;
        anim.SetBool("isShaking", true);
        GameMgr.Inst.Shake(pat2Duration, 20, 0.08f, 0, true);
        while(timeLeft >= 0)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        anim.SetBool("isShaking", false);

        StartCoroutine(co_Idle());
    }
}
