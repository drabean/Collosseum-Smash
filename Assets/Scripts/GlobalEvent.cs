using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GlobalEvent : Singleton<GlobalEvent>
{
    public Action onEnemyDie;
}
