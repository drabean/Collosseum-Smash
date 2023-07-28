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


    //�ӽ� ��ȯ �ڵ�
    public void SpawnEnemy()
    {
        float difficultyTotal = 0;

        while(difficultyTotal < 3)
        {
            Enemy spawnedEnemy = Instantiate(enemyList[Random.Range(0, enemyList.Length)], spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
            spawnedEnemy.Target = curPlayer.transform;
            spawnedEnemy.StartAI();
            difficultyTotal += spawnedEnemy.difficulty;
        }    
    }
}
