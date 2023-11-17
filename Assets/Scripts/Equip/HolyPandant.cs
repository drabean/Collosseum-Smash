using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyPandant : Equip
{
    public float Cooltime;
    float cooltimeLeft = 0;
    public Poolable curIcon;

    public override void onEquip(Player player)
    {
        base.onEquip(player);
        player.actionHit += blessing;

    }
    public override void onUnEquip(Player player)
    {
    }

    bool blessing(bool resisted)
    {
        if (resisted) return true;  //���� ü�ο��� �̹� ���׿� ���������Ƿ�, �׳� ���


        if (cooltimeLeft > 0) return false; // ��Ÿ�� ���� �ȵ����Ƿ� ���� ����

        owner.iconHolder.removeIcon(curIcon.transform);
        curIcon.Push();
        curIcon = null;

        cooltimeLeft = Cooltime;

        DictionaryPool.Inst.Pop("Prefabs/Effect/HolyShield").transform.position = owner.transform.position;
        return true;

    }

    private void Update()
    {
        if (owner == null) return;

        cooltimeLeft -= Time.deltaTime;

        if (cooltimeLeft <= 0)
        {
            if (curIcon == null)
            {
                curIcon = DictionaryPool.Inst.Pop("Prefabs/Effect/Icon/ShieldEffect").GetComponent<Poolable>();
                owner.iconHolder.addIcon(curIcon.transform);
            }
        }
    }
}
