using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMgr : Singleton<UIMgr>
{
    public UIScoreCanvas score;
    public UIHPCanvavs hp;
    public UIJoystickCanvas joystick;


    private void Start()
    {
        //�׽�Ʈ
        testF();
    }
    [ContextMenu("TEST")]
    public  void testF()
    {
        score.Set(0);
    }
}
