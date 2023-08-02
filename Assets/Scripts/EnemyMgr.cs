using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMgr : Singleton<EnemyMgr>
{
    [SerializeField] Enemy[] enemyList;

    GameObject warning;
    GameObject smoke;

    Player _player;

    public Transform[] spawnArea = new Transform[2];
    public Player curPlayer
    {
        get
        {
            if (_player == null) _player = FindObjectOfType<Player>();
            return _player;
        }  
    }

    protected override void Awake()
    {
        base.Awake();
        warning = Resources.Load<GameObject>("Prefabs/Warning");
        smoke = Resources.Load<GameObject>("Prefabs/Smoke");

    }
    private void Start()
    {
        InvokeRepeating("SpawnEnemy", 0f, 2.0f);
    }


    //임시 소환 코드
    public void SpawnEnemy()
    {
        float difficultyTotal = 0;

        while(difficultyTotal < 5)
        {
            Enemy enemyPrefab = enemyList[Random.Range(0, enemyList.Length)];
            Vector3 position = Vector3.right * (Random.Range(spawnArea[0].position.x, spawnArea[1].position.x)) + Vector3.up * (Random.Range(spawnArea[0].position.y, spawnArea[1].position.y));

            StartCoroutine(co_SpawnEnemy(enemyPrefab, position));
            difficultyTotal += enemyPrefab.difficulty;
        }    
    }

    IEnumerator co_SpawnEnemy(Enemy enemyPrefab, Vector3 position)
    {
        GameObject warning = Instantiate(this.warning, position, Quaternion.identity);

        yield return new WaitForSeconds(1.0f);
        Destroy(warning);
        Instantiate(smoke, position, Quaternion.identity);
        Enemy spawnedEnemy = Instantiate<Enemy>(enemyPrefab, position, Quaternion.identity);
        spawnedEnemy.Target = curPlayer.transform;
        spawnedEnemy.StartAI();
    }
}
