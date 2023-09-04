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




    private void Start()
    {
        if (ownerTr == null) ownerTr = transform;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<CharacterBase>(out CharacterBase character))
        {
            character.onHit(ownerTr, dmg, StunTIme);

            if (cantPenetrate)
            {
                Destroy(gameObject);
            }
        }
    }
}
