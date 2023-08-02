using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GlobalEvent : MonoSingleton<GlobalEvent>
{
    public Action onEnemyDie;
}
