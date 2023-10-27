using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRapid : Attack
{
    [SerializeField] Projectile projectile;
    public float repNum;
    public float interval;
    public float acc; // 공격 정확도

    public override void Shoot(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(co_Shoot(startPos, targetPos));
    }

    IEnumerator co_Shoot(Vector3 startPos, Vector3 targetPos)
    {
        for (int i = 0; i < repNum; i++)
        {
            Instantiate<Projectile>(projectile).Shoot(startPos, targetPos + Random.Range(-acc, acc) * Vector3.right + Random.Range(-acc, acc) * Vector3.up);
            yield return new WaitForSeconds(interval);
        }

        Destroy(gameObject);
    }

    public override GameObject ShowWarning(Vector3 startPos, Vector3 targetPos, float time)
    {
        return projectile.ShowWarning(startPos, targetPos, time);
    }
}
