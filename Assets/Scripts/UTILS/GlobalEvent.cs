using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GlobalEvent : Singleton<GlobalEvent>
{
    public Action onEnemyDie;
    public Action onAttack;
    public Action onCombo;

    public void EnemyDieEvent() { onEnemyDie?.Invoke(); }
    public void AttackEvent() { onAttack?.Invoke(); }
    public void ComboEvent() { onCombo?.Invoke(); }
}
