using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 패턴 정보 구조체
/// </summary>
[System.Serializable]
public struct PatternInfo
{
    public float range;
    public float width;

    //공격 선후딜
    public float waitBeforeTime;
    public float waitAfterTime;

    //반복 공격 시 대기시간 및 반복 횟수
    public float intervalTime;
    public int repeatTIme;

    //지속시간이 있는 패턴일 시, 지속시간
    public float duration;
    
    //패턴에서 사용 할 프리팹 path.
    public string prefabName;
}
public class EnemyBoss : Enemy
{
    public ACHIEVEMENT BossAchivement;
    /// <summary>
    /// 패턴 정보
    /// </summary>
    public PatternInfo[] patterns;
    /// <summary>
    /// 패턴 초기화 시 패턴 카운트
    /// </summary>
    public int patternCount;
    /// <summary>
    /// 남은 패턴 카운트
    /// </summary>
    protected int patternCountLeft;

    /// <summary>
    /// 현재 상태에 따른 패턴 선택
    /// </summary>
    protected virtual void selectPattern() { }

    /// <summary>
    /// 하드모드 패턴을 사용할지 정함
    /// </summary>
    public bool isHardMode = false;
    /// <summary>
    /// 상태 확인용으로 서브로 사용하는 HP
    /// </summary>
    protected float subHP = 0;
    /// <summary>
    /// 소환할 잡몹들
    /// </summary>
    public List<Enemy> mobs = new List<Enemy>();
    public override void onHit(Transform attackerPos, float dmg, float stunTime = 0.0f)
    {
        checkRage();
        subHP -= dmg;
        base.onHit(attackerPos, dmg, stunTime);
        UIMgr.Inst.progress.SetBossHP(curHP, maxHP);
    }

    int explosionRepeatTime = 10;

    ItemCoin coin;
    protected override IEnumerator co_Smash(Transform attackerPos)
    {
        GameMgr.Inst.removeAllNormalEnemies();

        GameMgr.Inst.MainCam.changeTarget(transform);
        //죽은 적을 다시 공격하는 것을 막기 위해, 콜라이더 없애줌
        Collider2D[] cols = GetComponentsInChildren<Collider2D>();


        LoadedSave.Inst.TryAddAchievement(BossAchivement);


        if(cols.Length != 0)
        {
            foreach(Collider2D col in cols)
            {
                Destroy(col);
            }
        }

        GameMgr.Inst.SlowTime(3f, 0.3f, true);
        coin = Resources.Load<ItemCoin>("Prefabs/Item/ItemCoin");
        //여기서 적 사망 연출
        for(int i = 0; i < explosionRepeatTime; i++)
        {
            hit.FlashWhite(0.2f);
            GameObject temp = DictionaryPool.Inst.Pop("Prefabs/Effect/ExplosionEffect");
            temp.transform.position = transform.position.Randomize(size);
            ItemCoin c = Instantiate(coin, transform.position, Quaternion.identity);

            RunData curRunData = GameMgr.Inst.curRunData;

            c.Init(1);
            c.onAcquire += () => { curRunData.totalCoinCount++; };
            yield return new WaitForSecondsRealtime(0.3f);
        }

        SoundMgr.Inst.Play("Smash");

        yield return base.co_Smash(GameMgr.Inst.player.transform);
        ItemCoin[] coins = GameObject.FindObjectsOfType<ItemCoin>();
        foreach (ItemCoin c in coins) c.magnet(Target.transform);
        ItemThrowable[] throwItems = GameObject.FindObjectsOfType<ItemThrowable>();
        foreach (ItemThrowable t in throwItems) Destroy(t.gameObject);
        GameMgr.Inst.MainCam.changeTargetToDefault();
    }
    #region Rage

    protected bool isRage = false; // 현재 분노 페이즈인가
    protected bool isRagePattern = false;
    public bool alreadyUsedPattern = false; // 반피 이하에서 Continue했을때, 바로 발악패턴 쓰는거 막음
    //반피 이하일 시 2페이즈 진입
    protected virtual void checkRage()
    {
        if(!isRage && (curHP < ((float)maxHP * (0.6f))))
        {
            isRage = true;
            if (!alreadyUsedPattern) isRagePattern = true;
            else isRagePattern = isRagePattern = false;
            rageChange();
            Debug.Log("RAGE!");
        }
    }

   protected virtual void rageChange()
    {

    }


    #endregion
    #region Spawn
    public int maxSpawnCount = 3;
    protected int curSpawnCount = 0;

    protected virtual void spawnMob(int idx, Action<Vector3> deadOption, BUF buf = BUF.NONE)
    {
        spawnMob(idx, EnemyMgr.Inst.getRandomPos(), deadOption, buf);
    }
     protected virtual void spawnMob(int idx, Vector3 position, Action<Vector3> deadOption, BUF buf = BUF.NONE)
    {
        spawnMob(mobs[idx], position, deadOption, buf);
    }

    protected virtual void spawnMob(Enemy enemyPrefab, Vector3 position, Action<Vector3> deadOption, BUF buf = BUF.NONE)
    {
        if (curSpawnCount >= maxSpawnCount) return;
        curSpawnCount++;
        EnemyMgr.Inst.SpawnEnemy(enemyPrefab, position, deadOption, buf);
    }
    protected virtual void deadOption(Vector3 hitVec)
    {
        curSpawnCount--;
    }

#endregion
}
