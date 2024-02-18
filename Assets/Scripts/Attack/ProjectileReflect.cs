using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileReflect : MonoBehaviour
{
    [SerializeField] Projectile owner;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // ������ ���� ����
            Vector2 contactNormal = contact.normal;

            //�ݻ簢���
            Vector2 reflectionVector = (Vector2)owner.moveVec - 2 * Vector2.Dot(owner.moveVec, contactNormal) * contactNormal;

            owner.moveVec = reflectionVector;
        }
    }
}
