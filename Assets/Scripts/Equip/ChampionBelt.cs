using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionBelt : Equip
{
    public Poolable curIcon;

    public override void onEquip(Player player)
    {
        base.onEquip(player);
        owner.Stat.STR++;
        owner.Stat.SPD++;
        owner.SetStatus();
        owner.resistenceChain.AddResistanceEffect(onHit);
    }
    public override void onUnEquip(Player player)
    {
        owner.Stat.STR--;
        owner.Stat.SPD--;
        owner.SetStatus();
        owner.resistenceChain.RemoveResistanceEffect(onHit);
    }

    public bool onHit(bool resisted)
    {
        debufTimeLeft = DebufTime;

        if (isDebufOn) return resisted;

        isDebufOn = true;
        debufOn();
        
        return resisted;
    }
    public float DebufTime;
    float debufTimeLeft = 0;
    bool isDebufOn = false;

    private void Update()
    {
        if (!isDebufOn) return;
        debufTimeLeft -= Time.deltaTime;

        if(debufTimeLeft < 0)
        {
            isDebufOn = false;
            debufOff();
        }

    }
    void debufOn()
    {
        owner.Stat.STR--;
        owner.Stat.SPD--;
        owner.SetStatus();
        curIcon = DictionaryPool.Inst.Pop("Prefabs/Effect/Icon/StunEffect").GetComponent<Poolable>();
        owner.iconHolder.addIcon(curIcon.transform);
    }

    void debufOff()
    {
        owner.Stat.STR++;
        owner.Stat.SPD++;
        owner.SetStatus();
        curIcon?.Push();
        owner.iconHolder.removeIcon(curIcon.transform);
        curIcon = null;
    }
}
