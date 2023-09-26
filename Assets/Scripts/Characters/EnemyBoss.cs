using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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


    public override void onHit(Transform attackerPos, float dmg, float stunTime = 0.3F)
    {
        base.onHit(attackerPos, dmg, stunTime);
        UIMgr.Inst.progress.SetBossHP(curHP, maxHP);
    }

    int explosionRepeatTime = 10;


    protected override IEnumerator co_Smash(Transform attackerPos)
    {
        GameMgr.Inst.stopSpawningNormalEnemy();
        GameMgr.Inst.MainCam.changeTarget(transform);
        //죽은 적을 다시 공격하는 것을 막기 위해, 콜라이더 없애줌
        Collider2D[] cols = GetComponentsInChildren<Collider2D>();
        if(cols.Length != 0)
        {
            foreach(Collider2D col in cols)
            {
                Destroy(col);
            }
        }

        GameMgr.Inst.SlowTime(3f, 0.3f, true);
        //여기서 적 사망 연출
        for(int i = 0; i < explosionRepeatTime; i++)
        {
            hit.FlashWhite(0.2f);
            GameObject temp = DictionaryPool.Inst.Pop("Prefabs/Effect/ExplosionEffect");
            temp.transform.position = transform.position + Vector3.right *  Random.Range(-size, size) + Vector3.up * Random.Range(-size, size);
            yield return new WaitForSecondsRealtime(0.3f);
        }

        SoundMgr.Inst.Play("Smash");
        yield return base.co_Smash(attackerPos);
        GameMgr.Inst.MainCam.changeTargetToDefault();
    }
}
