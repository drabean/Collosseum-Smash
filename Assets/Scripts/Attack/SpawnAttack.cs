using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAttack : Attack
{
    public string SpawnName;

    public override GameObject ShowWarning(Vector3 startPos, Vector3 targetPos, float time)
    {
        curWarning = GameMgr.Inst.AttackEffectCircle(targetPos, 1.5f, time);
        return curWarning;
    }

    public override void Shoot(Vector3 startPos, Vector3 targetPos)
    {
        DictionaryPool.Inst.Pop(SpawnName).transform.position = targetPos;

        Destroy(gameObject);
    }
}
