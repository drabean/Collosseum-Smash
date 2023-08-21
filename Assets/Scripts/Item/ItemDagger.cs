using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDagger : Item
{
    Attack DaggerPrefab;
    private void Awake()
    {
        DaggerPrefab = Resources.Load<Attack>("Prefabs/Attack/Dagger");
    }
    protected override void onAcquired(Player player)
    {
        RaycastHit2D[] hits =  Physics2D.CircleCastAll(transform.position, 7.0f, Vector3.forward, LayerMask.NameToLayer("Enemy"));

        if (hits.Length == 0) return;

        Transform target = hits[0].transform;

        Attack dagger = Instantiate<Attack>(DaggerPrefab);
        dagger.Shoot(transform.position, target.position);
    }
}
