using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Poolable : MonoBehaviour
{
    public Action<GameObject> ActionPush;
    public Action ActionPop;
    public float autoPushTime = 0;

    IEnumerator co_PushAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        Push();
    }

    public void Push()
    {
        ActionPush?.Invoke(gameObject);
    }
    public void Push(float time)
    {
        StartCoroutine(co_PushAfterTime(time));
    }

    public void Pop()
    {
        ActionPop?.Invoke();
        if (autoPushTime != 0) Push(autoPushTime);
    }
}


