using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyMgr : MonoSingleton<EnemyMgr>
{
    public float curDifficulty;

    int curNormalEnemyCount = 0;
    float normalEnemyMaxCount => curDifficulty;

    int curRangedEnemyCount = 0;
    float specialEnemyMaxCount => curDifficulty / 5f;
    int curSpecialEnemyCount = 0;
    float rangedEnemyMaxCount => curDifficulty / 5f;

    float totalCount => curNormalEnemyCount + curRangedEnemyCount + curSpecialEnemyCount;

    [SerializeField] List<Enemy> normalEnemy;
    [SerializeField] List<Enemy> rangedEnemy;
    [SerializeField] List<Enemy> specialEnemy;
    Player _player;

    public Transform[] spawnArea = new Transform[2];

    List<Vector2> spawnPoints;
    int spawnAreaNum = 5;
    public Player curPlayer
    {
        get
        {
            if (_player == null) _player = FindObjectOfType<Player>();
            return _player;
        }  
    }

    protected  void Awake()
    {
        spawnPoints = new List<Vector2>();
        float xDif = (spawnArea[1].transform.position.x - spawnArea[0].transform.position.x) / spawnAreaNum;
        float yDif = (spawnArea[1].transform.position.y - spawnArea[0].transform.position.y) / spawnAreaNum;


        for(int i = 0; i < spawnAreaNum; i++)
        {
            for(int j  = 0; j < spawnAreaNum; j++)
            {
                spawnPoints.Add( Vector2.right * (spawnArea[0].transform.position.x + (xDif) * i) + Vector2.up * (spawnArea[0].transform.position.y + (yDif) * j));
            }
        }

    }
    private void Start()
    {
        StartCoroutine(co_SpawnRoutine());
    }

    //TODO: 난이도에따라 시간 조절 가능하게 하기
    const float spawnTime = 10;
    IEnumerator co_SpawnRoutine()
    {
        float spawnTimeLeft = spawnTime;

        while (true)
        {
            if(spawnTimeLeft <= 0 || totalCount <= 2)
            {

                if (curRangedEnemyCount <= 1)
                {
                    spawnEnemy(true, false);
                }
                else if (curSpecialEnemyCount <= 1)
                {
                    spawnEnemy(false, true);
                }
                else
                {
                    spawnEnemy();
                }
                spawnTimeLeft = spawnTime;
            }

            spawnTimeLeft -= Time.deltaTime;
            yield return null;
        }
    }

    void spawnEnemy(bool isSpawnRanged = false, bool isSpawnSpecial = false)
    {
        List<Enemy> total = new List<Enemy>();

        float spawnDifficulty = curDifficulty;
        Enemy temp;

        if (isSpawnRanged)
        {
            for (int i = curRangedEnemyCount; i < rangedEnemyMaxCount; i++)
            {
                temp = getRanged();
                spawnDifficulty -= temp.difficulty;
                total.Add(temp);
            }
        }
        if (isSpawnSpecial)
        {
            for (int i = curSpecialEnemyCount; i < specialEnemyMaxCount; i++)
            {
                temp = getSpecial();
                spawnDifficulty -= temp.difficulty;
                total.Add(temp);
            }
        }
        for(int i = curNormalEnemyCount; i < spawnDifficulty; i++) total.Add(getNormal());  // 남은 difficulty만큼 일반 적 소환

        List<Vector2> spawnList = spawnPoints.ToList();
        
        for(int i = 0; i < spawnList.Count; i++)
        {
            //플레이어에서 가까운 위치의 소환좌표들을 제외시켜줌
            if (Vector2.Distance(spawnList[i], curPlayer.transform.position) <= 6) spawnList.Remove(spawnList[i]);
        }
        
        for (int i = 0; i < total.Count; i++)
        {
            if (spawnList.Count == 0) break;            //몬스터가 소환될 위치가 더이상 없다면, 소환 중지.
            Vector2 tempVec = spawnList[Random.Range(0, spawnList.Count)];
            spawnList.Remove(tempVec);
            StartCoroutine(co_SpawnEnemy(total[i], tempVec));
        }
    }


    Enemy getNormal()
    {
        curNormalEnemyCount++;
        return normalEnemy[Random.Range(0, normalEnemy.Count)];
    }
    Enemy getSpecial()
    {
        curSpecialEnemyCount++;
        return specialEnemy[Random.Range(0, specialEnemy.Count)];
    }
    Enemy getRanged()
    {
        curRangedEnemyCount++;
        return rangedEnemy[Random.Range(0, rangedEnemy.Count)];
    }

    IEnumerator co_SpawnEnemy(Enemy enemyPrefab, Vector3 position)
    {
        GameObject warning = DictionaryPool.Inst.Pop("Prefabs/Warning");
        warning.transform.position = position;

        yield return new WaitForSeconds(1.0f);
        warning.GetComponent<Poolable>().Push();

        GameObject smoke = DictionaryPool.Inst.Pop("Prefabs/Smoke");
        smoke.transform.position = position;
        smoke.GetComponent<Poolable>().Push(2.0f);

        Enemy spawnedEnemy = Instantiate<Enemy>(enemyPrefab, position, Quaternion.identity);
        spawnedEnemy.Target = curPlayer;
        spawnedEnemy.StartAI();
        switch(spawnedEnemy.type)
        {
            case ENEMYTYPE.NORMAL:
                spawnedEnemy.onDeath += () => { curNormalEnemyCount--; };
                break;
            case ENEMYTYPE.RANGED:
                spawnedEnemy.onDeath += () => { curRangedEnemyCount--; };
                break;
            case ENEMYTYPE.SPECIAL:
                spawnedEnemy.onDeath += () => { curSpecialEnemyCount--; };
                break;
        }
    }
}
