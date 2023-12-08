using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatPotion : Equip
{
    [Range(0,4)]
    public int statIdx;

    public override void onEquip(Player player)
    {
        base.onEquip(player);
        switch(statIdx)
        {
            case 0:
                player.Stat.STR++;
                break;
            case 1:
                player.Stat.SPD++;
                break;
            case 2:
                player.Stat.VIT++;
                break;
            case 3:
                player.Stat.ACC++;
                break;
        }

        player.SetStatus();
        player.SetHPMax();
    }

    public override void onUnEquip(Player player)
    {
        switch (statIdx)
        {
            case 0:
                player.Stat.STR--;
                break;
            case 1:
                player.Stat.SPD--;
                break;
            case 2:
                player.Stat.VIT--;
                break;
            case 3:
                player.Stat.ACC--;
                break;
        }
    }
}
