using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//������ ���� Ǯ�� �����ϴ� ��ũ��Ʈ
public class ItemMgr : MonoSingleton<ItemMgr>
{
    /// <summary>
    /// ���� ��� ����
    /// </summary>
    List<Equip> normalPool;
    List<Equip> potionPool;


    /// <summary>
    /// �Ϲ� ������ Ǯ �ʱ�ȭ
    /// </summary>
    /// <param name="data"></param>
    public void InitNormalEquipPool(RunData data)
    {
        normalPool = new List<Equip>();
        normalPool.AddRange(Resources.Load<EquipInfo>("Datas/EquipInfo/EquipNormal").list); // �⺻ ������
        if(LoadedSave.Inst.save.BossKill > 10)
            normalPool.AddRange(Resources.Load<EquipInfo>("Datas/EquipInfo/Unlock1").list); // 1�� �ر� - ��ô ���� �۵�
        if (LoadedSave.Inst.save.NormalKill > 100)
            normalPool.AddRange(Resources.Load<EquipInfo>("Datas/EquipInfo/Unlock2").list); // 2�� �ر� - ���ۿ� �ִ� ������
        //�̹� ȹ���� �������� Ǯ���� ���ֱ�
        foreach (int ItemsGot in data.item)
        {
            if(normalPool.Contains(LoadedData.Inst.getEquipByID(ItemsGot)))
            {
                normalPool.Remove(LoadedData.Inst.getEquipByID(ItemsGot));
            }
        }
       //TODO: ���� Ȯ���Ͽ� �ر� ������ �߰�
       //TODO: �������� �� ���� ������ �߰�
    }
    /// <summary>
    /// �⺻ �ɷ�ġ �����۵�
    /// </summary>
    /// <param name="player"></param>
    public void InitPotionEquipPool(RunData data)
    {
        potionPool = new List<Equip>();
        potionPool.AddRange(Resources.Load<EquipInfo>("Datas/EquipInfo/EquipPotion").list);

        foreach (int ItemsGot in data.item)
        {
            if (potionPool.Contains(LoadedData.Inst.getEquipByID(ItemsGot)))
            {
                potionPool.Remove(LoadedData.Inst.getEquipByID(ItemsGot));
            }
        }
    }

    /// <summary>
    /// Ǯ���� ������ �Ϲ� ��� ��ȯ
    /// </summary>
    /// <returns></returns>
    public Equip GetNormalEquip()
    {
        if (normalPool.Count == 0) return null;

        Equip equip = normalPool[Random.Range(0, normalPool.Count)];
        normalPool.Remove(equip);

        return equip;
    }
    /// <summary>
    /// Ǯ���� ������ ���� ��ȯ
    /// </summary>
    /// <returns></returns>
    public Equip GetPotionEquip()
    {
        if (potionPool.Count == 0) return GetNormalEquip(); // �÷��̾��� �ɷ�ġ�� ��� �ִ�ġ���, �Ϲ� ��� ��ȯ.

        Equip equip = potionPool[Random.Range(0, potionPool.Count)];
        potionPool.Remove(equip);

        return equip;
    }

}
