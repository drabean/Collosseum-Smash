using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCoin : Item
{
    public int value;

    public void Init(int value)
    {
        this.value = value;
        Throw(transform.position.Randomize(3.0f));
    }
    protected override void onAcquired(Player player)
    {

        SoundMgr.Inst.Play("Coin");
        LoadedSave.Inst.save.Coin++;
    }

    public void Throw(Vector3 destination)
    {
        StartCoroutine(co_Throw(destination));
    }


    IEnumerator co_Throw(Vector3 destination)
    {
        float timeLeft = 0.5f;
        float moveSpeed = Vector3.Distance(transform.position, destination) / 0.5f;
        anim.SetTrigger("doThrow");
        while (timeLeft >= 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

            timeLeft -= Time.deltaTime;
            yield return null;
        }
    }

    public void magnet(Transform target)
    {
        StartCoroutine(co_Follow(target));
    }
    IEnumerator co_Follow(Transform target)
    {
        while(Vector3.Distance(target.position, transform.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * 7.0f);
            yield return null;
        }
    }
}
