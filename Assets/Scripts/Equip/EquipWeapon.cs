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

    //���� óġ ���� �� ȣ��
    void countAttack()
    {
        if (curIcon != null) return;
        countLeft--;
        if (countLeft <= 0)
        {
            //���� ���ݿ� specialAttack�� ü����
            owner.onAttack += specialAttack;
            countLeft = reqCombo;
            //Ư�������� �غ�Ǿ����� �˸��� ������
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
