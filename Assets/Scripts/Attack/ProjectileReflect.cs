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
            // 접점의 법선 벡터
            Vector2 contactNormal = contact.normal;

            // 벡터 반사 공식을 사용하여 반사 벡터 계산
            Vector2 reflectionVector = (Vector2)owner.moveVec - 2 * Vector2.Dot(owner.moveVec, contactNormal) * contactNormal;

            owner.moveVec = reflectionVector;
        }
    }
}
