using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipWeapon : Equip
{
    public int reqCombo;
    int countLeft;
    public Poolable curIcon;

    public override void onEquip(Player player)
    {
        base.onEquip(player);
        //owner.onCombo += smash;
        owner.actionSmash += countAttack;
        countLeft = reqCombo;
    }

    public override void onUnEquip(Player player)
    {
        // owner.onCombo -= smash;
        owner.actionSmash -= countAttack;
    }
    void countAttack()
    {
        if (curIcon != null) return;
        countLeft--;

        if (countLeft <= 0)
        {
            owner.onAttack += specialAttack;
            countLeft = reqCombo;
            curIcon = DictionaryPool.Inst.Pop("Prefabs/Effect/Icon/SwordEffect").GetComponent<Poolable>();
            owner.iconHolder.addIcon(curIcon.transform);

        }
    }

    protected virtual void specialAttack()
    {
        owner.onAttack -= specialAttack;
        owner.iconHolder.removeIcon(curIcon.transform);
        curIcon?.Push();
    }
}
