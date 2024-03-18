using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EnemyMgr를 거치지 않고, 몬스터를 테스트 하고 싶을 때 사용하는 스크립트
/// </summary>
public class EnemyActionInvoker : MonoBehaviour
{
    public float timeToStart = 1.0f;
    //EnemyMgr를 거치지 않고 바로 소환되면 행동해야 하는 적이 사용할 스크립트.
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