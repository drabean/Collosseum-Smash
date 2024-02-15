using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterEyeband : Equip
{
    public override void onEquip(Player player)
    {
        base.onEquip(player);
        owner.findRange -= 2.5f;
        owner.Stat.STR++;
        owner.Stat.SPD++;
        owner.SetStatus();
    }
    public override void onUnEquip(Player player)
    {
        owner.findRange += 2.5f;
        owner.Stat.STR--;
        owner.Stat.SPD--;
        owner.SetStatus();
    }

}
