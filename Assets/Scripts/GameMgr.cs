using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMgr : MonoSingleton<GameMgr>
{
    CameraController m_mainCam;

    protected void Awake()
    {
        m_mainCam = Camera.main.GetComponent<CameraController>();

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
    Enemy curSpawnedEnemy;

    public void GameLogic()
    {
        if(curSpawnRoutine != null)StopCoroutine(curSpawnRoutine);
        curSpawnRoutine = StartCoroutine(normalEnemySpawnRoutine(info.enemy[0]));
        SoundMgr.Inst.PlayBGM("Temp");
        spawnBossEnemy();
    }


    //임시변수
    int dif = 4;
    int stageCount = 0;




    Coroutine curNormalEnemyRoutine;

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
        SceneManager.LoadScene("Main");
    }

    #region 유틸 함수들
    int score = 0;
    public void addScore(int score)
    {
        this.score += score;
        UIMgr.Inst.score.Set(this.score);

    }


    /// <summary>
    /// 카메라를 일정 시간동안 흔듭니다.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="shakeSpeed"></param>
    /// <param name="xPower"></param>
    /// <param name="yPower"></param>
    public void Shake(float duration, float shakeSpeed, float xPower, float yPower = 0, bool isForced = false)
    {
        m_mainCam.Shake(duration, shakeSpeed, xPower, yPower, isForced);
    }

    /// <summary>
    /// 카메라를 일정 시간동안 확대합니다.
    /// </summary>
    /// <param name="duration">확대 후 원래대로 돌아 올 때 까지의 시간</param>
    /// <param name="power">확대 비율 (%)</param>
    public void Zoom(float duration, float power)
    {
        m_mainCam.Zoom(duration, power);
    }


    /// <summary>
    /// 일정 시간동안 시간 배율을 조절합니다.
    /// </summary>
    /// <param name="time"> 배율을 조절하는 시간 </param>
    /// <param name="amount"> 시간 배율 </param>
    public void SlowTime(float time, float amount)
    {
        StartCoroutine(co_SlowTime(time, amount));
    }

    IEnumerator co_SlowTime(float time, float amount)
    {
        Time.timeScale = amount;

        yield return new WaitForSecondsRealtime(time);

        Time.timeScale = 1;
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
