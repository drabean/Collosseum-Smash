using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Effect : MonoBehaviour
{
    public Action despawnAction;
    public void OnDespawn() { despawnAction?.Invoke(); }

    private void Awake()
    {
        despawnAction += () => Destroy(gameObject);
    }
}
