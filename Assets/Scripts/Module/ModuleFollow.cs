using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleFollow : MonoBehaviour
{
    //�߰ݴ��
    public Transform Target;
    //�߰ݼӵ�
    [SerializeField] float moveSpd;
    //�Ÿ��� �ش��ġ �̸��̸� �߰� ����
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
