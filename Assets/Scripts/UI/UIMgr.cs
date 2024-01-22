using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMgr : MonoBehaviour
{
    public static UIMgr Inst;

    private void Awake()
    {
        Inst = this;
    }

    public UIPauseCanvas score;
    public UIHPCanvavs hp;
    public UIControlCanvas joystick;
    public UIProgressCanvas progress;
    public UIItemDescriptionCanvas itemDescription;
}
