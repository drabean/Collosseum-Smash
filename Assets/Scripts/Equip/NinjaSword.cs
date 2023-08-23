using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaSword : EquipWeapon
{
    public LayerMask layer;
    Attack daggerPrefab;

    public override void onEquip(Player player)
    {
        base.onEquip(player);
        daggerPrefab = Resources.Load<Attack>("Prefabs/Attack/NinjaDagger");
    }
    protected override void specialAttack()
    {
        base.specialAttack();
        //CircleCast�� ���� �ֺ� ��� Enemy Layer ������Ʈ �˻�
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 4.0f, Vector3.forward, 0f, layer);

        if (hits.Length == 0) return;

        int targetCount = hits.Length < 3 ? hits.Length : 3;

        for (int i = 0; i < targetCount; i++)
        {

            Attack dagger = Instantiate<Attack>(daggerPrefab);
            dagger.Shoot(transform.position, hits[i].transform.position);
        }

    }
}
