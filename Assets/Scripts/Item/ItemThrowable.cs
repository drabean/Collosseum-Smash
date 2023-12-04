using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemThrowable : Item
{
    public HoldingItem item;


    protected override void OnTriggerEnter2D(Collider2D collision)
    {

        Player target;
        if (collision.TryGetComponent<Player>(out target))
        {
            if (!target.isHolding)
            {
                onAcquired(target);
                StartCoroutine(co_AcquireItem());
            }
        }
    }
    protected override void onAcquired(Player player)
    {
        if (player.isHolding) return;

        base.onAcquired(player);
        player.HoldItem(item);
    }
}
