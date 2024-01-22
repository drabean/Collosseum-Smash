using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSwordSheath : Equip
{
    public float cooltime;

    public float lastAttackTime;

    bool isBufOn;
    public Poolable curIcon;

    public override void onEquip(Player player)
    {
        base.onEquip(player);
        owner.onAttack += onAttack;
    }
    public override void onUnEquip(Player player)
    {
        owner.onAttack -= onAttack;
    }

    void onAttack()
    {
        lastAttackTime = Time.time;
        if(isBufOn)
        {
            isBufOn = false;
            owner.Stat.STR -= 3;

            owner.iconHolder.removeIcon(curIcon.transform);
            curIcon?.Push();
            curIcon = null;
        }


    }
    private void Update()
    {
        if (isBufOn) return;
        if(Time.time - lastAttackTime > cooltime)
        {
            isBufOn = true;
            owner.Stat.STR+=3;
            curIcon = DictionaryPool.Inst.Pop("Prefabs/Effect/Icon/SwordEffect").GetComponent<Poolable>();
            SoundMgr.Inst.Play("BufOn");
            owner.iconHolder.addIcon(curIcon.transform);
        }

    }

}
