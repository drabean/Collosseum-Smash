using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer; // �˻��� ��� ���̾�
    [SerializeField] private float detectionRadius = 2f; // �˻��� �ݰ�

    [SerializeField] Player owner;
    private void Update()
    {
        // �ֺ��� ��� ���̾��� ������Ʈ�� �ִ��� �˻�
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
