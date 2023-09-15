using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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


    public override void onHit(Transform attackerPos, float dmg, float stunTime = 0.3F)
    {
        base.onHit(attackerPos, dmg, stunTime);
        UIMgr.Inst.progress.SetBossHP(curHP, maxHP);
    }
}
