using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerPouch : Equip
{
    public float spawnAreaSize = 3f;

    public int DaggerSpawnCount = 4;
    int daggerSpawnCountLeft = 0;

    public Item daggerPrefab;

    Transform[] spawnArea;
    public override void onEquip(Player player)
    {
        base.onEquip(player);
        
        daggerSpawnCountLeft = DaggerSpawnCount;
        spawnArea = EnemyMgr.Inst.spawnArea;
        player.onAttack += spawnThrowingKnife;
    }
    public override void onUnEquip(Player player)
    {
        player.onAttack -= spawnThrowingKnife;
    }

    void spawnThrowingKnife()
    {
        daggerSpawnCountLeft--;

        if(daggerSpawnCountLeft <= 0)
        {
            Vector3 spawnPos = transform.position + Vector3.right * Random.Range(0, spawnAreaSize) + Vector3.up * Random.Range(0, spawnAreaSize);

            Instantiate(daggerPrefab, spawnPos.Clamp(spawnArea[0].position, spawnArea[1].position), Quaternion.identity);
            daggerSpawnCountLeft = DaggerSpawnCount;
        }
    }
}
