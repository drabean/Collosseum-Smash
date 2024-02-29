using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileParabola : Attack
{
    public Impact targetAttack;// ���������� ��ȯ�� ����
    public float moveTime; // ���� ���� �������� �ɸ��� �ð�
    public Animator anim;

    float moveSpeed;
    public override void Shoot(Vector3 startPos, Vector3 targetPos)
    {
        transform.position = startPos;
        moveSpeed = Vector3.Distance(startPos, targetPos) / moveTime;
        anim.SetFloat("moveTime", 1/moveTime);

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
        Destroy(gameObject);
    }

    public override GameObject ShowWarning(Vector3 startPos, Vector3 targetPos, float time,float size = 1)
    {
        return targetAttack.ShowWarning(targetPos, targetPos, moveTime);
    }
}
