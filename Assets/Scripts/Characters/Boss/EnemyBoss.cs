using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ���� ���� ����ü
/// </summary>
[System.Serializable]
public struct PatternInfo
{
    public float range;
    public float width;

    //���� ���ĵ�
    public float waitBeforeTime;
    public float waitAfterTime;

    //�ݺ� ���� �� ���ð� �� �ݺ� Ƚ��
    public float intervalTime;
    public int repeatTIme;

    //���ӽð��� �ִ� ������ ��, ���ӽð�
    public float duration;
    
    //���Ͽ��� ��� �� ������ path.
    public string prefabName;
}
public class EnemyBoss : Enemy
{
    /// <summary>
    /// ���� ����
    /// </summary>
    public PatternInfo[] patterns;
    /// <summary>
    /// ���� �ʱ�ȭ �� ���� ī��Ʈ
    /// </summary>
    public int patternCount;
    /// <summary>
    /// ���� ���� ī��Ʈ
    /// </summary>
    protected int patternCountLeft;

    /// <summary>
    /// ���� ���¿� ���� ���� ����
    /// </summary>
    protected virtual void selectPattern() { }

    /// <summary>
    /// �ϵ��� ������ ������� ����
    /// </summary>
    public bool isHardMode = false;
    /// <summary>
    /// ���� Ȯ�ο����� ����� ����ϴ� HP
    /// </summary>
    protected float subHP = 0;
    /// <summary>
    /// ��ȯ�� �����
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
        //���� ���� �ٽ� �����ϴ� ���� ���� ����, �ݶ��̴� ������
        Collider2D[] cols = GetComponentsInChildren<Collider2D>();
        if(cols.Length != 0)
        {
            foreach(Collider2D col in cols)
            {
                Destroy(col);
            }
        }

        GameMgr.Inst.SlowTime(3f, 0.3f, true);
        coin = Resources.Load<ItemCoin>("Prefabs/Item/ItemCoin");
        List<ItemCoin> coins = new List<ItemCoin>();
        //���⼭ �� ��� ����
        for(int i = 0; i < explosionRepeatTime; i++)
        {
            hit.FlashWhite(0.2f);
            GameObject temp = DictionaryPool.Inst.Pop("Prefabs/Effect/ExplosionEffect");
            temp.transform.position = transform.position.Randomize(size);
            ItemCoin c = Instantiate(coin, transform.position, Quaternion.identity);
            c.Init(1);
            coins.Add(c);
            yield return new WaitForSecondsRealtime(0.3f);
        }

        SoundMgr.Inst.Play("Smash");

        yield return base.co_Smash(GameMgr.Inst.player.transform);
        foreach (ItemCoin c in coins) { if(c != null)c.magnet(Target.transform); }

        GameMgr.Inst.MainCam.changeTargetToDefault();
    }
    #region Rage

    protected bool isRage = false; // ���� �г� �������ΰ�
    protected bool isRagePattern = false;
    public bool alreadyUsedPattern = false; // ���� ���Ͽ��� Continue������, �ٷ� �߾����� ���°� ����
    //���� ������ �� 2������ ����
    protected virtual void checkRage()
    {
        if(!isRage && (curHP < ((float)maxHP/2)))
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

    protected virtual void spawnMob(int idx, Action<Vector3> deadOption)
    {
        spawnMob(idx, EnemyMgr.Inst.getRandomPos(), deadOption);
    }
     protected virtual void spawnMob(int idx, Vector3 position, Action<Vector3> deadOption)
    {
        spawnMob(mobs[idx], position, deadOption);
    }

    protected virtual void spawnMob(Enemy enemyPrefab, Vector3 position, Action<Vector3> deadOption)
    {
        if (curSpawnCount >= maxSpawnCount) return;
        curSpawnCount++;
        EnemyMgr.Inst.SpawnEnemy(enemyPrefab, position, deadOption);
    }
    protected virtual void deadOption(Vector3 hitVec)
    {
        curSpawnCount--;
    }

#endregion
}
