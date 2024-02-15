using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaSword : Equip
{
    public LayerMask layer;
    public Projectile daggerPrefab;

    public override void onEquip(Player player)
    {
        base.onEquip(player);
        player.onThrow += throwDagger;
    }

    public override void onUnEquip(Player player)
    {
    }

    protected void throwDagger(Projectile projectile)
    {

        //CircleCast를 통해 주변 모든 Enemy Layer 오브젝트 검색
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
