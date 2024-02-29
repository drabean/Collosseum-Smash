using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleFollow : MonoBehaviour
{
    //추격대상
    public Transform Target;
    //추격속도
    [SerializeField] float moveSpd;
    //거리가 해당수치 미만이면 추격 안함
    [SerializeField] float mindist;

    private void Update()
    {
        if (Target == null) return;

        if(Vector3.Distance(transform.position, Target.position) >= mindist)
        {
            transform.position = Vector3.MoveTowards(transform.position, Target.position, Time.deltaTime * moveSpd);
        }
    }
}
