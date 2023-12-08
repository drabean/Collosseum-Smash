using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShot : Equip
{
    public GameObject ItemRockPrefab;
    public override void onEquip(Player player)
    {
        base.onEquip(player);
        List<Vector2> corners = EnemyMgr.Inst.getCornerPos();

        foreach(Vector2 pos in corners)
        {
            Instantiate(ItemRockPrefab, pos, Quaternion.identity);
        }
    }

    public override void onUnEquip(Player player)
    {
    }

}
