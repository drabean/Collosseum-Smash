using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMgr : MonoSingleton<GameMgr>
{
    [HideInInspector] public CameraController MainCam;
    //   [SerializeField] List<Transform> audiencesPoint = new List<Transform>();

    Player player;
    
    protected void Awake()
    {
        MainCam = Camera.main.GetComponent<CameraController>();

        Pool_attackWarningLinear = new ObjectPool("Prefabs/AttackWarningLinear");
        Pool_attackWarningCircle = new ObjectPool("Prefabs/AttackWarningCircle");

        holder = Resources.Load<ItemEquipHolder>("Prefabs/ItemEquipHolder");
    }


    public bool isTest;
    public StageInfo testStage;

    private IEnumerator Start()
    {
        GameData.Inst.selectStage();
        //테스트를 위한 임의 스테이지 지정
        if (testStage != null) info = testStage;
        else info = GameData.Inst.curStageInfo;

        Time.timeScale = 1;
        player = GameObject.FindObjectOfType<Player>();

        player.AttachUI();
        if (isTest) yield break;
        yield return new WaitForSeconds(2.0f);

        UIMgr.Inst.progress.ShowStageStart();

        yield return new WaitForSeconds(2.0f);
        UIMgr.Inst.progress.HideAll();
        StartNormalStage();
    }
    StageInfo info;
    Coroutine curSpawnRoutine;
    int maxCount = 3; // 소환 될 수 있는 최대 마리수
    int progressCount = 0; //
    bool isBossSpawned;

    public void StartNormalStage()
    {
        Init();

        SoundMgr.Inst.PlayBGM(info.Intro, info.BGM);
        if (curSpawnRoutine != null) StopCoroutine(curSpawnRoutine);
        UIMgr.Inst.progress.ShowNormalUI();
        curSpawnRoutine = StartCoroutine(normalEnemySpawnRoutine());
    }
    void startBossStage()
    {
        if (isBossSpawned) return;
        isBossSpawned = true;
        StartCoroutine(co_StartBossStage());
    }

    IEnumerator co_StartBossStage()
    {

        GameMgr.Inst.removeAllNormalEnemies();

        UIMgr.Inst.progress.HideAll();
        EnemyMgr.Inst.SpawnBossEnemy(info.Boss, Vector3.up, onBossDie);
        yield return new WaitForSeconds(1f);
        UIMgr.Inst.progress.ShowBossUI();
    }
    List<int> curSpawnedEnemies; // 현재 소환 될 수 있는 적들의 index

    void Init()
    {
        curSpawnedEnemies = new List<int>();
        for (int i = 1; i < info.Enemies.Count; i++)
        {
            curSpawnedEnemies.Add(i);
        }
    }


    #region 소환로직

    /// <summary>
    /// 현재 소환 가능한 적의 index를 반환
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
            EnemyMgr.Inst.SpawnEnemy(info.Enemies[idx2Spwn], EnemyMgr.Inst.getRandomPos(), (pos) => onNormalEnemyDie(idx2Spwn));
            curEnemyCount++;
        }
    }



    /// <summary>
    /// 현재 소환되어있는 모든 일반 적을 없애버리고, 일반 적의 소환을 막음
    /// </summary>
    public void removeAllNormalEnemies()
    {
        if (curSpawnRoutine != null) StopCoroutine(curSpawnRoutine);

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
    /// 적이 죽을떄 호출 될 함수.
    /// </summary>
    /// <param name="index">해당 적의 StageInfo에서의 index </param>
    void onNormalEnemyDie(int index)
    {
        curEnemyCount--;
        if (index != 0) curSpawnedEnemies.Add(index);

        progressCount++;
        if (progressCount == info.maxKill / 2) maxCount++; // 진행도의 반에 도달 했다면, 최대 소환수를 1 늘려줌.

        UIMgr.Inst.progress.SetProgress(progressCount, info.maxKill);
        if (progressCount >= info.maxKill)
        {
            startBossStage();
        }
    }

    void onBossDie(Vector3 pos)
    {
        StartCoroutine(co_SpawnItems());
        StartCoroutine(SoundMgr.Inst.co_BGMFadeOut(2.0f));
        UIMgr.Inst.progress.HideAll();
    }
    
    IEnumerator co_SpawnItems()
    {
        yield return new WaitForSeconds(1.0f);

        MainCam.lockPos(Vector3.forward * -1);
        player.AutoMove(Vector3.up * -6.4f);

        yield return new WaitForSeconds(2.0f);
        spawnItems();
    }
    Equip getRandomItem()
    {
        if (ItemMgr.Inst.equipPool.Count == 0) return null;
        Equip equip = ItemMgr.Inst.getRandomEquip();
        return equip;
    }

    ItemEquipHolder holder;
    List<ItemEquipHolder> curEquips = new List<ItemEquipHolder>(); //풀에서 꺼내서 현재 스테이지에 배치된 아이템들.

    [ContextMenu("SpawnITem")]
    void testSpawnItem()
    {
        StartCoroutine(co_SpawnItems());
    }
    void spawnItems()
    {
        for (int i = 0; i < 3; i++)
        {
            Equip e2s = getRandomItem();
            if (e2s == null) return;
            ItemEquipHolder h = Instantiate<ItemEquipHolder>(holder);
            h.SetItem(e2s);
            h.transform.position = Vector3.right * (-4 + 4 * i) + Vector3.up * -1.5f;
            h.onAcquire = onAcqureEquip;
            h.GetComponent<ModuleHit>().FlashWhite(0.2f);
            h.index = i;
            DictionaryPool.Inst.Pop("Prefabs/Smoke").transform.position = h.transform.position;
            curEquips.Add(h);
        }
    }

    void onAcqureEquip(int index)
    {
        for(int i = 0; i < curEquips.Count; i++)
        {
            if (i != index) ItemMgr.Inst.equipPool.Add(curEquips[i].equip);
            Destroy(curEquips[i].gameObject);
        }

        StartCoroutine(co_ToNextScene());
    }

    IEnumerator co_ToNextScene()
    {
        yield return new WaitForSeconds(3.0f);
        LoadSceneMgr.LoadSceneAsync("Main");
    }
    #endregion
    #region 유틸 함수들
    int score = 0;
    public void addScore(int score)
    {
        this.score += score;
        UIMgr.Inst.score.Set(this.score);

    }

    Coroutine curSlowtimeCoroutine;
    bool slowTimeLock;
    /// <summary>
    /// 일정 시간동안 시간 배율을 조절합니다.
    /// </summary>
    /// <param name="time"> 배율을 조절하는 시간 </param>
    /// <param name="amount"> 시간 배율 </param>
    public void SlowTime(float time, float amount, bool isSlowTimeLocked = false)
    {
        if (slowTimeLock) return;
        if (curSlowtimeCoroutine != null) StopCoroutine(curSlowtimeCoroutine);// 이전에 실행되고 있었던 시간 멈춤 코루틴 중지

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
