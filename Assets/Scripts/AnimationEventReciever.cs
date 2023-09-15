using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReciever : MonoBehaviour
{
    public Action commandLockStart;
    public Action commandLockEnd;

    public Action attack;
    public Action attack2;
    public Action attack3;
    public Action moveEffect;

    public void onCommandLockStart() { commandLockStart?.Invoke(); }
    public void onCommandLockEnd() { commandLockEnd?.Invoke(); }

    public void onAttackEvent() { attack?.Invoke();}
    public void onAttack2Event() { attack2?.Invoke();}
    public void onAttack3Event() { attack3?.Invoke();}
    public void onMoveEffect() { moveEffect?.Invoke();}

}
