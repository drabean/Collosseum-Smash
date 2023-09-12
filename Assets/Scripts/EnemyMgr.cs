using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class EnemyMgr : MonoSingleton<EnemyMgr>
{
    public StageInfo info;

    /// <summary>
    /// 스테이지의 범위. 0: min, 1: max
    /// </summary>
    public Transform[] spawnArea = new Transform[2];

    List<Vector2> spawnPoints  = new List<Vector2>();
    List<Vector2> cornerPoints = new List<Vector2>();

    int spawnAreaNum = 8;

    public Player player;
    #region 외부 접근용 유틸 함수
    public Vector2 getRandomPos() { return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)]; }
    public List<Vector2> getCornerPos() { return cornerPoints; }
    #endregion

    protected void Awake()
    {
        //스테이지 범위와 spawnAreaNum을 기반으로, 적이 소환될 수 있는 위치들을 계산하여 초기화 해줌.
        float xDif = (spawnArea[1].transform.position.x - spawnArea[0].transform.position.x) / spawnAreaNum;
        float yDif = (spawnArea[1].transform.position.y - spawnArea[0].transform.position.y) / spawnAreaNum;

        for (int i = 0; i <= spawnAreaNum; i++)
        {
            for (int j = 0; j <= spawnAreaNum; j++)
            {
                int cornerCount = 0;

                Vector2 point = Vector2.right * (spawnArea[0].transform.position.x + (xDif) * i) + Vector2.up * (spawnArea[0].transform.position.y + (yDif) * j);

                if (i == 1 || i == spawnAreaNum-1) cornerCount++;
                if (j == 1 || j == spawnAreaNum-1) cornerCount++;

                if (cornerCount == 1) spawnPoints.Add(point);
                if (cornerCount == 2) cornerPoints.Add(point);
            }
        }

    }

    private IEnumerator Start()
    {
        yield return null;
    }


    public List<Vector2> GetSpawnPoints(int num)
    {
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < num + 2; i++)
        {
            if (i < 4) points.Add(cornerPoints[i]);
            else points.Add(spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)]);
        }
        return points;
    }
    /// <summary>
    /// 적을 소환함
    /// </summary>
    /// <param name="enemyPrefab">소환할 적 프리팹</param>
    /// <param name="position">소환할 위치</param>
    /// <param name="deadOption">사망시 호출될 추가 함수 (옵션)</param>
    public void SpawnEnemy(Enemy enemyPrefab, Vector3 position, Action deadOption = null)
    {
        StartCoroutine(co_SpawnEnemy(enemyPrefab, position, deadOption));
    }

    /// <summary>
    /// 적 소환 코루틴
    /// </summary>
    /// <param name="enemyPrefab"></param>
    /// <param name="position"></param>
    /// <param name="deadOption"></param>
    /// <returns></returns>
    IEnumerator co_SpawnEnemy(Enemy enemyPrefab, Vector3 position, Action deadOption = null)
    {
        GameMgr.Inst.AttackEffectCircle(position, 1.0f, 1.0f);
        Poolable warning = DictionaryPool.Inst.Pop("Prefabs/Warning").GetComponent<Poolable>();
        warning.transform.position = position;
        warning.Push(1.0f);
        yield return new WaitForSeconds(1.0f);

        GameObject smoke = DictionaryPool.Inst.Pop("Prefabs/Smoke");
        smoke.transform.position = position;
        smoke.GetComponent<Poolable>().Push(2.0f);

        Enemy spawnedEnemy = Instantiate<Enemy>(enemyPrefab, position, Quaternion.identity);
        spawnedEnemy.Target = player;
        spawnedEnemy.StartAI();

        if(deadOption != null) spawnedEnemy.onDeath += deadOption;
    }

}

