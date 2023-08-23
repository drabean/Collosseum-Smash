using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ModuleAttack : MonoBehaviour
{
    public bool isStun;
    public bool isMelee;
    public bool cantPenetrate;

    public Transform ownerTr;

    public Action onHit;
    void InvokeOnHit() { onHit?.Invoke(); }


    private void Start()
    {
        if (ownerTr == null) ownerTr = transform;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<CharacterBase>(out CharacterBase character))
        {
            if (!isStun) character.Hit(transform, isMelee);
            else character.Stun(transform);

            InvokeOnHit();
            if (cantPenetrate)
            {
                Destroy(gameObject);
            }
        }
    }
}
