using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeGroup : MonoBehaviour
{
    public List<Animator> anims;


    public void ShowWarning(float time)
    {
        foreach (Animator anim in anims)
        {
            GameMgr.Inst.AttackEffectLinear(anim.transform.position+ Vector3.right * -0.7f, anim.transform.position + Vector3.right * 0.7f, 1.4f, time);
        }
    }

    public void Show()
    {
        foreach (Animator anim in anims)
        {
            anim.SetTrigger("On");
        }
    }
    [ContextMenu("TEST")]
    public void Hide()
    {
        foreach(Animator anim in anims)
        {
            anim.SetTrigger("Off");
        }
    }
}
