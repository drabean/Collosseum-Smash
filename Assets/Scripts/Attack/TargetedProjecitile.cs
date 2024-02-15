using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetedProjecitile : MonoBehaviour
{
    public float moveSpeed;

    public GameObject Target;
    public Action onTouch;
    public void Shoot(Vector3 startPos, GameObject Target)
    {
        transform.position = startPos;
        this.Target = Target;
    }


    private void Update()
    {
        if (Target == null)
        {
            Destroy(gameObject);
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, moveSpeed * Time.deltaTime);
        if(Vector3.Distance(transform.position, Target.transform.position) <= 0.1f)
        {
            onTouch?.Invoke();
            Destroy(gameObject);
        }
    }
}
