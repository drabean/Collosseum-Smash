using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : Attack
{
    public float range;

    public override GameObject ShowWarning(Vector3 startPos, Vector3 targetPos, float time, float size = 1)
    {
        curWarning = GameMgr.Inst.AttackEffectCircle(targetPos, range, time);
        return curWarning;
    }

    public override void Shoot(Vector3 startPos, Vector3 targetPos)
    {
        if (SFXName != "") SoundMgr.Inst.Play(SFXName);
        transform.position = targetPos;
    }
}
