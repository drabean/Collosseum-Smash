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
        //CircleCast�� ���� �ֺ� ��� Enemy Layer ������Ʈ �˻�
        RaycastHit2D[] hits =  Physics2D.CircleCastAll(transform.position, 10.0f, Vector3.forward, 0f, layer);
        Transform target;

        if (hits.Length == 0)
        {
            target = player.aim;
        }
        else
        {

            float maxLength = Vector3.Distance(transform.position, hits[0].transform.position);
            target = hits[0].transform;
            //���� �ָ��ִ� Enemy ã��
            for (int i = 1; i < hits.Length; i++)
            {
                //TODO: �� ������ ȿ������ �ڵ� ã�ƺ���
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
    protected override IEnumerator co_AcquireItem()
    {
        Material origin = sp.material;
        sp.material = Resources.Load<Material>("Materials/FlashWhite");

        yield return new WaitForSeconds(0.2f);
        InvokeOnAcquire();
        sp.material = origin;
    }
}
