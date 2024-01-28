using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using Random = UnityEngine.Random;

public class GameMgr : MonoSingleton<GameMgr>
{
    [HideInInspector] public CameraController MainCam;
    [HideInInspector] public RunData curRunData;
    [HideInInspector] public SaveData curSaveData;
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
    private IEnumerator Start()
    {
        if (isTest)
        {
            LoadedData.Inst.LoadData();
            curRunData = testRunData;
        }
        //������ �ҷ�����
        else curRunData = UTILS.GetRunData();
        curSaveData = UTILS.LoadSaveData();
        //�÷��̾� ���� �� ���� ����ȭ
        player = Instantiate(LoadedData.Inst.getCharacterInfoByID(curRunData.characterInfoIdx).playerPrefab);
        player.transform.position = Vector3.up * -3f;
        MainCam.SetBaseTarget(player.transform);
        EnemyMgr.Inst.player = player;

        //�÷��̾� ������ ����
        for(int i = 0; i < curRunData.item.Count; i++)
        {
            Instantiate(LoadedData.Inst.getEquipByID(curRunData.item[i])).onEquip(player);
        }
        player.SetStatus();
        player.Heal(10);
        player.AttachUI();
        isPlayerInstantiated = true;

        ItemMgr.Inst.InitNormalEquipPool(curRunData);
        ItemMgr.Inst.InitPotionEquipPool(player);
        //�׽�Ʈ�� ���� ���� �������� ����
        if (testStage == null) stageInfo = getStageData();
        else stageInfo = testStage;

        if(stageInfo.StageDeco != null) Instantiate(stageInfo.StageDeco, Vector3.zero, Quaternion.identity);
        Time.timeScale = 1;

        //��� ������ �ƴ�, ���� ������Ʈ�� ���������μ� ������ �����ϵ��� ����.

        if (isTest && testStage == null) yield break;

        if(stageInfo == null)
        {
            //���� Ŭ����� ���� rundata ���� �� ���� Ŭ�����ϱ�
            GameClearMgr clear = gameObject.AddComponent<GameClearMgr>();
            clear.Init(curSaveData);
            curSaveData.ClearAchivement(ACHIEVEMENT.NORMALCLEAR);
        }

        if(curSaveData.checkAchivement(ACHIEVEMENT.TUTORIALCLEAR)) spawnGong();
        else
        {
            //TUTORIAL START
            TutorialMgr tutorial =  gameObject.AddComponent<TutorialMgr>();
            tutorial.Init(player, progressTMP, curSaveData);
            tutorial.startTutorial();
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
            // ���� �Լ� ���ο��� StartStage() �Լ� ȣ��
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
    int maxCount = 3; // ��ȯ �� �� �ִ� �ִ� ������
    int progressCount = 0; //
    bool isBossSpawned;
    int dropCount = 4; // ��ô ������ ��ȯ ��
    int curDropCount = 0;
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
        progressTMP.text = progressCount + " / " + stageInfo.maxKill;
        curSpawnRoutine = StartCoroutine(normalEnemySpawnRoutine());
    }

    void InitEnemyPool()
    {
        enemiesCanBeSpawned = new List<int>();
        for (int i = 1; i < stageInfo.Enemies.Count; i++)
        {
            enemiesCanBeSpawned.Add(i);
        }
    }


    #region ��ȯ����

    /// <summary>
    /// ���� ��ȯ ������ ���� index�� ��ȯ
    /// </summary>
    /// <returns></returns>
    int getIndexToSpawn()
    {
        if (enemiesCanBeSpawned.Count == 0) return 0; // �� ������������ ��ȯ�� �� �ִ� ������ ���� �� ���ٸ�, 0 ��ȯ
        else if(baseEnemyCount < 2) // �⺻ ���� Ÿ�� ���� ���� ������ 2������ �����ϱ� ����, 0 ��ȯ
        {
            return 0;
        }
        else
        {
            int idx2Spwn = enemiesCanBeSpawned[Random.Range(0, enemiesCanBeSpawned.Count)]; // ��ȯ ������ �� �ε����� �ϳ� ��ȯ
            enemiesCanBeSpawned.Remove(idx2Spwn);
            return idx2Spwn;
        }
    }
    int curEnemyCount = 0;
    int baseEnemyCount = 0;
    WaitForSeconds waitForEnemySpawn = new WaitForSeconds(0.5f);
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
        StartCoroutine(co_StartBossStage());
    }

    IEnumerator co_StartBossStage()
    {

        GameMgr.Inst.removeAllNormalEnemies();

        progressTMP.text = stageInfo.bossText;

        UIMgr.Inst.progress.HideAll();
        EnemyMgr.Inst.SpawnBossEnemy(stageInfo.Boss, Vector3.up, onBossDie);
        yield return new WaitForSeconds(1f);
        UIMgr.Inst.progress.ShowBossUI();
        yield return new WaitForSeconds(1.5f);
        progressTMP.text = "";
    }
    /// <summary>
    /// ���� ��ȯ ������ ���͵� Index
    /// </summary>
    List<int> enemiesCanBeSpawned;

    /// <summary>
    /// ���� ��ȯ�Ǿ��ִ� ��� �Ϲ� ���� ���ֹ�����, �Ϲ� ���� ��ȯ�� ����
    /// </summary>
    public void removeAllNormalEnemies()
    {
        if (curSpawnRoutine != null) StopCoroutine(curSpawnRoutine);

        EnemyMgr.Inst.canSpawnEnemy = false;

        //��ȯ�Ǿ��ִ� �� ��� ����
        Enemy[] spawnedEnemies = GameObject.FindObjectsOfType<Enemy>();
        for (int i = 0; i < spawnedEnemies.Length; i++)
        {
            if (!spawnedEnemies[i].CompareTag("Boss"))
            {
                spawnedEnemies[i].Despawn();
            }
        }
        //��� ���� ����
        Attack[] attacks = GameObject.FindObjectsOfType<Attack>();
        foreach(Attack a in attacks)
        {
            Destroy(a.gameObject);
        }
    }


    /// <summary>
    /// ���� ������ ȣ�� �� �Լ�.
    /// </summary>
    /// <param name="index">�ش� ���� StageInfo������ index </param>
    void onNormalEnemyDie(int index)
    {
        curEnemyCount--;
        if (index != 0) enemiesCanBeSpawned.Add(index);
        else baseEnemyCount--;

        progressCount++;
        if (progressCount == stageInfo.maxKill / 2) maxCount++; // ���൵�� �ݿ� ���� �ߴٸ�, �ִ� ��ȯ���� 1 �÷���.
        curDropCount++;

        if(curDropCount == dropCount)
        {
            curDropCount = 0;
            SpawnThrowableItem();
        }
        UIMgr.Inst.progress.SetProgress(progressCount, stageInfo.maxKill);
        progressTMP.text = progressCount + " / " + stageInfo.maxKill;
        if (progressCount >= stageInfo.maxKill)
        {
            removeAllNormalEnemies();
            startBossStage();
        }
    }

    public void SpawnThrowableItem()
    {
        Debug.Log("SPAWN");
        Instantiate(stageInfo.ThrowItem, EnemyMgr.Inst.getRandomPos(), Quaternion.identity);
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


    #region ������ ����
    ItemEquipHolder holder;
    [SerializeField]ModuleHit DescriptionObject;
    List<ItemEquipHolder> curEquips = new List<ItemEquipHolder>(); //Ǯ���� ������ ���� ���������� ��ġ�� �����۵�.

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

    IEnumerator co_ToNextScene()
    {
        curRunData.stageProgress++;
        curSaveData.Exp++;
        yield return new WaitForSeconds(3.0f);
        //TESTCODE
        foreach(int i in curRunData.item)
        {
            Debug.Log(LoadedData.Inst.getEquipByID(i).name);
        }
        UTILS.SaveRunData(curRunData);
        UTILS.SaveSaveData(curSaveData);
        LoadSceneMgr.LoadSceneAsync("Main");
    }
    #endregion

    #region ��ƿ �Լ���
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

        curSlowtimeCoroutine = StartCoroutine(co_SlowTime(time, amount, isSlowTimeLocked));
    }

    IEnumerator co_SlowTime(float time, float amount, bool isSlowTimeLocked)
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
