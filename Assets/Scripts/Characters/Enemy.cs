using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : CharacterBase
{
    [Header("Àû")]
    public Transform Target;

    public virtual void StartAI() { }

    public override void Hit()
    {
        Debug.Log("OUC");
    }
}
