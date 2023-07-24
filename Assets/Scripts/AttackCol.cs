using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//테스트코드

public class AttackCol : MonoBehaviour
{
    Collider2D col;
    [SerializeField] Transform attackerPos;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
       if( collision.TryGetComponent<Enemy>(out Enemy enem))
        {
            enem.Hit(attackerPos);
        }
    }

    public void Attack()
    {
        StartCoroutine(co_atk());
    }
    IEnumerator co_atk()
    {
        col.enabled = true;
        yield return new WaitForSeconds(0.3f);

        col.enabled = false;
    }
}
