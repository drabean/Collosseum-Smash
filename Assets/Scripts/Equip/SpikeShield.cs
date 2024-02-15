using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeShield : Equip
{
    public override void onEquip(Player player)
    {
        base.onEquip(player);
        player.actionHit += Spike;

    }
    public override void onUnEquip(Player player)
    {
        player.actionHit -= Spike;
    }

    bool Spike(bool resisted)
    {   
        DictionaryPool.Inst.Pop("Prefabs/Effect/SpikeShieldEffect").transform.position = owner.transform.position;

        return resisted;

    }
}
