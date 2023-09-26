using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMgr : MonoSingleton<GameMgr>
{
    [HideInInspector]public CameraController MainCam;
    [HideInInspector] public List<Audience> audiences;
    [SerializeField] List<Transform> audiencesPoint = new List<Transform>();
    protected void Awake()
    {
        MainCam = Camera.main.GetComponent<CameraController>();

        Pool_attackWarningLinear = new ObjectPool("Prefabs/AttackWarningLinear");
        Pool_attackWarningCircle = new ObjectPool("Prefabs/AttackWarningCircle");
    }


    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1.0f);
        StageData.Inst.selectStage();
        info = StageData.Inst.curStageInfo;
        GameLogic();
    }
    StageInfo info;


    Coroutine curSpawnRoutine;

    public void GameLogic()
    {
        spawnAudiences();

        if(curSpawnRoutine != null)StopCoroutine(curSpawnRoutine);
        curSpawnRoutine = StartCoroutine(normalEnemySpawnRoutine(info.enemy[0]));
        SoundMgr.Inst.PlayBGM("BGM");
        spawnBossEnemy();
    }


    #region ���� ����
    void spawnAudiences()
    {
        int count = StageData.Inst.audC;
        Audience audiencePrefab = Resources.Load<Audience>("Prefabs/Audience/Audience");
        for(int i = 0; i < count; i++)
        {
            Transform point = audiencesPoint[Random.Range(0, audiencesPoint.Count)];
            audiencesPoint.Remove(point);

            audiences.Add(Instantiate<Audience>(audiencePrefab, point.position, Quaternion.identity));
        }
    }

    void coinToss()
    {
        audiences[Random.Range(0, audiences.Count)].throwCoin();
    }

    public void actionOnSmash()
    {
        coinToss();
    }

    public void actionOnBossKill()
    {
        foreach(Audience aud in audiences)
        {
            aud.onEnemyKill();
        }
    }
    #endregion

    //�ӽú���
    //int dif = 4;
    //int stageCount = 0;
    #region ��ȯ����
    int curEnemyCount = 0;
    WaitForSeconds wait01 = new WaitForSeconds(1.0f);
    IEnumerator normalEnemySpawnRoutine(Enemy enem2Spwn)
    {
        while (true)
        {
            while (curEnemyCount != 0)
            {
                yield return null;
            }

            yield return wait01;
            EnemyMgr.Inst.SpawnEnemy(enem2Spwn, EnemyMgr.Inst.getRandomPos(), onNormalEnemyDie);
            curEnemyCount++;
        }
    }

    public void stopSpawningNormalEnemy()
    {
        StopCoroutine(curSpawnRoutine);

        Enemy[] spawnedEnemies = GameObject.FindObjectsOfType<Enemy>();

        for(int i = 0; i  < spawnedEnemies.Length; i++)
        {
            if(!spawnedEnemies[i].CompareTag("Boss"))
            {
                spawnedEnemies[i].Despawn();
            }
        }

    }

    void onNormalEnemyDie()
    {
        curEnemyCount--;
    }

    void spawnBossEnemy()
    {
        EnemyMgr.Inst.SpawnEnemy(info.Boss, Vector3.zero, onBossDie);
    }
    void onBossDie()
    {
        StartCoroutine(co_BossDie());

        actionOnBossKill();
    }

    IEnumerator co_BossDie()
    {
        actionOnBossKill();
        yield return new WaitForSeconds(8.0f);
        SceneManager.LoadScene("Main");
    }


#endregion
    #region ��ƿ �Լ���
    int score = 0;
    public void addScore(int score)
    {
        this.score += score;
        UIMgr.Inst.score.Set(this.score);

    }

    Coroutine curSlowtimeCoroutine;
    bool slowTimeLock;
    /// <summary>
    /// ���� �ð����� �ð� ������ �����մϴ�.
    /// </summary>
    /// <param name="time"> ������ �����ϴ� �ð� </param>
    /// <param name="amount"> �ð� ���� </param>
    public void SlowTime(float time, float amount, bool isSlowTimeLocked = false)
    {
        if (slowTimeLock) return;
        if(curSlowtimeCoroutine != null)StopCoroutine(curSlowtimeCoroutine);// ������ ����ǰ� �־��� �ð� ���� �ڷ�ƾ ����

        curSlowtimeCoroutine = StartCoroutine(co_SlowTime(time, amount));
    }

    IEnumerator co_SlowTime(float time, float amount, bool isSlowTimeLocked = false)
    {
        Time.timeScale = amount;

        slowTimeLock= isSlowTimeLocked;
        yield return new WaitForSecondsRealtime(time);

        Time.timeScale = 1;
        slowTimeLock = false;
    }


    ObjectPool Pool_attackWarningLinear;
    public GameObject AttackEffectLinear(Vector3 startPos, Vector3 endpos, float height, float time)
    {
        //TODO: ObjectPool
        GameObject temp = Pool_attackWarningLinear.Pop();
        temp.transform.position = startPos;
        temp.transform.rotation = (endpos - startPos).ToQuaternion();
        temp.GetComponent<SpriteRenderer>().size = Vector2.right * (endpos - startPos).magnitude + Vector2.up * height;

        temp.GetComponent<Poolable>().Push(time);

        return temp;
    }

   // SpriteRenderer attackWarningCircle;
    ObjectPool Pool_attackWarningCircle;
    public GameObject AttackEffectCircle(Vector3 startPos, float range, float time)
    {
        //TODO: ObjectPool
        GameObject temp = Pool_attackWarningCircle.Pop();
        temp.transform.position = startPos;
        temp.transform.localScale = Vector3.one * (range * 2);

        temp.GetComponent<Poolable>().Push(time);

        return temp;
    }

    #endregion
}
