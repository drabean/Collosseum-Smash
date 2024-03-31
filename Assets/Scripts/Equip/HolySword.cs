using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolySword : EquipWeapon
{
    Projectile projectile;

    public override void onEquip(Player player)
    {
        base.onEquip(player);
        projectile = Resources.Load<Projectile>("Prefabs/Attack/HolySlash");
    }

    protected override void specialAttack()
    {
        base.specialAttack();
        Projectile special = Instantiate(projectile, transform.position, Quaternion.identity);
        special.moduleAttack.dmg = owner.Stat.STR;
        special.Shoot(transform.position, owner.aim.transform.position);

    }

}

