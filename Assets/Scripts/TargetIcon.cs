using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIcon : MonoBehaviour
{
    public Transform Owner;
    public Transform Target;
    [SerializeField] float moveSpeed;

    [SerializeField] LineRenderer line;

    [HideInInspector] public Vector3 curTargetingPosition;

    private void Update()
    {
        if (Target == null) return;
        if (Owner == null) Destroy(gameObject);
        curTargetingPosition = Vector3.MoveTowards(curTargetingPosition, Target.position, moveSpeed * Time.deltaTime);
        line.SetPosition(0, Owner.position);
        line.SetPosition(1, curTargetingPosition);
        transform.position = curTargetingPosition;
    }
}
