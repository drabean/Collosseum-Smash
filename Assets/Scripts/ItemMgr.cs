using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMgr : MonoSingleton<ItemMgr>
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
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
