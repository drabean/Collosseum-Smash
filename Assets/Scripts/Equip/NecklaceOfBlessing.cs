using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecklaceOfBlessing : Equip
{
    float lastTIme = -20;

    public override void onEquip(Player player)
    {
        base.onEquip(player);
        player.onHit += blessing;

    }
    public override void onUnEquip(Player player)
    {
       
    }

    bool blessing(bool resisted)
    {
        if (resisted) return true;  //이전 체인에서 이미 저항에 성공했으므로, 그냥 통과

        if(Time.time - lastTIme >= 10.0f)
        {
            lastTIme = Time.time;

            DictionaryPool.Inst.Pop("Prefabs/Effect/HolyShield").transform.position = owner.transform.position;
            return true;
        }
        else
        {
            return false;
        }
    }
}
