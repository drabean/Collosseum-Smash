using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyMgr : MonoSingleton<EnemyMgr>
{
    public float curDifficulty;

    float specialEnemyMaxCount => curDifficulty / 10f;
    float rangedEnemyMaxCount => curDifficulty / 10f;

    int totalCount = 0;

    [SerializeField] List<Enemy> normalEnemy;
    [SerializeField] List<Enemy> rangedEnemy;
    [SerializeField] List<Enemy> specialEnemy;

    public Transform[] spawnArea = new Transform[2];

    List<Vector2> spawnPoints;
    int spawnAreaNum = 8;

    public Player player;

    protected void Awake()
    {
        spawnPoints = new List<Vector2>();
        float xDif = (spawnArea[1].transform.position.x - spawnArea[0].transform.position.x) / spawnAreaNum;
        float yDif = (spawnArea[1].transform.position.y - spawnArea[0].transform.position.y) / spawnAreaNum;


        for (int i = 0; i <= spawnAreaNum; i++)
        {
            for (int j = 0; j <= spawnAreaNum; j++)
            {
                spawnPoints.Add(Vector2.right * (spawnArea[0].transform.position.x + (xDif) * i) + Vector2.up * (spawnArea[0].transform.position.y + (yDif) * j));
            }
        }

    }
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(co_SpawnRoutine());
    }

    IEnumerator co_SpawnRoutine()
    {
        while (true)
        {
            if (totalCount <= 0)
            {
                curDifficulty += 4;
                yield return StartCoroutine(spawnEnemy(true, true));      
            }
            yield return null;
        }
    }

    IEnumerator spawnEnemy(bool isSpawnRanged = false, bool isSpawnSpecial = false)
    {
        //이 루틴에서 소환할 모든 적을 담는 리스트
        List<Enemy> total = new List<Enemy>();

        float spawnDifficulty = curDifficulty;
        Enemy temp;

        UIMgr.Inst.progress.showDifficulty((int)spawnDifficulty);
        if (isSpawnRanged)
        {
            for (int i = totalCount; i < rangedEnemyMaxCount; i++)
            {
                temp = getRanged();
                spawnDifficulty -= temp.difficulty;
                total.Add(temp);
            }
        }
        if (isSpawnSpecial)
        {
            for (int i = totalCount; i < specialEnemyMaxCount; i++)
            {
                temp = getSpecial();
                spawnDifficulty -= temp.difficulty;
                total.Add(temp);
            }
        }
        for (int i = totalCount; i < spawnDifficulty; i++) total.Add(getNormal());  // 남은 Count만큼 일반 적 소환

        List<Vector2> spawnList = spawnPoints.ToList();

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            //플레이어에서 가까운 위치의 소환좌표들을 제외시켜줌
            if (Vector2.Distance(spawnPoints[i], player.transform.position) <= 6)
            {
                spawnList.Remove(spawnPoints[i]);
            }
        }

        yield return new WaitForSeconds(1.0f);
        for (int i = 0; i < total.Count; i++)
        {
            if (spawnList.Count == 0) break;            //몬스터가 소환될 위치가 더이상 없다면, 소환 중지.
            totalCount++;
            Vector2 tempVec = spawnList[Random.Range(0, spawnList.Count)];
            spawnList.Remove(tempVec);
            StartCoroutine(co_SpawnEnemy(total[i], tempVec));
        }
    }


    Enemy getNormal()
    {
        return normalEnemy[Random.Range(0, normalEnemy.Count)];
    }
    Enemy getSpecial()
    {
        return specialEnemy[Random.Range(0, specialEnemy.Count)];
    }
    Enemy getRanged()
    {
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
        spawnedEnemy.Target = player;
        spawnedEnemy.StartAI();

        spawnedEnemy.onDeath += () => onEnemyDie();

        
    }

    #region 난이도 관련

    void onEnemyDie()
    {
        totalCount--;
    }

    #endregion
}

