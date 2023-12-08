using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleExplosiveAttack : ModuleAttack
{
    public Attack Explosion;
    bool exploded;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<CharacterBase>(out CharacterBase character))
        {
            if (exploded) return;

            exploded = true;
            Debug.Log("ASD");
            Instantiate(Explosion).Shoot(transform.position, transform.position + Vector3.up * 0.01f);
            Destroy(gameObject);
        }
    }
}
