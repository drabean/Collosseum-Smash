using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActionInvoker : MonoBehaviour
{
    //EnemyMgr�� ��ġ�� �ʰ� �ٷ� ��ȯ�Ǹ� �ൿ�ؾ� �ϴ� ���� ����� ��ũ��Ʈ.
    private void Start()
    {
        GameObject smoke = DictionaryPool.Inst.Pop("Prefabs/Smoke");
        smoke.transform.position = transform.position;
        smoke.GetComponent<Poolable>().Push(2.0f);

        GetComponent<Enemy>().StartAction();
    }
}