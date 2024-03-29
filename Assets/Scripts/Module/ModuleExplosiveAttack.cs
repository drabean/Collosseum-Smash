using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleExplosiveAttack : ModuleAttack
{
    public Attack Explosion;
    bool exploded;
    public GameObject owner;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<CharacterBase>(out CharacterBase character))
        {
            if (exploded) return;

            character.onHit(ownerTr, dmg, StunTIme);
            exploded = true;
            Instantiate(Explosion).Shoot(transform.position, transform.position + Vector3.up * 0.01f);
            Destroy(owner);
        }
    }
}
