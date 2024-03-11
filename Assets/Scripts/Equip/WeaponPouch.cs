using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPouch : Equip
{
    public override void onEquip(Player player)
    {
        base.onEquip(player);
        GameMgr.Inst.dropCount -= 2;
    }
    public override void onUnEquip(Player player)
    {
        GameMgr.Inst.dropCount += 2;
    }
}
