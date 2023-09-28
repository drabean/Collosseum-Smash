using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMgr : MonoSingleton<GameMgr>
{
    [HideInInspector] public CameraController MainCam;
    //   [SerializeField] List<Transform> audiencesPoint = new List<Transform>();
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
        StartNormalStage();
    }
    StageInfo info;
    Coroutine curSpawnRoutine;
    int maxCount = 3; // ��ȯ �� �� �ִ� �ִ� ������
    int progressCount = 0; //
    public void StartNormalStage()
    {
        Init();

        if (curSpawnRoutine != null) StopCoroutine(curSpawnRoutine);
        UIMgr.Inst.progress.ShowNormalUI();
        curSpawnRoutine = StartCoroutine(normalEnemySpawnRoutine());
        SoundMgr.Inst.PlayBGM("BGM");
    }
    void startBossStage()
    {

        GameMgr.Inst.removeAllNormalEnemies();
        UIMgr.Inst.progress.ShowBossUI();
        EnemyMgr.Inst.SpawnBossEnemy(info.Boss, Vector3.zero, onBossDie);

    }
    List<int> curSpawnedEnemies; // ���� ��ȯ �� �� �ִ� ������ index

    void Init()
    {
        curSpawnedEnemies = new List<int>();
        for (int i = 1; i < info.Enemies.Count; i++)
        {
            curSpawnedEnemies.Add(i);
        }
        Debug.Log(curSpawnedEnemies.Count);
    }


    //�ӽú���
    //int dif = 4;
    //int stageCount = 0;
    #region ��ȯ����

    /// <summary>
    /// ���� ��ȯ ������ ���� index�� ��ȯ
    /// </summary>
    /// <returns></returns>
    int getIndexToSpawn()
    {
        if (curSpawnedEnemies.Count == 0) return 0;
        else
        {
            int idx2Spwn = curSpawnedEnemies[Random.Range(0, curSpawnedEnemies.Count)];
            curSpawnedEnemies.Remove(idx2Spwn);
            return idx2Spwn;
        }
    }
    int curEnemyCount = 0;
    WaitForSeconds waitForEnemySpawn = new WaitForSeconds(1.5f);
    IEnumerator normalEnemySpawnRoutine()
    {
        while (true)
        {
            while (curEnemyCount >= maxCount)
            {
                yield return null;
            }

            yield return waitForEnemySpawn;

            int idx2Spwn = getIndexToSpawn();
            EnemyMgr.Inst.SpawnEnemy(info.Enemies[idx2Spwn], EnemyMgr.Inst.getRandomPos(), () => onNormalEnemyDie(idx2Spwn));
            curEnemyCount++;
        }
    }



    /// <summary>
    /// ���� ��ȯ�Ǿ��ִ� ��� �Ϲ� ���� ���ֹ�����, �Ϲ� ���� ��ȯ�� ����
    /// </summary>
    public void removeAllNormalEnemies()
    {
        StopCoroutine(curSpawnRoutine);
        EnemyMgr.Inst.canSpawnEnemy = false;

        Enemy[] spawnedEnemies = GameObject.FindObjectsOfType<Enemy>();

        for (int i = 0; i < spawnedEnemies.Length; i++)
        {
            if (!spawnedEnemies[i].CompareTag("Boss"))
            {
                spawnedEnemies[i].Despawn();
            }
        }

    }


    /// <summary>
    /// ���� ������ ȣ�� �� �Լ�.
    /// </summary>
    /// <param name="index">�ش� ���� StageInfo������ index </param>
    void onNormalEnemyDie(int index)
    {
        curEnemyCount--;
        if (index != 0) curSpawnedEnemies.Add(index);

        progressCount++;
        UIMgr.Inst.progress.SetProgress(progressCount, info.maxKill);
        if (progressCount >= info.maxKill)
        {
            startBossStage();
        }
    }

    void onBossDie()
    {

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
        if (curSlowtimeCoroutine != null) StopCoroutine(curSlowtimeCoroutine);// ������ ����ǰ� �־��� �ð� ���� �ڷ�ƾ ����

        curSlowtimeCoroutine = StartCoroutine(co_SlowTime(time, amount));
    }

    IEnumerator co_SlowTime(float time, float amount, bool isSlowTimeLocked = false)
    {
        Time.timeScale = amount;

        slowTimeLock = isSlowTimeLocked;
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
