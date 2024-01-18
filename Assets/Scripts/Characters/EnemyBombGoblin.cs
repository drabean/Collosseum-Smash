using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBombGoblin : Enemy
{
    public float attackWaitTime;
    public float explosionRange;

    bool isExploded;

    public Attack Explosion;
    public GameObject prefab;
    public void Awake()
    {
        onDeath += spawnOnDeath;
    }
    public override void StartAI()
    {
        StartCoroutine(co_Chase());
    }
    void spawnOnDeath(Vector3 pos)
    {
        Instantiate(prefab, transform.position, Quaternion.identity);
    }
    IEnumerator co_Chase()
    {
        while(!isDead)
        {
            moveTowardTarget(Target.transform.position);
            if(Vector3.Distance(transform.position, Target.transform.position) <= explosionRange && !isExploded)
            {
                isExploded = true;
                StartCoroutine(co_Explode());
            }
            yield return null;
        }
    }

    IEnumerator co_Explode()
    {
        hit.FlashWhite(2.0f);
        moveSpeed *= 0.6f;
        yield return new WaitForSeconds(attackWaitTime);
        GameMgr.Inst.MainCam.Shake(0.15f, 30, 0.25f, 0f);
        Instantiate(Explosion, transform.position, Quaternion.identity).Shoot(transform.position, transform.position);
        onDeath -= spawnOnDeath;
        invokeOnDeath(Vector3.zero);
        Despawn();
    }



}
