using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpDagger : Equip
{
    public override void onEquip(Player player)
    {
        base.onEquip(player);
        owner.onThrow += Sharpen;

    }
    public override void onUnEquip(Player player)
    {
        owner.onThrow -= Sharpen;
    }

    void Sharpen(Projectile projectile)
    {
        projectile.moduleAttack.maxHit += 2;
        projectile.moduleAttack.cantPenetrate = false;
    }
}
