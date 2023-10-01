using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandantOfWind : Equip
{
    public override void onEquip(Player player)
    {
        base.onEquip(player);
        owner.stat.SPD++;
    }
    public override void onUnEquip(Player player)
    {
        owner.stat.SPD--;
    }
}
