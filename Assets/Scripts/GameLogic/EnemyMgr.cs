using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
public enum BUF
{
    NONE = 0,
    MEDAL = 1,
    BOMB = 2,
    TELEPORT = 3,
    SHOOTER = 4,
    HARDMODEDEFAULT = 10,
}

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
    public bool canSpawnEnemy = true;

    #region 외부 접근용 유틸 함수
    public Vector2 getRandomPos() { return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)]; }
    public List<Vector2> getCornerPos() { return cornerPoints; }

    public bool checkInStage(Vector3 pos)
    {
        return pos.x >= spawnArea[0].position.x && pos.x <= spawnArea[1].position.x &&
               pos.y >= spawnArea[0].position.y && pos.y <= spawnArea[1].position.y &&
               pos.z >= spawnArea[0].position.z && pos.z <= spawnArea[1].position.z;
    }
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
    public void SpawnEnemy(Enemy enemyPrefab, Vector3 position, Action<Vector3> deadOption = null, BUF buf = BUF.NONE)
    {
        canSpawnEnemy = true;
        StartCoroutine(co_SpawnEnemy(enemyPrefab, position, deadOption, buf));
    }

    /// <summary>
    /// 적 소환 코루틴
    /// </summary>
    /// <param name="enemyPrefab"></param>
    /// <param name="position"></param>
    /// <param name="deadOption"></param>
    /// <returns></returns>
    IEnumerator co_SpawnEnemy(Enemy enemyPrefab, Vector3 position, Action<Vector3> deadOption = null, BUF buf = BUF.NONE)
    {
        position = getClampedVec(position);
        GameMgr.Inst.AttackEffectCircle(position, 1.0f, 1.0f);
        Poolable warning = DictionaryPool.Inst.Pop("Prefabs/Warning").GetComponent<Poolable>();
        warning.transform.position = position;
        warning.Push(1.0f);
        yield return new WaitForSeconds(1.0f);

        GameObject smoke = DictionaryPool.Inst.Pop("Prefabs/Smoke");
        smoke.transform.position = position;
        smoke.GetComponent<Poolable>().Push(2.0f);

        if (!canSpawnEnemy) yield break;
        Enemy spawnedEnemy = Instantiate<Enemy>(enemyPrefab, position, Quaternion.identity);
        spawnedEnemy.Target = player;
        spawnedEnemy.StartAI();
        if (buf != BUF.NONE) addBufToEnemy(spawnedEnemy, buf);
        if(deadOption != null) spawnedEnemy.ActionOnDeath += deadOption;
    }

    #region 적 버프 (하드모드)

    void addBufToEnemy(Enemy enemy, BUF buf)
    {

        switch (buf)
        {
            case BUF.HARDMODEDEFAULT:
                enemy.curHP += enemy.curHP *  (0.3f);
                break;
            case BUF.MEDAL:
                GameObject Icon = DictionaryPool.Inst.Pop("Prefabs/Effect/Icon/IconMedal");
                Icon.transform.parent = enemy.transform;
                Icon.transform.localPosition = Vector3.up * 1.0f;

                //크기 증가, 체력 및 이속 증가
                enemy.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                enemy.curHP += enemy.curHP *(1.5f);
                enemy.moveSpeed *= 1.2f;
                enemy.isSuperarmor = true;
                break;
            case BUF.BOMB:
                Icon = DictionaryPool.Inst.Pop("Prefabs/Effect/Icon/IconBomb");
                Icon.transform.parent = enemy.transform;
                Icon.transform.localPosition = Vector3.up * 0.8f;

                //사망시 위치에 폭탄 설치
                enemy.ActionOnDeath += (Vector3 attackerPos) => { BUFBOMB(attackerPos, enemy.transform); };

                break;
            case BUF.TELEPORT:
                Icon = DictionaryPool.Inst.Pop("Prefabs/Effect/Icon/IconTeleport");
                Icon.transform.parent = enemy.transform;
                Icon.transform.localPosition = Vector3.up * 0.8f;
                //피격시 플레이어 주변 무작위 위치로 텔레포트.
                enemy.curHP += 3; // 원킬 방지
                enemy.ActionOnHit += (Transform attackerPos, float dmg) => { BUFTeleport(attackerPos, dmg, enemy.transform); };
                break;
            case BUF.SHOOTER:
                Icon = DictionaryPool.Inst.Pop("Prefabs/Effect/Icon/IconShooter");
                Icon.transform.parent = enemy.transform;
                Icon.transform.localPosition = Vector3.up * 0.8f;
                GameObject Shooter = Instantiate(Resources.Load<GameObject>("Prefabs/BUFShooter"));
                Shooter.transform.SetParent(enemy.transform);
                Shooter.transform.localPosition = Vector3.up * 0.8f;


                enemy.ActionOnDeath += (Vector3 attackerPos) => { Destroy(Shooter.gameObject); };
                break;
        }
    }

    void BUFTeleport(Transform attackerPos, float dmg, Transform owner)
    {
        //공격모션 중지
        owner.GetComponent<Enemy>().stun(0);
        //공격자 위치 주변, 스테이지 밖을 벗어나지 않는 범위.
        owner.transform.position = attackerPos.position.Randomize(6.0f).Clamp(spawnArea[0].position, spawnArea[1].position);
    }

    void BUFBOMB(Vector3 hitVec, Transform owner)
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/Bomb"), owner.position, Quaternion.identity);
    }

    #endregion
    public void SpawnBossEnemy(Enemy enemyPrefab, Vector3 position, bool isHardMode, Action<Vector3> deadOption = null)
    {
        StartCoroutine(co_spawnBoss(enemyPrefab, position, isHardMode, deadOption));
    }

    IEnumerator co_spawnBoss(Enemy enemyPrefab, Vector3 position,bool isHardMode, Action<Vector3> deadOption = null)
    {
        //소환 연출중에 캐릭터가 죽는걸 방지하기 위해 모든 일반적을 없애줌
        GameMgr.Inst.removeAllNormalEnemies();
        //카메라 위치 고정을 위한 임시 오브젝트 생성
        GameObject camTarget = new GameObject();
        camTarget.transform.position = position;

        GameMgr.Inst.MainCam.changeTarget(camTarget.transform);

        yield return new WaitForSeconds(1.0f);
        GameMgr.Inst.MainCam.Shake(0.2f, 20f, 0.2f, 0);
        SoundMgr.Inst.Play("Swoosh");
        yield return new WaitForSeconds(1.0f);
        GameMgr.Inst.MainCam.Shake(0.2f, 20f, 0.2f, 0);
        SoundMgr.Inst.Play("Swoosh");
        yield return new WaitForSeconds(1.0f);
        GameMgr.Inst.MainCam.Shake(0.3f, 20f, 0.2f, 0);
        SoundMgr.Inst.Play("Impact");

        Enemy spawnedEnemy = Instantiate<Enemy>(enemyPrefab, position, Quaternion.identity);
        
        spawnedEnemy.Target = player;
        spawnedEnemy.GetComponent<EnemyBoss>().isHardMode = isHardMode;
        if (deadOption != null) spawnedEnemy.ActionOnDeath += deadOption;

        UIMgr.Inst.progress.ShowBossUI();
        spawnedEnemy.curHP -= GameMgr.Inst.curRunData.bossProgress;
        Collider2D[] cols = spawnedEnemy.GetComponentsInChildren<Collider2D>();
        if (cols.Length != 0)
        {
            foreach (Collider2D col in cols)
            {
                col.enabled = false;
            }
        }
        spawnedEnemy.hit.FlashWhite(0.5f);
        yield return new WaitForSeconds(1.0f);

        if (cols.Length != 0)
        {
            foreach (Collider2D col in cols)
            {
                col.enabled = true;
            }
        }

        UIMgr.Inst.progress.SetBossHP(spawnedEnemy.curHP, spawnedEnemy.maxHP);
        if(spawnedEnemy.curHP != spawnedEnemy.maxHP)SoundMgr.Inst.Play("Hit");
        if (spawnedEnemy.curHP < ((float)spawnedEnemy.maxHP * (0.65f)))
        {
            Debug.Log("Continued boss's HP is lower than half");
            spawnedEnemy.GetComponent<EnemyBoss>().alreadyUsedPattern = true; // 반피 이하에서 continue시 발악패턴 안쓰도록 조절
        }
        else
        {
            spawnedEnemy.GetComponent<EnemyBoss>().alreadyUsedPattern = false;
        }
        spawnedEnemy.StartAI();
        GameMgr.Inst.MainCam.changeTargetToDefault();
        Destroy(camTarget);
    }


    public Vector3 getClampedVec(Vector3 position)
    {
        return position.Clamp(spawnArea[0].position, spawnArea[1].position);
    }
}

