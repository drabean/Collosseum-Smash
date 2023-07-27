using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<CharacterBase>(out CharacterBase character))
        {
            character.Hit(transform);
        }
    }
}
