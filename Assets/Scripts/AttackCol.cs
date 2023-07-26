using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//테스트코드

public class AttackCol : MonoBehaviour
{
    Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
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
