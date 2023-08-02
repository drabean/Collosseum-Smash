using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReciever : MonoBehaviour
{
    public Action commandLockStart;
    public Action commandLockEnd;

    public Action attack;
    public Action moveEffect;

    public void onCommandLockStart() { commandLockStart?.Invoke(); }
    public void onCommandLockEnd() { commandLockEnd?.Invoke(); }

    public void onAttackEvent() { attack?.Invoke(); }
    public void onMoveEffect() { moveEffect?.Invoke(); }
    
}
