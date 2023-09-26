using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audience : MonoBehaviour
{
    Animator anim;
    SpriteRenderer sp;
    Coroutine cheerCoroutine;

    int bossKlilCoin = 5;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        sp.flipX = (transform.position.x > 0);
        cheerCoroutine = StartCoroutine(co_Cheer());
    }

    /// <summary>
    /// 일정 시간마다 환호함
    /// </summary>
    /// <returns></returns>
    IEnumerator co_Cheer()
    {
       while(true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 4f));
            Cheer();
        }
    }
    public void Cheer()
    {
        //효과음 
        anim.SetTrigger("doCheer");
    }
    /// <summary>
    /// 계속 환호하는 모드로 변경 (보스 처치 시 호출)
    /// </summary>
    public void onEnemyKill()
    {
        StopCoroutine(cheerCoroutine);
        cheerCoroutine = StartCoroutine(co_CheerRepeat());
    }
    IEnumerator co_CheerRepeat()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        while (true)
        {
            if(bossKlilCoin > 0)
            {
                bossKlilCoin--;
                throwCoin();
            }

            yield return new WaitForSeconds(0.9f);
            Cheer();
        }
    }
    public void throwCoin()
    {
        anim.SetTrigger("doCheer");
        ItemCoin itemCoin = DictionaryPool.Inst.Pop("Prefabs/Item/ItemCoin").GetComponent<ItemCoin>();

        itemCoin.transform.position = transform.position;

        itemCoin.Throw(EnemyMgr.Inst.getRandomPos());

    }

}
