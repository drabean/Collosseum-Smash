using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//무작위 생성 아이템 풀을 관리하는 스크립트입니다.
public class ItemMgr : MonoSingleton<ItemMgr>
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        //TODO: 업적 확인하여 아이템을 풀에 추가하기
    }

    /// <summary>
    /// 현재 모든 장비들
    /// </summary>
    public List<Equip> equipPool; 

    public Equip getRandomEquip()
    {
        if (equipPool.Count == 0) return null;

        Equip equip = equipPool[Random.Range(0, equipPool.Count)];
        equipPool.Remove(equip);

        return equip;
    }

}
