using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolySword : EquipWeapon
{
    Attack projectile;

    public override void onEquip(Player player)
    {
        base.onEquip(player);
        projectile = Resources.Load<Attack>("Prefabs/Attack/HolySlash");
    }

    protected override void specialAttack()
    {
        base.specialAttack();
        Instantiate<Attack>(projectile, transform.position, Quaternion.identity).Shoot(transform.position, owner.aim.transform.position);

    }

}

