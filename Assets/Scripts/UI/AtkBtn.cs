using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AtkBtn : MonoBehaviour
{
    public Action onPressed;
    public void invokeOnPressed() { onPressed?.Invoke(); }

    public void setTarget(Action target)
    {
        onPressed = target;
    }
}   
