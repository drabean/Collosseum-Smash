using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : Enemy
{
    Vector3 destination;

    private void Start()
    {
        StartAI();
    }
    public override void StartAI()
    {
        StartCoroutine(co_Idle());
    }

    IEnumerator co_Idle()
    {
        anim.SetBool("isMoving", false);

        float waitTIme = 0.4f;

        while(waitTIme >= 0)
        {
            waitTIme -= Time.deltaTime;
            yield return null;
        }
        getNewDestination();
        StartCoroutine(co_Patrol());
        yield break;
    }

    IEnumerator co_Patrol()
    {
        while(Vector3.Distance(transform.position, destination) >= 0.3f)
        {
            Debug.Log("A");
            moveTowardTarget(destination);
            yield return null;
        }

        StartCoroutine(co_Idle());
        yield break;
    }

    void getNewDestination()
    {
        destination = Vector3.right * Random.Range(-3.5f, 3.5f) + Vector3.up * Random.Range(-1.4f, 1.4f);
    }
}
