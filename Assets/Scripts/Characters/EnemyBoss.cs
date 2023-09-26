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

    int explosionRepeatTime = 10;


    protected override IEnumerator co_Smash(Transform attackerPos)
    {
        GameMgr.Inst.stopSpawningNormalEnemy();
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
        //���⼭ �� ��� ����
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
