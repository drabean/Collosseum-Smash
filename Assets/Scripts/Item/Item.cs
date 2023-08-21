using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item : MonoBehaviour
{
    public Action onAcquire;
    void InvokeOnAcquire() { onAcquire?.Invoke(); }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player target;
        if(collision.TryGetComponent<Player>(out target))
        {
            onAcquired(target);
            StartCoroutine(co_AcquireItem());
        }
    }
    protected virtual void onAcquired(Player player)
    {
        Debug.Log("GotItem!");
    }

    IEnumerator co_AcquireItem()
    {
        GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Materials/FlashWhite");
        InvokeOnAcquire();

        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
