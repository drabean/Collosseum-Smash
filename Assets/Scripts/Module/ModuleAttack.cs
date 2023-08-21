using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleAttack : MonoBehaviour
{
    public bool isStun;
    public bool cantPenetrate;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<CharacterBase>(out CharacterBase character))
        {
            if (!isStun) character.Hit(transform);
            else character.Stun(transform);

            if (cantPenetrate)
            {
                Destroy(gameObject);
            }
        }
    }
}
