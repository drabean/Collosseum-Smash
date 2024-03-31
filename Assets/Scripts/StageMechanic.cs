using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMechanic : MonoBehaviour
{
    public Transform target;

    public virtual void Init(Transform target)
    {
        this.target = target;
    }
    public virtual void StartAction()
    {

    }

    public virtual void endAction()
    {

    }
}
