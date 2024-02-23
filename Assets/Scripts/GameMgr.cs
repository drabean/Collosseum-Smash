using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using Random = UnityEngine.Random;


public enum PHASE
{
    LOADING,
    NORMAL,
    BOSS,
    REWARD
}
public class GameMgr : MonoSingleton<GameMgr>
{
    [HideInInspector] public CameraController MainCam;
    [HideInInspector] public RunData curRunData;
    public bool isPlayerInstantiated;

    public Player player;
    
    protected void Awake()
    {
        MainCam = Camera.main.GetComponent<CameraController>();

        Pool_attackWarningLinear = new ObjectPool("Prefabs/AttackWarningLinear");
        Pool_attackWarningCircle = new ObjectPool("Prefabs/AttackWarningCircle");

        holder = Resources.Load<ItemEquipHolder>("Prefabs/ItemEquipHolder");
    }


    [Tooltip("isChecked, stage does not start. for test only")]
    public bool isTest;
    [Tooltip("testRunData for testMode.")]
    public RunData testRunData;
    [Tooltip("if not null, testStage will be loaded instead of saved stage")]
    public StageInfo testStage;

    public Enemy GongPrefab;

    public TextMeshPro progressTMP;
    PHASE curPhase = PHASE.LOADING;

    public ItemCoin CoinPrefab;
    
    private IEnumerator Start()
    {
        if (isTest)
        {
            if (!LoadedSave.isInit) LoadedSave.Inst.Init();
            if (!LoadedData.isDataLoaded) LoadedSave.Inst.Init();
        }
        //데이터 불러오기
        else curRunData = UTILS.GetRunData();
        //플레이어 생성 및 설정 동기화
        player = Instantiate(LoadedData.Inst.getCharacterInfoByID(curRunData.characterInfoIdx).playerPrefab);
        player.transform.position = Vector3.up * -3f;
        MainCam.SetBaseTarget(player.transform);
        EnemyMgr.Inst.player = player;

        //플레이어 아이템 장착
        for(int i = 0; i < curRunData.item.Count; i++)
        {
            Instantiate(LoadedData.Inst.getEquipByID(curRunData.item[i])).onEquip(player);
        }

        checkUnlock();

        player.SetStatus();
        player.curHP = Mathf.Min(player.maxHP, curRunData.curHP); // 체력세팅.
        player.AttachUI(); // 조이스틱 및 UI 연동
        isPlayerInstantiated = true;



        ItemMgr.Inst.InitNormalEquipPool(curRunData);
        ItemMgr.Inst.InitPotionEquipPool(player);
        //테스트를 위한 임의 스테이지 지정
        if (testStage == null) stageInfo = getStageData();
        else stageInfo = testStage;

        if(stageInfo.StageDeco != null) Instantiate(stageInfo.StageDeco, Vector3.zero, Quaternion.identity);
        Time.timeScale = 1;

        //즉시 시작이 아닌, 별도 오브젝트를 공격함으로서 게임이 시작하도록 만듬.

        if (isTest && testStage == null) yield break;


        if(curRunData.isTutorial)
        {
            TutorialMgr tutorial = gameObject.AddComponent<TutorialMgr>();
            tutorial.Init(player, progressTMP);
            tutorial.startTutorial();
            yield break;
        }
        else if(curRunData.isBoss)
        {
            yield return new WaitForSeconds(1.5f);
            SoundMgr.Inst.PlayBGM(stageInfo.Intro, stageInfo.BGM);
            startBossStage();
            yield break;
        }
        else
        {
            spawnGong();
            yield break;
        }
    }
 
    void checkUnlock()
    {
        if(LoadedSave.Inst.save.CheckUnlock(UNLOCKS.ATKSPD))
        {
            player.GetComponent<Animator>().SetFloat("atkSpd",1.15f);
        }
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCKS.THRWDMG))
        {
            player.Stat.ACC++;
        }
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCKS.MAXHP))
        {
            player.Stat.VIT++;
        }
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCKS.MONEY))
        {
            coinCount-=2;
        }
    }
    void spawnGong()
    {
        progressTMP.text = "SMASH TO START!";

        Enemy Gong = Instantiate(GongPrefab);
        Gong.transform.position = Vector3.up * 4;
        //StartNormalStage();
        Gong.onDeath += position =>
        {
            // 무명 함수 내부에서 StartStage() 함수 호출
            StartCoroutine(StartNormalStage());
        };
    }
    StageInfo getStageData()
    {
        if(curRunData.stageProgress >= LoadedData.Inst.stageInfos.Length)
        {
            Debug.Log("All Clear!");
            return null;
        }
        return LoadedData.Inst.stageInfos[curRunData.stageProgress];
    }
    StageInfo stageInfo;
    Coroutine curSpawnRoutine;
    int maxCount = 3; // 소환 될 수 있는 최대 마리수
    int maxBaseEnemyCount = 3;
    int progressCount = 0; 
    bool isBossSpawned;
    public int dropCount = 6; // 투척 아이템 소환 빈도
    int curDropCount = 0;
    public int coinCount = 10;
    int curCoinCount = 0;
    public IEnumerator StartNormalStage()
    {
        UIMgr.Inst.progress.ShowStageStart();

        yield return new WaitForSeconds(1.0f);
        UIMgr.Inst.progress.HideAll();

        InitEnemyPool();
        yield return new WaitForSeconds(1.0f);

        SoundMgr.Inst.PlayBGM(stageInfo.Intro, stageInfo.BGM);
        if (curSpawnRoutine != null) StopCoroutine(curSpawnRoutine);
        UIMgr.Inst.progress.ShowNormalUI();
        progressCount = curRunData.normalProgress;
        progressTMP.text = progressCount + " / " + stageInfo.maxKill;
        curSpawnRoutine = StartCoroutine(normalEnemySpawnRoutine());
        curPhase = PHASE.NORMAL;
    }

    void InitEnemyPool()
    {
        enemiesCanBeSpawned = new List<int>();
        for (int i = 1; i < stageInfo.Enemies.Count; i++)
        {
            enemiesCanBeSpawned.Add(i);
        }
    }


    #region 소환로직

    /// <summary>
    /// 현재 소환 가능한 적의 index를 반환
    /// </summary>
    /// <returns></returns>
    int getIndexToSpawn()
    {
        if (enemiesCanBeSpawned.Count == 0) return 0; // 이 스테이지에서 소환할 수 있는 종류의 적이 더 없다면, 0 반환
        else if(baseEnemyCount < maxBaseEnemyCount) // 기본 근접 타입 적의 수를 언제나 2마리로 유지하기 위해, 0 반환
        {
            return 0;
        }
        else
        {
            int idx2Spwn = enemiesCanBeSpawned[Random.Range(0, enemiesCanBeSpawned.Count)]; // 소환 가능한 적 인덱스중 하나 반환
            enemiesCanBeSpawned.Remove(idx2Spwn);
            return idx2Spwn;
        }
    }
    int curEnemyCount = 0;
    int baseEnemyCount = 0; // 기본형 적 최대 소환 수
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
            if (idx2Spwn == 0) baseEnemyCount++;

            EnemyMgr.Inst.SpawnEnemy(stageInfo.Enemies[idx2Spwn], EnemyMgr.Inst.getRandomPos(), (pos) => onNormalEnemyDie(idx2Spwn));
            curEnemyCount++;
        }
    }

    void startBossStage()
    {
        if (isBossSpawned) return;
        isBossSpawned = true;
        curPhase = PHASE.BOSS;
        StartCoroutine(co_StartBossStage());
    }

    IEnumerator co_StartBossStage()
    {

        GameMgr.Inst.removeAllNormalEnemies();

        progressTMP.text = stageInfo.bossText;

        UIMgr.Inst.progress.HideAll();
        EnemyMgr.Inst.SpawnBossEnemy(stageInfo.Boss, Vector3.up, onBossDie);
        yield return new WaitForSeconds(1f);
        progressTMP.text = stageInfo.bossText;
        yield return new WaitForSeconds(1.7f);
        progressTMP.text = "";
    }
    /// <summary>
    /// 현재 소환 가능한 몬스터들 Index
    /// </summary>
    List<int> enemiesCanBeSpawned;

    /// <summary>
    /// 현재 소환되어있는 모든 일반 적을 없애버리고, 일반 적의 소환을 막음
    /// </summary>
    public void removeAllNormalEnemies()
    {
        if (curSpawnRoutine != null) StopCoroutine(curSpawnRoutine);

        EnemyMgr.Inst.canSpawnEnemy = false;

        //소환되어있는 적 모두 제거
        Enemy[] spawnedEnemies = GameObject.FindObjectsOfType<Enemy>();
        for (int i = 0; i < spawnedEnemies.Length; i++)
        {
            if (!spawnedEnemies[i].CompareTag("Boss"))
            {
                spawnedEnemies[i].Despawn();
            }
        }
        //모든 공격 제거
        Attack[] attacks = GameObject.FindObjectsOfType<Attack>();
        foreach(Attack a in attacks)
        {
            Destroy(a.gameObject);
        }
    }


    /// <summary>
    /// 적이 죽을떄 호출 될 함수.
    /// </summary>
    /// <param name="index">해당 적의 StageInfo에서의 index </param>
    void onNormalEnemyDie(int index)
    {
        curEnemyCount--;
        if (index != 0) enemiesCanBeSpawned.Add(index);
        else baseEnemyCount--;

        progressCount++;

        #region 난이도관리
        switch(progressCount)
        {
            case <=5:
                baseEnemyCount = 3;
                maxCount = 4;
                break;
            case <= 15:
                baseEnemyCount = 4;
                maxCount = 5;
                break;
            case <= 25:
                baseEnemyCount = 3;
                maxCount = 5;
                break;
            case <= 30:
                baseEnemyCount = 4;
                maxCount = 6;
                break;           
            case <= 35:
                baseEnemyCount = 4;
                maxCount = 7;
                break;
        }



        #endregion
        UIMgr.Inst.progress.SetProgress(progressCount, stageInfo.maxKill);
        progressTMP.text = progressCount + " / " + stageInfo.maxKill;
        if (progressCount >= stageInfo.maxKill)
        {
            removeAllNormalEnemies();
            startBossStage();
        }
    }

    /// <summary>
    /// 투척 아이템 및 재화 생성 관리 함수
    /// </summary>
    /// <param name="position"></param>
    public void SpawnThrowableItem(Vector3 position)
    {
        curDropCount++;
        if (curDropCount >= dropCount)
        {
            curDropCount = 0;
            Debug.Log("SPAWN");
            Instantiate(stageInfo.ThrowItem, position, Quaternion.identity);
        }
        curCoinCount++;
        if(curCoinCount >= coinCount)
        {
            curCoinCount = 0;
            Instantiate(CoinPrefab, position, Quaternion.identity).Init(1);
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
    #endregion

    #region 아이템 관련
    ItemEquipHolder holder;
    [SerializeField]ModuleHit DescriptionObject;
    List<ItemEquipHolder> curEquips = new List<ItemEquipHolder>(); //풀에서 꺼내서 현재 스테이지에 배치된 아이템들.

    [ContextMenu("SpawnITem")]
    void testSpawnItem()
    {
        StartCoroutine(co_SpawnItems());
    }
    void spawnItems()
    {
        UIMgr.Inst.itemDescription.ShowDescription("Select One!");

        DescriptionObject.gameObject.SetActive(true);
        DescriptionObject.FlashWhite(0.1f);
        for (int i = 0; i < 3; i++)
        {
            Equip e2s;
            if (i != 2) e2s = ItemMgr.Inst.GetNormalEquip();
            else e2s = ItemMgr.Inst.GetPotionEquip();

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
            if (i == index)
            {
                curRunData.item.Add(curEquips[index].equip.ID);
            }
            Destroy(curEquips[i].gameObject);
        }

        StartCoroutine(co_ToNextScene());
    }
    #endregion

    /// <summary>
    /// 정상적으로 게임 클리어 해서 다음 라운드로 넘어갈때
    /// </summary>
    /// <returns></returns>
    IEnumerator co_ToNextScene()
    {
        curRunData.stageProgress++;
        curRunData.curHP = (int)player.curHP + 2;
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCKS.HPREG)) curRunData.curHP++;
        curRunData.isBoss = false;

        curRunData.normalProgress = 0;
        curRunData.bossProgress = 0;
        //세이브데이터는 여기서!
        yield return new WaitForSeconds(3.0f);

        UTILS.SaveRunData(curRunData);
        LoadedSave.Inst.SyncSaveData();
        LoadSceneMgr.LoadSceneAsync("Main");
    }

    #region 유틸 함수들
    /// <summary>
    /// 플레이어 사망시 플레이어쪽에서 호출
    /// 현재 스테이지 안에서의 진행사항을 임시적으로 저장함.
    /// </summary>
    public void SaveCurRunData()
    {
        switch(curPhase)
        {
            case PHASE.NORMAL:
                curRunData.isBoss = false;
                curRunData.normalProgress = progressCount;
                break;
            case PHASE.BOSS:
                curRunData.isBoss = true;

                curRunData.bossProgress = (int)(UIMgr.Inst.progress.bossMaxHP - UIMgr.Inst.progress.bossCurHP);
                break;
        }
        UTILS.SaveRunData(curRunData);
    }

    Coroutine curSlowtimeCoroutine;
    bool slowTimeLock;
    public bool isPause;
    /// <summary>
    /// 일정 시간동안 시간 배율을 조절합니다.
    /// </summary>
    /// <param name="time"> 배율을 조절하는 시간 </param>
    /// <param name="amount"> 시간 배율 </param>
    public void SlowTime(float time, float amount, bool isSlowTimeLocked = false)
    {
        if (slowTimeLock) return;
        if (curSlowtimeCoroutine != null) StopCoroutine(curSlowtimeCoroutine);// 이전에 실행되고 있었던 시간 멈춤 코루틴 중지

        curSlowtimeCoroutine = StartCoroutine(co_SlowTime(time, amount, isSlowTimeLocked));
    }

    IEnumerator co_SlowTime(float time, float amount, bool isSlowTimeLocked)
    {
        if (!isPause) Time.timeScale = amount;

        slowTimeLock = isSlowTimeLocked;
        yield return new WaitForSecondsRealtime(time);

        if(!isPause)Time.timeScale = 1; // 일시정지중에 해당 분기에서 일시정지 푸는 경우 방지
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


    public void Btn_ToTitle()
    {
        LoadSceneMgr.LoadSceneAsync("Start");
    }

    public void Btn_Exit()
    {
        Application.Quit();
    }

    #endregion
}
