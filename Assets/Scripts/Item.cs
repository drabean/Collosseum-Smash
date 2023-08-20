using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player target;
        if(collision.TryGetComponent<Player>(out target))
        {
            onAcquired(target);
        }
    }
    protected virtual void onAcquired(Player player)
    {
        Debug.Log("GotItem!");
    }
}
