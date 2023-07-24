using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer; // 검사할 대상 레이어
    [SerializeField] private float detectionRadius = 2f; // 검사할 반경

    [SerializeField] Player owner;
    private void Update()
    {
        // 주변에 대상 레이어의 오브젝트가 있는지 검사
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, targetLayer);
        owner.isInAttackRange = (colliders.Length > 0);
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
#endif
}
