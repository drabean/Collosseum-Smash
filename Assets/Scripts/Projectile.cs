using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 moveVec;
    public float moveSpeed;


    public void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + moveVec, moveSpeed * Time.deltaTime);
    }
}
