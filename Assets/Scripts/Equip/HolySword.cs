using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolySword : Equip
{
    public int reqCombo;
    Attack projectile;

    public override void onEquip(Player player)
    {
        base.onEquip(player);
        owner.onCombo += holySlash;
        projectile = Resources.Load<Attack>("Prefabs/Attack/HolySlash");
    }

    public override void onUnEquip(Player player)
    {
        owner.onCombo -= holySlash;
    }

    void holySlash(int combo)
    {
        if(combo % reqCombo == 0 && combo > 0)
        {
            Instantiate<Attack>(projectile, transform.position, Quaternion.identity).Shoot(transform.position, owner.aim.transform.position);
        }
    }

}
