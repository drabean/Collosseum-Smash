using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFireball : Item
{
    public LayerMask layer;
    public Attack FireBallPrefab;

    protected override void onAcquired(Player player)
    {
        //CircleCast를 통해 주변 모든 Enemy Layer 오브젝트 검색
        RaycastHit2D[] hits =  Physics2D.CircleCastAll(transform.position, 10.0f, Vector3.forward, 0f, layer);
        Transform target;

        if (hits.Length == 0)
        {
            target = player.aim;
        }
        else
        {

            float minLength = Vector3.Distance(transform.position, hits[0].transform.position);
            target = hits[0].transform;
            //가장 가까이 있는 Enemy 찾기
            for (int i = 1; i < hits.Length; i++)
            {
                //TODO: 더 빠르고 효율적인 코드 찾아보기
                float dist = Vector3.Distance(transform.position, hits[i].transform.position);
                if (dist < minLength)
                {
                    target = hits[i].transform;
                    minLength = dist;
                }
            }
        }

        Attack Fireball = Instantiate(FireBallPrefab);
        Fireball.Shoot(transform.position, target.position);
    }
    protected override IEnumerator co_AcquireItem()
    {
        Material origin = sp.material;
        sp.material = Resources.Load<Material>("Materials/FlashWhite");

        yield return new WaitForSeconds(0.2f);
        sp.material = origin;
        Destroy(gameObject);
    }
}
