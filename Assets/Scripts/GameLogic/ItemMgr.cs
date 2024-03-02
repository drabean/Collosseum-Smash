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
        if(LoadedSave.Inst.save.Exp > 5)
            normalPool.AddRange(Resources.Load<EquipInfo>("Datas/EquipInfo/Unlock1").list); // 1�� �ر� - ��ô ���� �۵�
        if (LoadedSave.Inst.save.Exp > 10)
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
    /// �÷��̾��� ���� ������ ������� ���� ������ Ǯ �ʱ�ȭ
    /// </summary>
    /// <param name="player"></param>
    public void InitPotionEquipPool(Player player)
    {
        EquipInfo potionItems = Resources.Load<EquipInfo>("Datas/EquipInfo/EquipPotion");
        potionPool = new List<Equip>();
        if (player.Stat.STR <= 5) potionPool.Add(potionItems.list[0]);
        if (player.Stat.SPD <= 5) potionPool.Add(potionItems.list[1]);
        if (player.Stat.VIT <= 5) potionPool.Add(potionItems.list[2]);

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
