using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ModuleAttack : MonoBehaviour
{
    public float StunTIme;
    public bool cantPenetrate;
    public float dmg;
    public Transform ownerTr;

    public int maxHit;
    int curHit=0;

    private void Start()
    {
        if (ownerTr == null) ownerTr = transform;
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<CharacterBase>(out CharacterBase character))
        {
            character.onHit(ownerTr, dmg, StunTIme);
            curHit++;

            if(curHit >= maxHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
