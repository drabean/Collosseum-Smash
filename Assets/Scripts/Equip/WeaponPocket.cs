using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPocket : Equip
{
    public float spawnWaitTIme = 6.0f;

    Item daggerPrefab;
    Item curDagger;

    Transform[] spawnArea;

    public override void onEquip(Player player)
    {
        daggerPrefab = Resources.Load<Item>("Prefabs/Item/ItemDagger");
         spawnArea = EnemyMgr.Inst.spawnArea;

        spawnNewDagger();
    }
    public override void onUnEquip(Player player)
    {
        Destroy(curDagger);
    }


    void spawnNewDagger()
    {
        curDagger = Instantiate(daggerPrefab, Vector2.right * Random.Range(spawnArea[0].position.x, spawnArea[1].position.x) + Vector2.up * Random.Range(spawnArea[0].position.y, spawnArea[1].position.y), Quaternion.identity);
        curDagger.onAcquire += spawnNewDagger;
    }
}
