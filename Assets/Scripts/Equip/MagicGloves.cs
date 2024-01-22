using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicGloves : Equip
{

    public override void onEquip(Player player)
    {
        base.onEquip(player);
        owner.Stat.ACC+=2;
    }
    public override void onUnEquip(Player player)
    {
        owner.Stat.ACC -= 2;
    }
}
