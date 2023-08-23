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

    public UIScoreCanvas score;
    public UIHPCanvavs hp;
    public UIJoystickCanvas joystick;
    public AtkBtn atkBtn;
}
