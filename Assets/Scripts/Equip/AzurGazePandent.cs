using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AzurGazePandent : Equip
{
    public override void onEquip(Player player)
    {
        base.onEquip(player);
        owner.findRange += 2;
        owner.throwRange += 2;
        owner.SetStatus();
    }
    public override void onUnEquip(Player player)
    {
        owner.findRange -= 2;
        owner.throwRange -= 2;
        owner.SetStatus();
    }

}
