using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour
{
    protected GameObject curWarning;


    public abstract GameObject ShowWarning(Vector3 startPos, Vector3 targetPos, float time);

    public abstract void Shoot(Vector3 startPos, Vector3 targetPos);
}
