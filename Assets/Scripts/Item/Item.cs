using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item : MonoBehaviour
{
    public Action onAcquire;
    protected void InvokeOnAcquire() { onAcquire?.Invoke(); }
    protected Animator anim;
   [SerializeField]protected SpriteRenderer sp;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private IEnumerator Start()
    {
        Material origin = sp.material;
        sp.material = Resources.Load<Material>("Materials/FlashWhite");

        yield return new WaitForSeconds(0.2f);
        sp.material = origin;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
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

    protected virtual IEnumerator co_AcquireItem()
    {
        Material origin = sp.material;
        sp.material = Resources.Load<Material>("Materials/FlashWhite");
        InvokeOnAcquire();

        yield return new WaitForSeconds(0.2f);
        sp.material = origin;
        Destroy(gameObject);
    }
}
