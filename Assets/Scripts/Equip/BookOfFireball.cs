using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookOfFireball : Equip
{
    GameObject curItemFireball;
    public GameObject ItemFireballPrefab;
    Coroutine curSpawnCoroutine;

    public override void onEquip(Player player)
    {
        base.onEquip(player);
        curItemFireball = Instantiate(ItemFireballPrefab, EnemyMgr.Inst.getRandomPos(), Quaternion.identity);
        curSpawnCoroutine = StartCoroutine(co_SpawnCoroutine());
    }

    public override void onUnEquip(Player player)
    {
        Destroy(curItemFireball);
        StopCoroutine(curSpawnCoroutine);
    }

    IEnumerator co_SpawnCoroutine()
    {
        while (true)
        {
            if (curItemFireball != null)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(9.0f);
                curItemFireball = Instantiate(curItemFireball, EnemyMgr.Inst.getRandomPos(), Quaternion.identity);
            }

        }
    }
}
