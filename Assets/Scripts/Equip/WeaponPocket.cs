using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPocket : Equip
{
    public float spawnWaitTIme = 6.0f;

    public override void onEquip(Player player)
    {
        StartCoroutine(co_SpawnRoutine());
    }
    public override void onUnEquip(Player player)
    {
        StopCoroutine(co_SpawnRoutine());
    }

    IEnumerator co_SpawnRoutine()
    {
        WaitForSeconds waitForSpawn = new WaitForSeconds(spawnWaitTIme);
        GameObject Dagger = Resources.Load<GameObject>("Prefabs/Item/ItemDagger");
        Transform[] spawnArea =  EnemyMgr.Inst.spawnArea;
        while(true)
        {
            yield return waitForSpawn;
            Instantiate(Dagger, Vector2.right * Random.Range(spawnArea[0].position.x, spawnArea[1].position.x) + Vector2.up * Random.Range(spawnArea[0].position.y, spawnArea[1].position.y), Quaternion.identity);
        }
    }
}
