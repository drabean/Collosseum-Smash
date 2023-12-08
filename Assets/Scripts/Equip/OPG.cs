using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OPG : Equip
{
    public override void onEquip(Player player)
    {
        base.onEquip(player);
        owner.Stat.STR++;
    }
    public override void onUnEquip(Player player)
    {
        owner.Stat.STR--;
    }
}
