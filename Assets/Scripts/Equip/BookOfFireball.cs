using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookOfFireball : Equip
{
    public GameObject MagicCircle;
    GameObject curMagicCircle;

    public override void onEquip(Player player)
    {
        base.onEquip(player);
       curMagicCircle =  Instantiate(MagicCircle, Vector3.up * (-1.5f), Quaternion.identity);
    }

    public override void onUnEquip(Player player)
    {
        Destroy(curMagicCircle);
    }


}
