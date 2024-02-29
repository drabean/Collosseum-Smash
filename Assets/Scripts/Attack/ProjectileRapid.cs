using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRapid : Attack
{
    [SerializeField] Projectile projectile;
    public float repNum;
    public float interval;
    public float acc; // ���� ��Ȯ��

    public override void Shoot(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(co_Shoot(startPos, targetPos));
    }

    IEnumerator co_Shoot(Vector3 startPos, Vector3 targetPos)
    {
        Instantiate<Projectile>(projectile).Shoot(startPos, targetPos); // ��ź�� ��Ȯ�ϰ�
        yield return new WaitForSeconds(interval);
        for (int i = 1; i < repNum; i++)
        {
            Instantiate<Projectile>(projectile).Shoot(startPos, targetPos.Randomize(acc));
            yield return new WaitForSeconds(interval);
        }

        Destroy(gameObject);
    }

    public override GameObject ShowWarning(Vector3 startPos, Vector3 targetPos, float time, float size = 1)
    {
        return projectile.ShowWarning(startPos, targetPos, time);
    }
}
