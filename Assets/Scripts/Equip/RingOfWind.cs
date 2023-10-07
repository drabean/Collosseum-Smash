using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingOfWind : Equip
{
    public float cooltime;
    public float lastEffectedTime;
    public Poolable curIcon;

    public override void onEquip(Player player)
    {
        base.onEquip(player);
        owner.onMovement += windBuf;
        owner.onMovementStop += windBufStop;

    }

    public override void onUnEquip(Player player)
    {
        owner.onMovement -= windBuf;
        owner.onMovementStop -= windBufStop;
    }

    bool isBufOn;

    void windBuf()
    {
        if (isBufOn) return;

        //���������� Ȱ��ȭ ó��
        if (curIcon == null)
        {
            curIcon = DictionaryPool.Inst.Pop("Prefabs/Effect/Icon/SpeedEffect").GetComponent<Poolable>();
            owner.iconHolder.addIcon(curIcon.transform);
        }

        owner.Stat.SPD+=1;
        owner.SetStatus();
        isBufOn = true;
    }

    void windBufStop()
    {
        if (!isBufOn) return;

        //���������� ��Ȱ��ȭ ó��
        owner.iconHolder.removeIcon(curIcon.transform);
        curIcon.Push();
        curIcon = null;


        owner.Stat.SPD-=1;
        owner.SetStatus();
        isBufOn = false;
    }
}
