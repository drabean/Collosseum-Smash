using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OPG : Equip
{
    public override void onEquip(Player player)
    {
        base.onEquip(player);
        owner.Stat.STR += 2;
        owner.Stat.SPD -= 1;
        owner.SetStatus();
    }
    public override void onUnEquip(Player player)
    {
        owner.Stat.STR -= 2;
        owner.Stat.SPD += 1;
        owner.SetStatus();
    }
}
