using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Attack : MonoBehaviour
{
    protected GameObject curWarning;
    public string SFXName;
    public Action attackAction;
    public abstract GameObject ShowWarning(Vector3 startPos, Vector3 targetPos, float time, float size = 1);

    public abstract void Shoot(Vector3 startPos, Vector3 targetPos);
}
