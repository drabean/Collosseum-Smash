using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Poolable : MonoBehaviour
{
    public Action<GameObject> ActionPush;
    public Action ActionPop;

    WaitForSeconds wait;
    IEnumerator co_PushAfterTime(float time)
    {
        if (wait == null) wait = new WaitForSeconds(time);

        yield return wait;

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
    }
}


