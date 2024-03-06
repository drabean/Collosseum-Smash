using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    public void OnSmash()
    {
        StartSceneCamera.Inst.Shake();
        SoundMgr.Inst.Play("Impact");
    }
}
