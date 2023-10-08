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
        owner.onAttack += countAttack;
        countLeft = reqCombo;
    }

    public override void onUnEquip(Player player)
    {
        // owner.onCombo -= smash;
        owner.onAttack -= countAttack;
    }

    //적을 처치 했을 때 호출
    void countAttack()
    {
        if (curIcon != null) return;
        countLeft--;
        if (countLeft <= 0)
        {
            //다음 공격에 specialAttack을 체인함
            owner.onAttack += specialAttack;
            countLeft = reqCombo;
            //특수공격이 준비되었음을 알리는 아이콘
            curIcon = DictionaryPool.Inst.Pop("Prefabs/Effect/Icon/SwordEffect").GetComponent<Poolable>();
            owner.iconHolder.addIcon(curIcon.transform);
        }
    }

    protected virtual void specialAttack()
    {
        owner.onAttack -= specialAttack;
        owner.iconHolder.removeIcon(curIcon.transform);
        curIcon?.Push();
        curIcon = null;
    }
}
