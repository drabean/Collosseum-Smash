using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCoin : Item
{
    protected override void onAcquired(Player player)
    {
        Debug.Log("GotCoin!");
        SoundMgr.Inst.Play("Coin");
    }

    public void Throw(Vector3 destination)
    {
        StartCoroutine(co_Throw(destination));
    }


    IEnumerator co_Throw(Vector3 destination)
    {
        float timeLeft = 1;
        float moveSpeed = Vector3.Distance(transform.position, destination);
        anim.SetTrigger("doThrow");
        while (timeLeft >= 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

            timeLeft -= Time.deltaTime;
            yield return null;
        }
    }
}
