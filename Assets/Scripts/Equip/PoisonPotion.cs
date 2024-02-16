using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonPotion : Equip
{
    GameObject curItemPoisonPotion;
    public GameObject ItemPoisonPotionPrefab;
    Coroutine curSpawnCoroutine;

    public int maxUse;

    int curUse = 0;
    public override void onEquip(Player player)
    {
        base.onEquip(player);
        curItemPoisonPotion = Instantiate(ItemPoisonPotionPrefab, EnemyMgr.Inst.getRandomPos(), Quaternion.identity);
        curSpawnCoroutine = StartCoroutine(co_SpawnCoroutine());
    }

    public override void onUnEquip(Player player)
    {
        Destroy(curItemPoisonPotion);
        StopCoroutine(curSpawnCoroutine);
    }

    IEnumerator co_SpawnCoroutine()
    {
        while(true)
        {
            if (curUse >= maxUse) yield break;

            if (curItemPoisonPotion != null)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(9.0f);
                curItemPoisonPotion = Instantiate(ItemPoisonPotionPrefab, EnemyMgr.Inst.getRandomPos(), Quaternion.identity);
                curUse++;
            }

        }
    }
}
