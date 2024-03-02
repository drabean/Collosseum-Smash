using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//아이템 선택 풀을 관리하는 스크립트
public class ItemMgr : MonoSingleton<ItemMgr>
{
    /// <summary>
    /// 현재 모든 장비들
    /// </summary>
    List<Equip> normalPool;
    List<Equip> potionPool;


    /// <summary>
    /// 일반 아이템 풀 초기화
    /// </summary>
    /// <param name="data"></param>
    public void InitNormalEquipPool(RunData data)
    {
        normalPool = new List<Equip>();
        normalPool.AddRange(Resources.Load<EquipInfo>("Datas/EquipInfo/EquipNormal").list); // 기본 아이템
        if(LoadedSave.Inst.save.Exp > 5)
            normalPool.AddRange(Resources.Load<EquipInfo>("Datas/EquipInfo/Unlock1").list); // 1차 해금 - 투척 관련 템들
        if (LoadedSave.Inst.save.Exp > 10)
            normalPool.AddRange(Resources.Load<EquipInfo>("Datas/EquipInfo/Unlock2").list); // 2차 해금 - 부작용 있는 아이템
        //이미 획득한 아이템은 풀에서 빼주기
        foreach (int ItemsGot in data.item)
        {
            if(normalPool.Contains(LoadedData.Inst.getEquipByID(ItemsGot)))
            {
                normalPool.Remove(LoadedData.Inst.getEquipByID(ItemsGot));
            }
        }
       //TODO: 업적 확인하여 해금 아이템 추가
       //TODO: 스테이지 별 전용 아이템 추가
    }
    /// <summary>
    /// 플레이어의 최종 스탯을 기반으로 포션 아이템 풀 초기화
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
    /// 풀에서 랜덤한 일반 장비 반환
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
    /// 풀에서 랜덤한 포션 반환
    /// </summary>
    /// <returns></returns>
    public Equip GetPotionEquip()
    {
        if (potionPool.Count == 0) return GetNormalEquip(); // 플레이어의 능력치가 모두 최대치라면, 일반 장비를 반환.

        Equip equip = potionPool[Random.Range(0, potionPool.Count)];
        potionPool.Remove(equip);

        return equip;
    }

}
