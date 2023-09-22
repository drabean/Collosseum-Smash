using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolable_Animator : Poolable
{
    Animator anim;
    public string SFXName;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        ActionPop += onPop;
    }

    void onPop()
    {
        anim.SetTrigger("Awake");
        if (SFXName != "") SoundMgr.Inst.Play(SFXName);
    }
}
