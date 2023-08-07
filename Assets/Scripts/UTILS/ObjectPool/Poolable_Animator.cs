using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolable_Animator : Poolable
{
    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        ActionPop += () => { anim.SetTrigger("Awake"); };
    }
}
