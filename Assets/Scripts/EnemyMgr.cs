using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMgr : Singleton<EnemyMgr>
{
    [SerializeField] Enemy[] enemyList;
    [SerializeField] Transform[] spawnPoints;
    
    Player _player;
    public Player curPlayer
    {
        get
        {
            if (_player == null) _player = FindObjectOfType<Player>();
            return _player;
        }  
    }
    private void Start()
    {
        InvokeRepeating("SpawnEnemy", 0f, 2.0f);
    }


    //임시 소환 코드
    public void SpawnEnemy()
    {
        for (int i = 0; i < 3; i++)
        {
            Enemy spawnedEnemy = Instantiate(enemyList[Random.Range(0, enemyList.Length)], spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
            spawnedEnemy.Target = curPlayer.transform;
            spawnedEnemy.StartAI();
        }
    }
}
