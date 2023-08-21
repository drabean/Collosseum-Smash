using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderStep : Equip
{
    public float cooltime;
    public float lastEffectedTime;

    public override void onEquip(Player player)
    {
        owner = player;
        owner.onMovement += thunderStep;

    }

    public override void onUnEquip(Player player)
    {
        owner.onMovement -= thunderStep;
    }

    void thunderStep()
    {
        if(Time.time - lastEffectedTime >= cooltime)
        {
            lastEffectedTime = Time.time;

           DictionaryPool.Inst.Pop("Prefabs/Effect/Thunder").transform.position = owner.transform.position;
        }
    }
}
