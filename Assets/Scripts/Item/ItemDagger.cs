using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDagger : Item
{
    public LayerMask layer;
    Attack DaggerPrefab;
    private void Awake()
    {
        DaggerPrefab = Resources.Load<Attack>("Prefabs/Attack/Dagger");
    }
    protected override void onAcquired(Player player)
    {
        //CircleCast를 통해 주변 모든 Enemy Layer 오브젝트 검색
        RaycastHit2D[] hits =  Physics2D.CircleCastAll(transform.position, 10.0f, Vector3.forward, 0f, layer);
        Transform target;

        if (hits.Length == 0)
        {
            target = player.aim;
            return;
        }
        else
        {

            float maxLength = Vector3.Distance(transform.position, hits[0].transform.position);
            target = hits[0].transform;
            Debug.Log("NAME" + hits[0].transform.gameObject.name);
            //가장 멀리있는 Enemy 찾기
            for (int i = 1; i < hits.Length; i++)
            {
                Debug.Log("NAME" + hits[i].transform.gameObject.name);
                //TODO: 더 빠르고 효율적인 코드 찾아보기
                float dist = Vector3.Distance(transform.position, hits[i].transform.position);
                if (dist > maxLength)
                {
                    target = hits[i].transform;
                    maxLength = dist;
                }
            }
        }

        Attack dagger = Instantiate<Attack>(DaggerPrefab);
        dagger.Shoot(transform.position, target.position);
    }
}
