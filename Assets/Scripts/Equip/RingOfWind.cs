using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingOfWind : Equip
{
    public float cooltime;

    float lastAttackTime = 0;

    bool isBufOn;
    public int bufAmount = 3;
    public Poolable curIcon;

    public override void onEquip(Player player)
    {
        base.onEquip(player);
        owner.onAttack+= onAttack;
    }

    public override void onUnEquip(Player player)
    {
        owner.onAttack -= onAttack;
    }
    void onAttack()
    {
        lastAttackTime = Time.time;
        if (isBufOn)
        {
            isBufOn = false;
            owner.Stat.SPD -= bufAmount;
            owner.SetStatus();

            owner.iconHolder.removeIcon(curIcon.transform);
            curIcon?.Push();
            curIcon = null;
        }


    }

    private void Update()
    {
        if (isBufOn) return;
        if (Time.time - lastAttackTime > cooltime)
        {
            isBufOn = true;
            owner.Stat.SPD += bufAmount;
            owner.SetStatus();
            curIcon = DictionaryPool.Inst.Pop("Prefabs/Effect/Icon/SpeedEffect").GetComponent<Poolable>();
            SoundMgr.Inst.Play("BufOn");
            owner.iconHolder.addIcon(curIcon.transform);
        }

    }
}
