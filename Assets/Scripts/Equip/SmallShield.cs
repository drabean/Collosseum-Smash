using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallShield : Equip
{
    public int maxKillCount = 5;
    bool isShieldActive;
    public Poolable curIcon;

    public override void onEquip(Player player)
    {
        base.onEquip(player);
        owner.actionSmash += checkKillCount;

    }
    public override void onUnEquip(Player player)
    {
        owner.actionSmash -= checkKillCount;
    }

    int killCount;

    void checkKillCount()
    {
        if (isShieldActive) return;

        killCount++;
        if(killCount == maxKillCount)
        {
            owner.actionHit += Block;
            curIcon = DictionaryPool.Inst.Pop("Prefabs/Effect/Icon/ShieldEffect").GetComponent<Poolable>();
            owner.iconHolder.addIcon(curIcon.transform);

            isShieldActive = true;
        }
    }


    bool Block(bool resisted)
    {
        if (resisted) return true;  //이전 체인에서 이미 저항에 성공했으므로, 그냥 통과


        owner.iconHolder.removeIcon(curIcon.transform);
        curIcon.Push();
        curIcon = null;
        isShieldActive = false;

        owner.actionHit -= Block;
        DictionaryPool.Inst.Pop("Prefabs/Effect/BlockEffect").transform.position = owner.transform.position;

        return true;

    }
}
