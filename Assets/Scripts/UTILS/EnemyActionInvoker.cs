using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EnemyMgr�� ��ġ�� �ʰ�, ���͸� �׽�Ʈ �ϰ� ���� �� ����ϴ� ��ũ��Ʈ
/// </summary>
public class EnemyActionInvoker : MonoBehaviour
{
    public float timeToStart = 1.0f;
    //EnemyMgr�� ��ġ�� �ʰ� �ٷ� ��ȯ�Ǹ� �ൿ�ؾ� �ϴ� ���� ����� ��ũ��Ʈ.
    private IEnumerator Start()
    {
        Debug.Log("TESTMODE");
        //GameObject smoke = DictionaryPool.Inst.Pop("Prefabs/Smoke");
        //smoke.transform.position = transform.position;
        //smoke.GetComponent<Poolable>().Push(2.0f);

        yield return new WaitForSeconds(1.0f);
        GetComponent<Enemy>().StartAction();
    }
}